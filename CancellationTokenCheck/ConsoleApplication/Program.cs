using System.Diagnostics;

var sw = new Stopwatch();
sw.Start();
var tasks = new[]
{
    new Test().DoIterativeJobAsync(),
    new Test().DoHttpJobAsync()
};

await Task.WhenAll(tasks);


sw.Stop();
Console.WriteLine($"Operation cancelled within {sw.Elapsed.TotalMilliseconds} ms.");
//Without cancellation token result -> Operation cancelled within 10074,6948 ms.


class Test
{
    public async Task DoIterativeJobAsync()
    {
        var i = 0;
        while(i < 30)
        {
            await Task.Delay(100);
            Console.WriteLine($"Do iterative job async no: {i}");
            i++;
        }
    }

    public async Task DoHttpJobAsync()
    {
        var httpClient = new HttpClient();
        //Only for testing reasons. For production solutions better use httpClientFactory

        await httpClient.GetAsync("http://localhost:5043");
    }
}
