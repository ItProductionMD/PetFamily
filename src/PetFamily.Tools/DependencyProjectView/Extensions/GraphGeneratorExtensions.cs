using Microsoft.CodeAnalysis;

namespace PetFamily.Tools.DependencyProjectView.Extensions
{
    internal static class GraphGeneratorExtensions
    {

        internal static bool ContainsKeyWord(this string projectName, IEnumerable<string> keywords)
        {
            return keywords.Any(keyword =>
                projectName.IndexOf(keyword, StringComparison.InvariantCultureIgnoreCase) >= 0);
        }
    }
}