using System.Collections.Concurrent;

namespace DownloaderApp;

public class Downloader
{
    private static readonly ConcurrentBag<string> _cache = new();
    private static readonly HttpClient _httpClient = new();
    private static readonly SemaphoreSlim _semaphore = new(3);
        
    public async Task RunDownloadsAsync()
    {
        var totalDownloads = 10;
        var urls = new string[totalDownloads];

        for (var i = 1; i <= totalDownloads; i++)
        {
            urls[i - 1] = $"https://jsonplaceholder.typicode.com/posts/{i}";
        }

        var tasks = new Task[totalDownloads];
        for (var i = 0; i < totalDownloads; i++)
        {
            var index = i + 1;
            tasks[i] = DownloadAsync(urls[i], index);
        }

        await Task.WhenAll(tasks);

        Console.WriteLine("Cache size: " + _cache.Count);
    }

    private static async Task DownloadAsync(string url, int index)
    {
        await _semaphore.WaitAsync();
        try
        {
            var data = await _httpClient.GetStringAsync(url);
            _cache.Add(data);
            var fileName = $"download_{index}.json";
            await File.WriteAllTextAsync(fileName, data);
            Console.WriteLine($"Downloaded {url} → {fileName}, cache size now {_cache.Count}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to download {url}: {ex.Message}");
        }
        finally
        {
            _semaphore.Release();
        }
    }
}