using System.Diagnostics;

var source = new CancellationTokenSource();
var token = source.Token;
var token2 = source.Token;
//CancellationTokenSource.CreateLinkedTokenSource(token, token2).CancelAfter(TimeSpan.FromSeconds(1));
//CancellationToken needs to be make from CancellationTokenSource to have all functionality 

source.CancelAfter(TimeSpan.FromSeconds(2));

var sw = new Stopwatch();
sw.Start();
var tasks = new[]
{
    new Test().DoIterativeJobAsync(token2),
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
        while(i < 30 && token.IsCancellationRequested == false)
        { 
            //Do something
            await Task.Delay(100, token);
            //If cancellationToken will be only in while, returned Task from method will have RunToCompletion status
            //because token won't have info on Task level
            //In while this should be only optional
            Console.WriteLine($"Do iterative job async no: {i}");
            i++;
        }
    }

    public async Task DoHttpJobAsync(CancellationToken token)
    {
        var httpClient = new HttpClient();
        //Only for testing reasons. For production solutions better use httpClientFactory

        //Usage of callback 
        token.Register(() =>
        {
            Console.WriteLine("Cancelling HTTP request ...");
            httpClient.CancelPendingRequests();
        });
        await httpClient.GetAsync("http://localhost:5043");
        
        //await httpClient.GetAsync("http://localhost:5043", token);
    }
}