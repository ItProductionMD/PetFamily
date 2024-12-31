using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetFamily.Domain.ValueObjects
{
    public class DonateDetails : ValueObject
    {
        public string Name { get; private set; }
        public string Description { get; private set; }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Name;
            yield return Description;
        }
    }
}
