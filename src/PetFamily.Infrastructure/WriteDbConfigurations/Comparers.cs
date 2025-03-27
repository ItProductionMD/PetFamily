using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetFamily.Infrastructure.WriteDbConfigurations;

public class ReadOnlyListComparer<T> : ValueComparer<IReadOnlyList<T>>
{
    public ReadOnlyListComparer() : base(
        (c1, c2) => c1 != null && c2 != null && c1.Count == c2.Count && c1.SequenceEqual(c2),
        c => c != null ? c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())) : 0,
        c => c != null ? c.ToList() : new List<T>())
    { }
}
