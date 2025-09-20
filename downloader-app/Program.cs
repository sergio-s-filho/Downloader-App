namespace DownloaderApp;

public class Program {
    
    static async Task Main(string[] args)
    {
        Downloader downloader = new Downloader();
        await downloader.RunDownloadsAsync();

        Console.WriteLine("\nAll downloads completed!");
    }
}