using System.CommandLine;
using CupAvailabilityChecker.Cli.Commands;
using CupAvailabilityChecker.Cli.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace CupAvailabilityChecker.Cli
{
    internal class Program
    {
        public static async Task<int> Main(string[] args)
        {
            using ServiceProvider serviceProvider = new ServiceCollection()
                .AddCliServices()
                .BuildServiceProvider();

            // A scope represents the execution of a single command, conceptually equivalent to
            // the scope of a request in a REST API: "scoped" services live for the duration of
            // this execution, not for the whole process lifetime.
            using IServiceScope commandScope = serviceProvider.CreateScope();
            RootCommand rootCommand = commandScope.ServiceProvider
                .GetRequiredService<RootCommandBuilder>()
                .Build();

            // Allows Ctrl+C to gracefully stop the availability polling loop (step 4), instead of
            // abruptly killing the process while a browser session is open.
            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (_, eventArgs) =>
            {
                eventArgs.Cancel = true;
                cancellationTokenSource.Cancel();
            };

            return await rootCommand
                .Parse(args)
                .InvokeAsync(cancellationToken: cancellationTokenSource.Token);
        }
    }
}
