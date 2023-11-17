using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using TodoGrpc.Data;
using TodoGrpc.Models;

namespace TodoGrpc.Services;

public class ToDoService : ToDoIt.ToDoItBase
{
    private readonly AppDbContext _dbContext;

    /// <summary>
    /// Constructor
    /// </summary>
    public ToDoService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<CreateToDoResponse> CreateToDo(CreateToDoRequest request, ServerCallContext context)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Description))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply a valid object"));

        var todoItem = new TodoItem
        {
            Title = request.Title,
            Description = request.Description
        };

        await _dbContext.AddAsync(todoItem);
        await _dbContext.SaveChangesAsync();

        return new CreateToDoResponse
        {
            Id = todoItem.Id
        };
    }

    public override async Task<ReadToDoResponse> ReadToDo(ReadToDoRequest request, ServerCallContext context)
    {
        if (request.Id < 1)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply a valid id!"));


        var oneItem = await _dbContext.TodoItems.FirstOrDefaultAsync(w => w.Id == request.Id);

        if (oneItem == null)
            throw new RpcException(new Status(StatusCode.NotFound, $"No Task with id {request.Id} was found!"));

        return Convert(oneItem);
    }

    public override async Task<GetAllResponse> ListToDo(GetAllRequest request, ServerCallContext context)
    {
        var todoItems = await _dbContext.TodoItems.ToListAsync();

        var result = new GetAllResponse();

        todoItems.ForEach(oneItem => result.ToDo.Add(Convert(oneItem)));

        return result;
    }

    public override async Task<UpdateToDoResponse> UpdateToDo(UpdateToDoRequest request, ServerCallContext context)
    {
        if (request.Id < 1)
            throw new RpcException(new Status(StatusCode.InvalidArgument,  "You must supply a valid id!"));
        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Description))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply a valid object"));
        
        var oneItem = await _dbContext.TodoItems.FirstOrDefaultAsync(w => w.Id == request.Id);

        if (oneItem == null)
            throw new RpcException(new Status(StatusCode.NotFound, $"No Task with id {request.Id} was found!"));

        oneItem.Title = request.Title;
        oneItem.Description = request.Description;
        oneItem.ToDoStatus = request.ToDoStatus;

        _dbContext.Update(oneItem);
        await _dbContext.SaveChangesAsync();

        return new UpdateToDoResponse
        {
            Id = oneItem.Id
        };
    }

    public override async Task<DeleteToDoResponse> DeleteToDo(DeleteToDoRequest request, ServerCallContext context)
    {
        if (request.Id < 1)
            throw new RpcException(new Status(StatusCode.InvalidArgument,  "You must supply a valid id!"));
     
        var oneItem = await _dbContext.TodoItems.FirstOrDefaultAsync(w => w.Id == request.Id);

        if (oneItem == null)
            throw new RpcException(new Status(StatusCode.NotFound, $"No Task with id {request.Id} was found!"));

        _dbContext.Remove(oneItem);
        await _dbContext.SaveChangesAsync();

        return new DeleteToDoResponse()
        {
            Id = request.Id
        };
    }

    private ReadToDoResponse Convert(TodoItem oneItem)
    {
        return new ReadToDoResponse
        {
            Id = oneItem.Id,
            Title = oneItem.Title,
            Description = oneItem.Description,
            ToDoStatus = oneItem.ToDoStatus
        };
    }
}