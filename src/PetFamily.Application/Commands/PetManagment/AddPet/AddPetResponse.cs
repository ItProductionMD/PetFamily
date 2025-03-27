using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetFamily.Application.Commands.PetManagment.AddPet;

public record AddPetResponse(Guid PetId, int SerialNumber);

