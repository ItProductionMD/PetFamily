using PetFamily.Domain.PetTypeManagment.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetFamily.Application.Queries.PetType.GetBreeds;

public record GetBreedsResponse(int BreedsCount, List<Breed> Breeds);

