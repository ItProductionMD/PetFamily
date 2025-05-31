using Microsoft.CodeAnalysis;
using PetFamily.Tools.DependencyProjectView.Extensions;
using System.Xml.Linq;
using static PetFamily.Tools.DependencyProjectView.Extensions.GraphGeneratorExtensions;

namespace PetFamily.Tools.DependencyProjectView;

public partial class SolutionDependencyGraphGenerator
{
    private class ProjectInfo
    {
        public String SolutionName { get; set; } = string.Empty;
        public string Name { get; set; }
        public List<string> ProjectReferences { get; set; } = [];
        public string FilePath { get; set; }
        public static IEnumerable<string> projectsKeyWords => new[] { "Public", "Contract" };
        public string ModuleName { get;private set; }
        public string Color => GetProjectColor(Name);

        public static ProjectInfo Create(
            string solutionName,
            string csprojPath, 
            XDocument doc,
            IEnumerable<string> excludeProjectsKewords)
        {
            var projectName = Path.GetFileNameWithoutExtension(csprojPath);

            var moduleName = projectName.ContainsKeyWord(projectsKeyWords)
                ? projectName.Replace(projectName.Split('.').FirstOrDefault() ?? "", solutionName)
                : projectName.Split('.').FirstOrDefault() ?? string.Empty;

            if (projectName.ContainsKeyWord(["Host","Web"]))
                moduleName = moduleName+"_Host";

            var projectReferences = doc.Descendants("ProjectReference")
                .Select(pr => Path.GetFileNameWithoutExtension(pr.Attribute("Include").Value))
                .Where(refName => refName.ContainsKeyWord(excludeProjectsKewords) == false) // Исключаем ссылки на проекты из списка
                .ToList();

            var project = new ProjectInfo
            {
                SolutionName = solutionName,
                Name = projectName,
                ProjectReferences = new(projectReferences),
                FilePath = csprojPath,
                ModuleName = moduleName.Replace('.', '_')
            };
            project.Print();

            return project;
        }
        private string GetProjectColor(string projectName)
        {
            if (projectName.ContainsKeyWord(new[] { "Public", "Contract","SharedKernel","Domain" }))
                return "red";

            if (projectName.ContainsKeyWord(["Infrastructure"]))
                return "green";

            if (projectName.ContainsKeyWord(["Application","Core"]))
                return "blue";

            if (projectName.ContainsKeyWord(["Host", "Web"]))
                return "#581C87";

            if (projectName.ContainsKeyWord(["Presentation","Shared.Api","Framework","Controllers"]))
                return "#3F1F0F";//coffe brown

            else
                return "black";
        }

        void Print()
        {
            Console.WriteLine("##########################################");
            Console.WriteLine("\tPrint project info");
            Console.WriteLine($"\t\tModuleName: {this.ModuleName}" +
                $"\n\t\tProjectName:{this.Name}" +
                $"\n\t\tSolutionName:{this.SolutionName}" +
                $"\n\t\tFilePath:{ this.FilePath}");
            Console.WriteLine("\t\tReferences:");
            foreach(var references in this.ProjectReferences)           
                Console.WriteLine($"\t\t\t-> {references}");
            Console.WriteLine("##########################################");

        }
    }
}
