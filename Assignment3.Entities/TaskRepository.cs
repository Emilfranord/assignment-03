namespace Assignment3.Entities;
using static Assignment3.Core.State;
public class TaskRepository : ITaskRepository
{
    private readonly KanbanContext context;
    public TaskRepository(KanbanContext _context)
    {
        context = _context;
    }
    public (Response Response, int TaskId) Create(TaskCreateDTO task)
    {   
        Task _task;
        HashSet<Tag> Tags = new HashSet<Tag>();
        foreach (string tagName in task.Tags){
            Tags.Add((from t in context.Tags where t.Name == tagName select t).First());
        }

        if (task.AssignedToId is not null)
        {
            var user = context.Users.Find(task.AssignedToId);
            if (user is null) 
            {
                return (BadRequest, -1);
            }else{
                _task = new Task(task.Title, New){Description = task.Description, AssignedTo = user, Tags = Tags};
            }
        }else{
            _task = new Task(task.Title, New){Description = task.Description, Tags = Tags};
        }
        
        context.Tasks.Add(_task);
        context.SaveChanges();   
        return (Created, _task.Id);
    }

    public Response Delete(int taskId)
    {
        var task = context.Tasks.Find(taskId);
        if(task is null) return NotFound;
        switch (task.State){
            case New:
                context.Tasks.Remove(task);
                return Deleted;
            case Active:
                context.Tasks.Remove(task);
                task.State = Removed;
                task.Updated = DateTime.UtcNow;
                return Deleted;
            default:
                return Conflict;
        }
    }

    public TaskDetailsDTO Read(int taskId)
    {
        var task = context.Tasks.Find(taskId);
        if(task is null) return null!;
        try
        {
        return new TaskDetailsDTO(task.Id, task.Title, task.Description!, task.created, task.AssignedTo!.Name, task.tagsToString(), task.State, task.Updated);
        }
        catch (Exception)
        {
            return new TaskDetailsDTO(task.Id, task.Title, null, task.created, null, task.tagsToString(), task.State, task.Updated);
        }
    }

    public IReadOnlyCollection<TaskDTO> ReadAll()
    {
        var tasks =  from t in context.Tasks
                     orderby t.Id
                     select new TaskDTO(t.Id, t.Title, t.AssignedTo!.Name, t.tagsToString(), t.State);
        return tasks.ToArray();
    }

    public IReadOnlyCollection<TaskDTO> ReadAllByState(State state)
    {
        var tasks =  from t in context.Tasks
                     orderby t.State
                     select new TaskDTO(t.Id, t.Title, t.AssignedTo!.Name, t.tagsToString(), t.State);
        return tasks.ToArray();
    }

    public IReadOnlyCollection<TaskDTO> ReadAllByTag(string tag)
    {
        var tasks =  from t in context.Tasks
                     orderby t.Tags
                     select new TaskDTO(t.Id, t.Title, t.AssignedTo!.Name, t.tagsToString(), t.State);
        return tasks.ToArray();
    }

    public IReadOnlyCollection<TaskDTO> ReadAllByUser(int userId)
    {
        var tasks =  from t in context.Tasks
                     orderby t.AssignedTo!.Name
                     select new TaskDTO(t.Id, t.Title, t.AssignedTo!.Name, t.tagsToString(), t.State);
        return tasks.ToArray();
    }

    public IReadOnlyCollection<TaskDTO> ReadAllRemoved()
    {
        var tasks =  from t in context.Tasks
                     orderby t.Id
                     where t.State == Removed
                     select new TaskDTO(t.Id, t.Title, t.AssignedTo!.Name, t.tagsToString(), t.State);
        return tasks.ToArray();
    }

    public Response Update(TaskUpdateDTO task)
    {
        var _task = context.Tasks.Find(task.Id);
        if (_task is null) return NotFound;
        //context.Tasks.Remove(_task); 
        var user = context.Users.Find(task.AssignedToId);
        if (user is null) return BadRequest;
        if (task.State != _task.State) _task.Updated = DateTime.UtcNow; 
        _task.Title = task.Title;
        _task.Description = task.Description;
        _task.AssignedTo = user;
        _task.State = task.State;

        HashSet<Tag> Tags = new HashSet<Tag>();
        foreach (string tagName in task.Tags){
            Tags.Add((from t in context.Tags where t.Name == tagName select t).First());
        }
        _task.Tags = Tags;
       // context.Tasks.Add(_task);

        return Updated;
    }
}
