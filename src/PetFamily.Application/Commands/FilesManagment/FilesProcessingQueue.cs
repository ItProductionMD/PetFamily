using PetFamily.Application.Commands.FilesManagment.Dtos;
using System.Threading.Channels;

namespace PetFamily.Application.Commands.FilesManagment;

public class FilesProcessingQueue
{
    public Channel<List<AppFileDto>> DeleteChannel { get; }
    public FilesProcessingQueue()
    {
        DeleteChannel = Channel.CreateUnbounded<List<AppFileDto>>();
    }
}
