using FileStorage.Public.Dtos;
using System.Threading.Channels;

namespace FileStorage.Infrastructure.MessageQueue;

public class FileMessageQueue : IFileMessageQueue
{
    private readonly Channel<List<FileDto>> _deleteChannel;
    public FileMessageQueue()
    {
        _deleteChannel = Channel.CreateUnbounded<List<FileDto>>();
    }

    public async Task PublicToDeletionMessage(List<FileDto> fileDtos, CancellationToken ct = default)
    {
        await _deleteChannel.Writer.WriteAsync(fileDtos, ct);
    }

    public ChannelReader<List<FileDto>> DeleteReader => _deleteChannel.Reader;
}
