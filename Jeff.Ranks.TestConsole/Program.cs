using Microsoft.Extensions.Configuration;
using System.Diagnostics;

var client = new HttpClient
{
    Timeout = TimeSpan.FromSeconds(10)
};

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .Build();


var host = config["host"];
Console.WriteLine("Please confirm that the site is up and running.");
Console.WriteLine($"Current domain default name: {host}. It can be modified in the configuration file.");
Console.Write("host:");
host = Console.ReadLine();
if(string.IsNullOrWhiteSpace(host))
    host = config["host"];

int total = 1000_000;
int concurrency = 50;

int success = 0;
int fail = 0;

var sw = Stopwatch.StartNew();

using var semaphore = new SemaphoreSlim(concurrency);

var tasks = Enumerable.Range(0, total).Select(async i =>
{
    await semaphore.WaitAsync();
    try
    {
        var customerId = Random.Shared.Next(1, 10000);
        var score = Random.Shared.Next(-1000, 1000);

        var url = $"https://localhost:7248/customer/{customerId}/score/{score}";
        var resp = await client.PostAsync(url, null);
        Console.WriteLine($"{customerId} {score}");
        if (resp.IsSuccessStatusCode)
            Interlocked.Increment(ref success);
        else
            Interlocked.Increment(ref fail);
    }
    catch
    {
        Interlocked.Increment(ref fail);
    }
    finally
    {
        semaphore.Release();
    }
});

await Task.WhenAll(tasks);

sw.Stop();

Console.WriteLine($"Total: {total}");
Console.WriteLine($"Success: {success}");
Console.WriteLine($"Fail: {fail}");
Console.WriteLine($"Elapsed: {sw.Elapsed}");

Console.ReadLine();