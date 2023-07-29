var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<Job>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/", async (HttpContext ctx, CancellationToken token) => 
    {
        token.Register(() =>
        {
            Console.WriteLine("HTTP request aborted ...");
        });
        try
        {
            await Task.Delay(10_000, token);
        }
        catch (OperationCanceledException ex)
        {
            Console.WriteLine("Operation cancelled");
            return;
        }
        Console.WriteLine("Ok");
    }).WithOpenApi();

app.Run();

class Job : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (stoppingToken.IsCancellationRequested is false)
        {
            await Task.Delay(1_000);
        }
        //To stop press ctrl + c
        Console.WriteLine("Application is closing ... ");
    }
}