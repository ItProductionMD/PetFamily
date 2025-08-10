using FileStorage.Public.Dtos;
using System.Threading.Channels;

namespace FileStorage.Infrastructure.MessageQueue;

public interface IFileMessageQueue 
{
    Task PublicToDeletionMessage(List<FileDto> fileDtos ,CancellationToken ct = default);
    ChannelReader<List<FileDto>> DeleteReader { get; }
}
