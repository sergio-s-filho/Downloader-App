using System.Collections.Concurrent;

namespace DownloaderApp;

/// <summary>
/// The Downloader class is responsible for fetching data from a set of URLs concurrently,
/// caching results in memory, and saving them to files.
/// It demonstrates concurrency control using SemaphoreSlim and thread-safe collections.
/// </summary>

public class Downloader{

    // Thread-safe cache to store downloaded content
    private static readonly ConcurrentBag<string> _cache = new();

    // Shared HttpClient instance for efficiency and to avoid socket exhaustion
    private static readonly HttpClient _httpClient = new();
    
    // Semaphore to limit the number of concurrent downloads (set to 3 here)
    private static readonly SemaphoreSlim _semaphore = new(3);


    /// <summary>
    /// Initiates multiple downloads concurrently and waits for completion.
    /// </summary>
    
    public async Task RunDownloadsAsync(){
        var totalDownloads = 10;
        var urls = new string[totalDownloads];
        
        // Prepare a list of URLs to be downloaded
        for (var i = 1; i <= totalDownloads; i++){
            urls[i - 1] = $"https://jsonplaceholder.typicode.com/posts/{i}";
        }
        
        // Create a task for each download request
        var tasks = new Task[totalDownloads];
        for (var i = 0; i < totalDownloads; i++){
            var index = i + 1; // maintain a file naming convention
            tasks[i] = DownloadAsync(urls[i], index);
        }
        
        // Await completion of all download tasks
        await Task.WhenAll(tasks);

        Console.WriteLine("Cache size: " + _cache.Count);
    }
    
    /// <summary>
    /// Downloads content from the given URL, stores it in memory,
    /// and writes it to a local JSON file.
    /// </summary>
    /// <param name="url">The URL to download data from.</param>
    /// <param name="index">The file index for naming output files.</param>
    
    private static async Task DownloadAsync(string url, int index){
        await _semaphore.WaitAsync();
        try{
            var data = await _httpClient.GetStringAsync(url);
            _cache.Add(data);
            var fileName = $"download_{index}.json";
            await File.WriteAllTextAsync(fileName, data);
            Console.WriteLine($"Downloaded {url} → {fileName}, cache size now {_cache.Count}");
        }
        catch (Exception ex){
            Console.WriteLine($"Failed to download {url}: {ex.Message}");
        }
        finally{
            _semaphore.Release();
        }
    }
}
