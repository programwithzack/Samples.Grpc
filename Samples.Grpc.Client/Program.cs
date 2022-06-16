using Flurl;
using Flurl.Http;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Samples.Grpc.Api;

Console.WriteLine("Getting todos...");
Console.WriteLine();

// Connect to the gRPC service and retrieve a list of todos.
using var channel = GrpcChannel.ForAddress("http://localhost:5177");

var client = new Todos.TodosClient(channel);
var response = await client.GetTodosAsync(new GetTodosReqeust { UserId = 0 });

// Display the number of todos found.
var todos = response.Todos;
string todoCountMessage = $"Found {todos.Count()} todos";

Console.WriteLine(todoCountMessage);
Console.WriteLine("".PadLeft(todoCountMessage.Length, '='));
Console.WriteLine();

foreach (var todo in response.Todos)
{
    Console.WriteLine($"Id: {todo.Id}, UserId: {todo.UserId}, Title: {todo.Title}, Completed: {todo.Completed}");
}

Console.WriteLine();

Console.WriteLine("Delete all todos...");

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", true, true)
    .AddUserSecrets("b7ef45d5-024d-4c2b-8efc-243ca15f0b09")
    .Build();

// Get token
var tokenResponse = await config["Auth0:Domain"]
    .AppendPathSegment("oauth/token")
    .PostUrlEncodedAsync(new
    {
        audience = config["Auth0:Audience"],
        client_id = config["Auth0:ClientId"],
        client_secret = config["Auth0:ClientSecret"],
        grant_type = "client_credentials"
    })
    .ReceiveJson();

var headers = new Metadata();
headers.Add("Authorization", $"Bearer {tokenResponse.access_token}");

await client.DeleteTodosAsync(new Empty(), headers);

Console.WriteLine("Press any key to exit");

Console.Read();
