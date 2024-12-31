using CSharpFunctionalExtensions;
using PetFamily.Domain.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetFamily.Domain.Aggregates.Species
{
    public class Species:Entity<int>
    {
        public string Name { get; private set; }
        public List<Breed> Breeds { get; set; }= [];
        protected Species()
        {
            
        }
        private Species(string name,Breed breed)
        {
            Name = name;
            Breeds.Add(breed);
        }
        public static Result<Species> Create(string name, Breed breed)
        {
            var validateName = ValidationExtensions.ValidateName(name);
            if (validateName.IsFailure)
                return Result.Failure<Species>(validateName.Error);
            return Result.Success(new Species(name, breed));
        }
    }
}
