using System.CommandLine;
using CupAvailabilityChecker.Cli.Commands;
using CupAvailabilityChecker.Cli.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace CupAvailabilityChecker.Cli
{
    internal class Program
    {
        public static int Main(string[] args)
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

            return rootCommand
                .Parse(args)
                .Invoke();
        }
    }
}
