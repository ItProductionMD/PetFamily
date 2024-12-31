using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetFamily.Domain.PetEntities.PetType
{
    public class Species : Entity<int>
    {
        public string Name { get; private set; }
        public List<Breed> Breeds { get; private set; }
        protected Species() { }
    }
}
