using System.Collections.Concurrent;

namespace EncryptionApp.Api.Workers;

public class BackgroundTaskQueue
{
    private readonly ConcurrentQueue<BackgroundTask> _workItems = new ConcurrentQueue<BackgroundTask>();
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(0);

    public async Task<BackgroundTask> Dequeue(CancellationToken token)
    {
        await _semaphore.WaitAsync(token);
        _workItems.TryDequeue(out var item);

        return item;
    }

    public void Enqueue(BackgroundTask task)
    {
        _workItems.Enqueue(task);
        _semaphore.Release();
    }
}