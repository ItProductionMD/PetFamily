namespace PetFamily.API.Dtos;

public record UpdateImagesRequest(List<IFormFile> ImagesToUpload, List<string> ImagesToDelete);
