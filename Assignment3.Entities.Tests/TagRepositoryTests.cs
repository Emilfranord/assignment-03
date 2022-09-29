namespace Assignment3.Entities.Tests;



public class TagRepositoryTests : IDisposable
{
    private readonly KanbanContext context;
    private readonly TagRepository repository;
    public TagRepositoryTests(){
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>();
        builder.UseSqlite(connection);

        var _context = new KanbanContext(builder.Options);
        _context.Database.EnsureCreated();
        var InProgress = new Tag("In Progress"){Id = 1};
        InProgress.Tasks.Add(new Task("title", State.New));
        var Done = new Tag("Done") {Id = 2};
        _context.AddRange(InProgress, Done);
        
        _context.SaveChanges();

        context = _context;
        repository = new TagRepository(context);
    }

    [Fact]
    public void Create_given_Tag_returns_Created_Tag_Id()
    {
        var (response, created) = repository.Create(new TagCreateDTO("Long"));

        response.Should().Be(Created);

        created.Should().Be(3);
    }

    [Fact]
    public void Tag_Name_has_to_be_Uniqe()
    {
        var (response, oldTagID) = repository.Create(new TagCreateDTO("Done"));

        response.Should().Be(Conflict);
        oldTagID.Should().Be(2);
    }

    [Fact]
    public void Delete_non_exits_tag_retun_Not_Found()
    {
        var response = repository.Delete(4);    

        response.Should().Be(NotFound);
    }
    [Fact]
    public void Delete_tag_retun_Deleted()
    {
        var response = repository.Delete(2);    

        response.Should().Be(Deleted);
    }

    [Fact]
    public void Read_2_Should_return_Done()
    {
       var tagDTO = repository.Read(2);
       
       tagDTO.Name.Should().Be("Done");
    }
    [Fact]
    public void Read_5_Should_return_null()
    {       
       repository.Read(5).Should().Be(null);
    }

     [Fact]
    public void Delete_tag_in_use_return_Conflict()
    {
        var response = repository.Delete(1);    

        response.Should().Be(Conflict);
    }
    [Fact]
    public void Delete_tag_in_use_with_force_return_Deleted()
    {
        var response = repository.Delete(1, true);    

        response.Should().Be(Deleted);
    }
    [Fact]
    public void ReadAll_returns_all_tags() => repository.ReadAll().Should().BeEquivalentTo(new TagDTO[] {new TagDTO(1, "In Progress"), new TagDTO(2, "Done")});
    
    [Fact]
    public void Tag_update_should_return_Updated() {
        repository.Update(new TagUpdateDTO(2, "Finished")).Should().Be(Updated);
        repository.Read(2).Name.Should().Be("Finished");
    }
    [Fact]
    public void Tag_that_dosent_exits_update_should_return_Notfund() => repository.Update(new TagUpdateDTO(5, "Finished")).Should().Be(NotFound);
    public void Dispose()
    {
        context.Dispose();
    }
}
