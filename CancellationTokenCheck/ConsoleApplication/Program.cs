using System.Diagnostics;

var source = new CancellationTokenSource();
var token = source.Token;
//CancellationToken needs to be make from CancellationTokenSource to have all functionality 

source.CancelAfter(TimeSpan.FromSeconds(2));

var sw = new Stopwatch();
sw.Start();
var tasks = new[]
{
    new Test().DoIterativeJobAsync(token),
    new Test().DoHttpJobAsync(token)
};

try
{
    await Task.WhenAll(tasks);
}
catch(OperationCanceledException ex) when (ex.CancellationToken == token)
{
    Console.WriteLine("Operation has been cancelled. Exception thrown");
}
finally
{
    source.Dispose();
}

sw.Stop();
Console.WriteLine($"Operation cancelled within {sw.Elapsed.TotalMilliseconds} ms.");
// Results with cancellation token ->
// Operation has been cancelled. Exception thrown
// Operation cancelled within 2013,4343 ms.
// Result near to 2 sec because of line 7: source.CancelAfter(TimeSpan.FromSeconds(2));


class Test
{
    public async Task DoIterativeJobAsync(CancellationToken token)
    {
        var i = 0;
        while(i < 30)
        {
            await Task.Delay(100, token);
            Console.WriteLine($"Do iterative job async no: {i}");
            i++;
        }
    }

    public async Task DoHttpJobAsync(CancellationToken token)
    {
        var httpClient = new HttpClient();
        //Only for testing reasons. For production solutions better use httpClientFactory

        await httpClient.GetAsync("http://localhost:5043", token);
    }
}