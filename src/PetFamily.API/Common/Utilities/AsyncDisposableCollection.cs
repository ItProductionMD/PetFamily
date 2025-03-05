﻿namespace PetFamily.API.Common.Utilities;

public class AsyncDisposableCollection : IAsyncDisposable
{
    private readonly List<IAsyncDisposable> _disposables;

    public AsyncDisposableCollection(IEnumerable<IAsyncDisposable> disposables)
    {
        _disposables = disposables.ToList();
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var disposable in _disposables)
        {
            await disposable.DisposeAsync();
        }
    }
}

