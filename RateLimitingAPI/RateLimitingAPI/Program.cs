using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRateLimiter(
    options =>
    {

    });



builder.Services.AddRateLimiter(
    options =>
    {
        options.RejectionStatusCode = 429;
        options.AddFixedWindowLimiter("FixedWindow", _options =>
        {
            _options.Window = TimeSpan.FromSeconds(10);
            _options.PermitLimit = 2;
            _options.QueueLimit = 0;
            _options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        });
        options.OnRejected = (context, cancellationToken) =>
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            return new ValueTask(); 
        };
    }
    );


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();

app.Run();
