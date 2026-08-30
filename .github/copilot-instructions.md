# Istruzioni per GitHub Copilot

Queste istruzioni valgono per l'intero repository e vanno seguite quando si genera o modifica codice C#.

## Design del codice

- **Composizione preferita all'ereditarietà.** Evitare classi astratte usate solo per condividere
  comportamento tra più implementazioni. Preferire una classe concreta "helper" riusabile, composta
  (campo privato) dalle classi che la usano, che delegano ad essa i metodi dell'interfaccia comune.
  Esempio: `ItalianEnumMapper<TEnum>` è una classe concreta composta da `AreaItalianMapper` e
  `ProvinceItalianMapper`, invece di essere una base astratta da cui ereditare.
- **Evitare duplicazione di logica**, anche piccola (es. una `string.Join` ripetuta in più punti):
  estrarre un metodo di utility condiviso (vedi `CupAvailabilityChecker.Cli/Utilities/FormatUtils.cs`).
- **Preferire metodi generici riusabili** a strutture dati vincolate a un solo tipo (es. un
  `Dictionary<Option<string>, IValidator<string>>` non è riusabile per altri tipi: meglio un metodo
  generico `AddValidator<T>(...)`).
- Un'astrazione (interfaccia, classe base, ecc.) va introdotta solo se è **effettivamente riusata** in
  più punti; se serve un solo utilizzo, preferire codice diretto e più semplice.
- **Classi piccole e focalizzate**: evitare di accumulare molti metodi privati eterogenei in una
  singola classe (es. il punto di ingresso `Program.cs`). Estrarre le responsabilità distinte in
  classi dedicate (es. `Binding/OptionValidatorBinder.cs` per il collegamento dei validatori,
  `Mapping/ItalianEnumOptionParser.cs` per il parsing degli enum), lasciando a `Program.cs` solo la
  composizione/orchestrazione di alto livello.

## Formattazione

- **Blocchi `if` con una sola istruzione**: non aprire le graffe, scrivere l'istruzione sulla riga
  successiva senza `{ }`.
  ```csharp
  if (!TryGetValue(commandResult, option, out var value))
      return;
  ```
  Le graffe vanno usate solo quando il corpo contiene più di un'istruzione.
- **Estrarre i valori intermedi in variabili locali** prima di usarli come argomento di un'altra
  chiamata, anche a costo di codice più prolisso: migliora la leggibilità e semplifica il debug
  (si può ispezionare la variabile in un breakpoint). Evitare espressioni annidate come
  `AddErrorsIfFailed(commandResult, validator.Validate(value));`; preferire:
  ```csharp
  Result validationResult = validator.Validate(value);
  AddErrorsIfFailed(commandResult, validationResult);
  ```
