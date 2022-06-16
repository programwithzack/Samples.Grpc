using Grpc.Core;
using Bogus;

namespace Samples.Grpc.Api.Services
{
    public class TodosService : Todos.TodosBase
    {

        private readonly static List<Todo> _todos;
        private readonly static bool[] _todoCompleted = new[] { true, false };
        static TodosService()
        {
            int id = 1;
            var lorem = new Bogus.DataSets.Lorem("en");
            var generator = new Faker<Todo>("en")
            .RuleFor(o => o.UserId, f => f.Random.Number(0, 5))
            .RuleFor(o => o.Id, f => id++)
            .RuleFor(o => o.Title, f => lorem.Sentence())
            .RuleFor(o => o.Completed, f => _todoCompleted[f.Random.Number(0, _todoCompleted.Length - 1)]);
            _todos = Enumerable.Range(1, 99).Select(i => generator.Generate()).ToList();
        }

        private readonly ILogger<TodosService> _logger;
        public TodosService(ILogger<TodosService> logger)
        {
            _logger = logger;
        }

        public override Task<Todo> GetTodo(GetTodoRequest request, ServerCallContext context)
        {
            return Task.FromResult(_todos.Single(t => t.Id == request.Id));
        }

        public override Task<GetTodosResponse> GetTodos(GetTodosReqeust request, ServerCallContext context)
        {

            var result = new GetTodosResponse();
            if (request.UserId > 0)
                result.Todos.AddRange(_todos.Where(t => t.UserId == request.UserId));
            else
                result.Todos.AddRange(_todos);

            return Task.FromResult(result);

        }

    }
}
