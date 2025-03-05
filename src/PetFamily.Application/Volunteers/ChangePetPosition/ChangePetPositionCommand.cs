namespace PetFamily.Application.Volunteers.ChangePetPosition;

public record ChangePetPositionCommand(Guid VolunteerId, Guid PetId, int NewPetSerialNumber);


