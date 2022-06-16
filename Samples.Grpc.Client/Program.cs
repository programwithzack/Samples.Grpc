using Grpc.Net.Client;
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
Console.WriteLine("Press any key to exit");

Console.Read();
