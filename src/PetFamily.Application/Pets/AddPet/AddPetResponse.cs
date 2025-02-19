using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetFamily.Application.FilesManagment;
using PetFamily.Application.FilesManagment.Dtos;

namespace PetFamily.Application.Pets.CreatePet;

public record AddPetResponse(Guid PetId, int SerialNumber);

