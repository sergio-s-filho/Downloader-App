# DownloaderApp - C# Asynchronous Downloader

## Overview
This project contains a C# program (Downloader) designed to download multiple files concurrently from provided URLs.
The goal of this exercise is to demonstrate the ability to analyze existing code, identify concurrency issues, inefficiencies, and bad practices, and propose a robust, production-ready.

---

## Original Code Issues
The original code had several critical problems:

**Tasks not awaited**
- Downloads were started asynchronously, but the `Main` method did not await them.
- Critical: This can result in downloads not completing before the program exits and the cache remaining empty.

**Shared mutable list (`List<string>`) accessed concurrently**
- The cache list was accessed by multiple asynchronous tasks without synchronization.
- Critical: Can cause race conditions, data corruption, or runtime exceptions.

**HttpClient instantiated per request**
- A new HttpClient was created in each task.
- Critical: Inefficient and can exhaust system resources (sockets) under high load.

**Unlimited concurrency**
- All downloads were started at once without any control.
- Critical: Can overwhelm network or server resources and reduce performance.

**No exception handling per download**
- Any failed download could crash the entire application.
- Critical: Data loss and reduced reliability without proper logging.

**Premature reporting of cache size**
- Console output was executed immediately after starting tasks.
- Critical: Reported cache size is inaccurate because tasks may still be running.

---

## Improvements Implemented
The current implementation addresses all these issues:

**Thread-safe cache using `ConcurrentBag<string>`**
- Ensures safe concurrent writes from multiple asynchronous downloads.

**Awaiting all tasks using `Task.WhenAll()`**
- Guarantees that the program waits for all downloads to complete before reporting results.

**Single reusable `HttpClient` instance**
- Efficiently manages HTTP connections and prevents socket exhaustion.

**Concurrency control with `SemaphoreSlim`**
- Limits the number of simultaneous downloads to a configurable level.

**Exception handling per download**
- Each failed download logs the error without stopping the entire process.

**Separation of responsibilities**
- `Program.cs` handles entry and orchestration.
- `Downloader.cs` handles download logic, caching, and file writing.

**Configurable parameters**
- Maximum concurrent downloads and target URLs can be easily configured.

**Professional logging and progress reporting**
- Each download logs its completion and current cache size, helping with monitoring and debugging.

---

## Output Example
After running the Downloader program, the console output could look like this:

```
Downloaded https://jsonplaceholder.typicode.com/posts/3 → download_3.json, cache size now 3
Downloaded https://jsonplaceholder.typicode.com/posts/1 → download_1.json, cache size now 3
Downloaded https://jsonplaceholder.typicode.com/posts/2 → download_2.json, cache size now 3
...
All downloads completed successfully!
Total items in cache: 10
```

This output demonstrates:
- Downloads completing asynchronously
- Thread-safe cache updates
- Files saved with correct numeric order
- Reliable final cache count

---
