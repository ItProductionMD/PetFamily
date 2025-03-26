using PetFamily.Application.Abstractions;
using PetFamily.Application.Commands.FilesManagment.Commands;
using PetFamily.Application.Commands.FilesManagment.Dtos;

namespace PetFamily.API.Common.Utilities
{
    public class FileProcessingResult<T,TResponse>where T : ICommand
    {
        public List<AppFileDto> AppFileDtos { get; }
        public List<T> commands { get; }
        public List<TResponse> Responses { get; }

        public FileProcessingResult(
            List<AppFileDto> fileDtosForHandle,
            List<T> commands,
            List<TResponse> responses)
        {
            AppFileDtos = fileDtosForHandle;
            this.commands = commands;
            Responses = responses;
        }
    }
}
