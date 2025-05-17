using PetFamily.Tools;
using PetFamily.Tools.UsingsEdit;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Using: dotnet run -- [command]");
            Console.WriteLine("list of commands: " +
                "\n\tscaffold," +
                "\n\tseed --volunteers=<volunteersCount>" +
                "--pets=<petsOnVolunteersCount>," +
                "\n\tclear --<tableName>," +
                "\n\tusing --<operation> --usings:<old_using>%<new_using>" +
                "\n\tusing --<operation> --usings:<static:old_using>|<static:new_using>");

            return;
        }
        switch (args[0].ToLower())
        {
            case "scaffold":
                Scaffolder.RunScaffold();
                break;

            case "seed":
                Console.WriteLine($"######SeedVolunteers()");
                int volunteersCount = 10; // default
                int petsCount = 2;   // default

                foreach (var arg in args.Skip(1))
                {
                    if (arg.StartsWith("--volunteers=") && int.TryParse(arg.Split('=')[1], out var c))
                        volunteersCount = c;

                    if (arg.StartsWith("--pets=") && int.TryParse(arg.Split('=')[1], out var p))
                        petsCount = p;
                }
                await Seeder.RunSeed(volunteersCount, petsCount);
                break;

            case "clear":
                var tableName = args[1].Remove(0, 2);
                await Seeder.RunClear(tableName);
                break;

            case "using":
                var usingEditHandler = new UsingEditHandler(args);
                await usingEditHandler.Handle();
                break;

            default:
                Console.WriteLine($"unknown command: {args[0]}");
                break;
        }
    }
}

