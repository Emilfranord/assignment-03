namespace Assignment3.Entities.Tests;

public class UserRepositoryTests
{
    private readonly KanbanContext context;
    private readonly UserRepository repository;
    public UserRepositoryTests(){
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>();
        builder.UseSqlite(connection);

        var _context = new KanbanContext(builder.Options);
        _context.Database.EnsureCreated();
        var daniel = new User("Daniel", "daniel@jensen.dk");
        daniel.Tasks.Add(new Task("title", State.New));
        var lars = new User("Lars", "lars@me.com");

        _context.AddRange(daniel, lars);
        
        _context.SaveChanges();

        context = _context;
        repository = new UserRepository(context);
    }
    [Fact]
    public void Create_given_User_returns_Created_User_Id()
    {
        var (response, created) = repository.Create(new UserCreateDTO("Anna", "ABC@gmail.com"));

        response.Should().Be(Created);

        created.Should().Be(3);
    }

    [Fact]
    public void User_email_has_to_be_Uniqe()
    {
        var (response, oldTagID) = repository.Create(new UserCreateDTO("Lene", "lars@me.com"));

        response.Should().Be(Conflict);
        oldTagID.Should().Be(2);
    }

    [Fact]
    public void Delete_non_exits_User_retun_Not_Found()
    {
        var response = repository.Delete(4);    

        response.Should().Be(NotFound);
    }
    [Fact]
    public void Delete_User_return_Deleted()
    {
        var response = repository.Delete(2);    

        response.Should().Be(Deleted);
    }

    [Fact]
    public void Read_2_Should_return_Lars_and_larsEmail()
    {
       var user = repository.Read(2);
       
       user.Should().Be(new UserDTO(2, "Lars", "lars@me.com"));
    }
    [Fact]
    public void Read_5_Should_return_null()
    {       
       repository.Read(5).Should().Be(null);
    }

     [Fact]
    public void Delete_user_in_use_return_Conflict()
    {
        var response = repository.Delete(1);    

        response.Should().Be(Conflict);
    }
    [Fact]
    public void Delete_user_in_use_with_force_return_Deleted()
    {
        var response = repository.Delete(1, true);    

        response.Should().Be(Deleted);
    }
    [Fact]
    public void ReadAll_returns_all_users() => repository.ReadAll().Should().BeEquivalentTo(new UserDTO[] {new UserDTO(1, "Daniel", "daniel@jensen.dk"), new UserDTO(2, "Lars", "lars@me.com")});
    
    [Fact]
    public void User_update_should_return_Updated() {
        repository.Update(new UserUpdateDTO(2, "Lars jensen", "hej@med.dig")).Should().Be(Updated);
        repository.Read(2).Should().Be(new UserDTO(2, "Lars jensen", "hej@med.dig"));
    }
    [Fact]
    public void User_that_dosent_exits_update_should_return_Notfund() => repository.Update(new UserUpdateDTO(5, "Finished", "finish@me.com")).Should().Be(NotFound);
    
    public void User_update_to_exiting_email_should_return_Conflict() => repository.Update(new UserUpdateDTO(2, "lars 2", "daniel@jensen.dk")).Should().Be(NotFound);
    
    
    public void Dispose()
    {
        context.Dispose();
    }
}

