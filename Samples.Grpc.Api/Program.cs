using Microsoft.AspNetCore.Authentication.JwtBearer;
using Samples.Grpc.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Authentication Services
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = "https://dev-5vwcncjv.us.auth0.com/";
    options.Audience = "https://todos.testdomain.com";
});

builder.Services.AddAuthorization();

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add Grpc services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

// Enable authentication middleware
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{

    // Configure the HTTP request pipeline.
    endpoints.MapGrpcService<GreeterService>();
    endpoints.MapGrpcService<TodosService>();

    endpoints.MapGet("/", async context =>
    {
        await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
    });
});


app.Run();
