using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PetFamily.Infrastructure.Contexts;
using PetFamily.Infrastructure.Dapper;
using PetFamily.Tools;
using System.Reflection;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("########################## All commands #######################################");
            Console.WriteLine("Using: dotnet run -- [command]");
            Console.WriteLine("list of commands: " +
                "\n\tscaffold //add classes with names and columns of Tables  for using Dapper easy" +
                "\n\tseed --volunteers=<volunteersCount> " +
                " --pets=<petsOnVolunteersCount> //seed the volunteers and pets data to db" +
                "\n\tclear --<tableName> //clear all data from a specific table" +
                "\n\tadd_using --class=<ClassName> --namespace=<NamespaceToAdd> //add indicated using to the classes with indicated type of Class or Interface " +
                "\n\tremove_using --remove=<static%namespace> //remove indicated usings static from all classes" +
                "\n\tremove_using --remove=<namespace> //remove indicated usings from all classes" +
                "\n\treplace_usings --old=<oldName> new=<newName>");

            Console.WriteLine("################################################################################");
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

            case "add_using":
                await UsingsAndNameSpacesRedactor.RunAddUsings(args);
                break;

            case "remove_using":  
                await UsingsAndNameSpacesRedactor.RunRemoveUsings(args);
                break;

            case "replace_usings":

                await UsingsAndNameSpacesRedactor.RunReplaceUsings(args);
                break;

            default:
                Console.WriteLine($"unknown command: {args[0]}");
                break;
        }
    }
}

