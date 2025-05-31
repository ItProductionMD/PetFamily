using PetFamily.Tools;
using PetFamily.Tools.DependencyProjectView;
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
                "\n\tusing --<operation> --usings:<static:old_using>|<static:new_using>"+
                "\n\tdiagram ");

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

            case "diagram":
                var solutionPath = GetPathToSolution();
                var graphGenerator = new SolutionDependencyGraphGenerator(
                    solutionPath,
                    ["Test","Tools"]);

                graphGenerator.GenerateDependencyGraph();

                break;

            default:
                Console.WriteLine($"unknown command: {args[0]}");
                break;
        }
    }

    public static string GetPathToSolution()
    {
        string parent1 = Directory.GetParent(Directory.GetCurrentDirectory())!.FullName;
        string parent2 = Directory.GetParent(parent1)!.FullName;
        string pathSolutoion = Path.Combine(parent2,"PetFamily.sln");

        Console.WriteLine("#######################################################");
        Console.WriteLine($"\tSolution = {pathSolutoion}");
        Console.WriteLine("#######################################################");


        if (!File.Exists(pathSolutoion))
            throw new FileNotFoundException($"Assemby fail not found: {pathSolutoion}");

        return pathSolutoion;
    }


}

