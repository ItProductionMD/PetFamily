using PetFamily.Application.Commands.FilesManagment.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace PetFamily.Application.Commands.FilesManagment;

public class FilesProcessingQueue
{
    public Channel<List<AppFileDto>> DeleteChannel { get; }
    public FilesProcessingQueue()
    {
        DeleteChannel = Channel.CreateUnbounded<List<AppFileDto>>();
    }
}