- **Usare `var` solo quando il tipo è ovvio dall'espressione a destra** (operatore `new`, cast
  esplicito, valore letterale). Non usare `var` solo perché il nome del metodo o della variabile
  "suggerisce" il tipo (es. `Trim()`, `GetValue(...)`, una stringa interpolata `$"..."`): in questi
  casi dichiarare il tipo esplicito (`string message = $"...";`, `IList<Province> allowedProvinces = ...;`).
  Eccezione: per un parametro `out` il cui tipo è un parametro generico già dichiarato nella firma del
  metodo (es. `out T value`), usare `out var value` — dichiarare esplicitamente il tipo generico può
  generare avvisi di nullability spuri quando il metodo usa attributi come `[MaybeNullWhen]`.
  Riferimento: [.NET Coding Conventions – Implicitly typed local variables](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions#implicitly-typed-local-variables).

## Result pattern

- Usare **FluentResults** (`FluentResults.Result` / `Result<T>`) per rappresentare l'esito di
  operazioni che possono fallire (es. validazioni), invece di creare tipi di risultato custom.

## Lingua

- **Commenti nel codice (XML doc `///` e commenti inline `//`) in inglese**, indipendentemente
  dalla lingua della conversazione con l'utente o dalla lingua dei messaggi/testi rivolti
  all'utente finale della CLI (es. descrizioni delle option, messaggi di errore), che invece
  restano in italiano perché la CLI è pensata per utenti italiani.

## Nomi

- **I nomi di classi, interfacce, record, enum ed i loro membri (proprietà, metodi) devono essere
  in inglese**, anche quando il dominio applicativo è italiano (es. `Municipality` e non `Comune`,
  `IMunicipalityRepository` e non `IComuneRepository`). Questo vale anche per i DTO interni usati
  per la deserializzazione (es. `MunicipalityJsonRecord`), le cui proprietà C# restano in inglese
  mappando le chiavi JSON italiane con `[JsonPropertyName("...")]` (la chiave JSON è un contratto
  esterno immutabile e resta quindi in italiano).
  **Eccezione**: le variabili locali che rappresentano un'opzione della CLI mantengono un nome che
  rispecchia il flag italiano corrispondente (es. `comuneOption`, `raggioOption` per `--comune`,
  `--raggio`), per coerenza con il nome del flag stesso, che resta in italiano essendo rivolto
  all'utente finale.
- **Una sola classe per file `.cs`**, anche per tipi ausiliari/interni non pubblici (es. un DTO
  usato da un solo repository, come `MunicipalityJsonRecord`): non annidare classi private/interne
  dentro la classe principale né definirne più di una nello stesso file.

## Core (`CupAvailabilityChecker.Core`)

- **Repository pattern per l'accesso ai dati di dominio**: quando un dato può in futuro provenire
  da fonti diverse (file locale, chiamata HTTP, database), definire un'interfaccia in
  `Repositories/` (es. `IMunicipalityRepository`) e un'implementazione concreta specifica per la
  fonte attuale (es. `JsonMunicipalityRepository`), tenendo i dettagli della fonte (DTO di
  deserializzazione, percorso del file, ecc.) privati all'implementazione. I consumer dipendono
  solo dall'interfaccia.
- **Logica ausiliaria scollegata dal repository** (es. calcolo di distanze geografiche) va estratta
  in una classe dedicata e riusabile, composta dal repository tramite un campo privato (es.
  `JsonMunicipalityRepository` compone `HaversineDistanceCalculator`), coerentemente con la regola
  di "composizione preferita all'ereditarietà".
- I dataset di riferimento (es. `Data/gi_comuni.json`) vanno copiati in output con
  `<None Include="..." CopyToOutputDirectory="PreserveNewest" />` nel `.csproj`.

## CLI (`CupAvailabilityChecker.Cli`)

- Il parsing degli argomenti da riga di comando usa **System.CommandLine**.
- La validazione dei parametri CLI segue queste astrazioni in `Parameters/`:
  - `IParameterValidator<T>`: valida un singolo parametro, indipendente dagli altri.
  - `IDependentParameterValidator<TValue, TDependency>`: valida un parametro la cui correttezza
    dipende dal valore di un altro parametro (es. `provincia` dipende da `area`).
- I valori enum inseriti dall'utente in italiano vengono tradotti tramite mapper in `Mapping/`
  (es. `AreaItalianMapper`, `ProvinceItalianMapper`), che compongono `ItalianEnumMapper<TEnum>`.
- **Dependency Injection**: le classi (validatori, mapper, parser) dichiarano le proprie dipendenze
  nel costruttore anziché costruirle con `new`. System.CommandLine non fornisce un meccanismo di DI
  incorporato (nessuna injection automatica negli handler dei comandi); il composition root è
  realizzato manualmente in `Program.cs` con `Microsoft.Extensions.DependencyInjection`
  (`ServiceCollection` → `AddCliServices()` in `DependencyInjection/ServiceCollectionExtensions.cs`
  → `BuildServiceProvider()`).
- Per evitare una sequenza prolissa di `GetRequiredService<T>()` in `Program.cs`, l'assemblaggio del
  `RootCommand` (option, parser, validatori) è incapsulato in `Commands/RootCommandBuilder.cs`, una
  classe che riceve tutte le sue dipendenze nel costruttore ed espone un solo metodo `Build()`.
  `RootCommandBuilder` stesso è registrato nel container: `Program.cs` risolve così un solo servizio
  (`serviceProvider.GetRequiredService<RootCommandBuilder>().Build()`).
- **Logging**: usare `Microsoft.Extensions.Logging` (`ILogger<T>` iniettato via costruttore),
  registrato con `services.AddLogging(builder => builder.AddConsole())`, invece di `Console.WriteLine`
  per tracciare informazioni (es. i parametri ricevuti dalla CLI). Usare i placeholder strutturati
  del logging (es. `{CodiceFiscale}`) anziché stringhe interpolate.
- **Lifetime dei servizi**: i servizi della CLI sono registrati come `Scoped`, non `Singleton`.
  Concettualmente, lo scope corrisponde all'esecuzione di un singolo comando, così come in una REST
  API lo scope corrisponde all'esecuzione di una request/action. `Program.cs` crea esplicitamente
  uno scope (`serviceProvider.CreateScope()`) per l'esecuzione del comando e ne risolve i servizi da
  lì. `Singleton` è riservato a servizi che devono davvero vivere per l'intero processo (es. una
  configurazione letta una sola volta all'avvio).
