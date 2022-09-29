namespace Assignment3.Entities.Tests;

public class TaskRepositoryTests : IDisposable
{
    private readonly KanbanContext context;
    private readonly TaskRepository repository;
    public TaskRepositoryTests(){
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>();
        builder.UseSqlite(connection);

        var _context = new KanbanContext(builder.Options);
        _context.Database.EnsureCreated();
        var task1 = new Task("Homework", Active);
        var user1 = new User("kristian", "kb@itu.dk");
        task1.AssignedTo = user1;
        var task2 = new Task("Cleaning", New);
        var task3 = new Task("Running", Resolved);
        var tag1 = new Tag("tedious");
        _context.AddRange(task1, task2, task3);
        _context.Add(user1);
        _context.Add(tag1);
        
        _context.SaveChanges();

        context = _context;
        repository = new TaskRepository(context);
    }
    [Fact]
    public void Create_given_Task_returns_Created_Task_Id()
    {
        var (response, created) = repository.Create(new TaskCreateDTO("Workout", null, "kristian", new HashSet<string>()));
        
        response.Should().Be(Created);
        created.Should().Be(4);
        repository.Read(4).StateUpdated.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(5));
    }

    public void Create_given_Task_where_user_dosent_exits_returns_BadRequest()
    {
        var (response, created) = repository.Create(new TaskCreateDTO("Workout", null, "kristian", new HashSet<string>()));
        response.Should().Be(BadRequest);
    }

    [Fact]
    public void Delete_non_exits_task_retun_Not_Found()
    {
        var response = repository.Delete(4);    

        response.Should().Be(NotFound);
    }

    [Fact]
    public void Delete_new_task_return_Deleted()
    {
        var response = repository.Delete(1);    

        response.Should().Be(Deleted);
    }
    public void Delete_Active_task_return_Deleted_and_State_removed()
    {
        var response = repository.Delete(3);    

        response.Should().Be(Deleted);
        repository.Read(1).State.Should().Be(Removed);
    }
    public void Delete_other_state_task_return_Conflict()
    {
        var response = repository.Delete(2);    

        response.Should().Be(Conflict);
    }

    [Fact]
    public void Read_2_Should_return_Cleaning()
    {
       var taskDTO = repository.Read(1);
       
       taskDTO.Title.Should().Be("Cleaning");
    }
    [Fact]
    public void Read_5_Should_return_null()
    {       
       repository.Read(5).Should().Be(null);
    }

    [Fact]
    public void ReadAll_returns_all_tasks() => repository.ReadAll().Should().BeEquivalentTo(new TaskDTO[] {new TaskDTO(3, "Homework", "kristian", new HashSet<string>(), Active), new TaskDTO(1, "Cleaning", null!, new HashSet<string>(), New), new TaskDTO(2, "Running", null!, new HashSet<string>(), Resolved)});
    
    [Fact]
    public void Task_update_should_return_Updated() {
        var response = repository.Update(new TaskUpdateDTO(3, "Homework", 1, null, new HashSet<string> {"tedious"}, Resolved)).Should().Be(Updated);
        var task = repository.Read(3);
        
        //response.Should().Be(Updated);
        task.Tags.Should().Contain("tedious");
        task.State.Should().Be(Resolved);
        task.StateUpdated.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(5));
    }
    [Fact]
    public void Task_that_dosent_exits_update_should_return_Notfund() => repository.Update(new TaskUpdateDTO(5, "ab", null, null, new HashSet<string> {"tedious"}, Resolved)).Should().Be(NotFound);
 
    public void Dispose()
    {
        context.Dispose();
    }


}
