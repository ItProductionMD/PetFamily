using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetFamily.Infrastructure.Configurations;

public static class Comparers
{
    public class ReadOnlyListComparer<T> : ValueComparer<IReadOnlyList<T>>
    where T : IEquatable<T>
    {
        public ReadOnlyListComparer()
            : base(
                (c1, c2) =>
                c1 != null
                && c2 != null
                && c1.Count == c2.Count
                && c1.ToHashSet().SetEquals(c2),//Compare Hashset collection with elements from list

                c => c != null ? c.Aggregate(0, (a, v) => HashCode.Combine(a, v!.GetHashCode())) : 0,

                c => c != null ? c.ToList() : new List<T>())
        { }
    }
}
