using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRateLimiter(
    options =>
    {
        options.RejectionStatusCode = 429;
        options.AddFixedWindowLimiter(policyName: "FixedWindow", configureOptions: _options =>
        {
            _options.Window = TimeSpan.FromSeconds(10);
            _options.PermitLimit = 5;
            _options.QueueLimit = 0;
            _options.AutoReplenishment = true;
            _options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        });
    }
);

builder.Services.AddRateLimiter(
    options =>
    {
        options.RejectionStatusCode = 429;
        options.AddFixedWindowLimiter(policyName: "FixedWindowInOneInQueue", configureOptions: _options =>
        {
            _options.Window = TimeSpan.FromSeconds(10);
            _options.PermitLimit = 5;
            _options.QueueLimit = 1;
            _options.AutoReplenishment = true;
            _options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        });
    }
);

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("apikey", httpContext =>
    {
        if (httpContext.Request.Query.Keys.Contains("api_key"))
        {
            return RateLimitPartition.GetFixedWindowLimiter(
                httpContext.Request.Query["api_key"].ToString(),
                fac =>
                {
                    return new FixedWindowRateLimiterOptions
                    {
                        Window = TimeSpan.FromSeconds(60),
                        PermitLimit = 5,
                    };
                });
        }
        else
        {
            return RateLimitPartition.GetNoLimiter("");
        }
    });
});


builder.Services.AddRateLimiter(
    options =>
    {
        options.RejectionStatusCode = 429;
        options.AddConcurrencyLimiter("Concurrency", _options =>
        {
            _options.PermitLimit = 4;
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


builder.Services.AddRateLimiter(
    options =>
    {
        options.RejectionStatusCode = 429;
        options.AddSlidingWindowLimiter(policyName: "SlidingWindow", configureOptions: _options =>
        {
            _options.Window = TimeSpan.FromSeconds(10);
            _options.PermitLimit = 5;
            _options.QueueLimit = 0;
            _options.AutoReplenishment = true;
            _options.SegmentsPerWindow = 1;
            _options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        });
    }
);


builder.Services.AddRateLimiter(
    options =>
    {
        options.RejectionStatusCode = 429;
        options.AddTokenBucketLimiter("TokenBucket", _options =>
        {
            _options.AutoReplenishment = true;
            _options.ReplenishmentPeriod = TimeSpan.FromSeconds(12);
            _options.TokensPerPeriod = 5;
            _options.TokenLimit = 5;
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



 