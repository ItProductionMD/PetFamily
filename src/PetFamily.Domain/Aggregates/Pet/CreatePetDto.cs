using CSharpFunctionalExtensions;
using PetFamily.Domain.Aggregates.Species;
using PetFamily.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetFamily.Domain.Aggregates.Pet
{
    public class CreatePetDto
    {
        public string? Name { get; set; }
        public Breed? Breed { get; set; }    
        public Species.Species? Species { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public StatusForHelp? StatusForHelp { get; set; }
        public string? Description { get; set; }
        public string? Adress { get; set; }
        public string? OwnerPhone { get; set; }
        public string? PaymentName { get; set; }
        public string? PaymentDescription { get; set; }
        public HealthStatus? HealthStatus { get; set; }
        public bool? IsNeutered { get; set; }
        public bool? IsVaccinated { get; set; }
        public double? Weight { get; set; }
        public double? Height { get; set; }
        public string? Color { get; set; }
    }
}
