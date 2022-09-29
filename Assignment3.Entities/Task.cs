namespace Assignment3.Entities;

public class Task
{
    [Key]
    public int Id { get; set; }
    public string Title { get; set; }
    public User? AssignedTo { get; set; }
    public string? Description { get; set; }
    public State State { get; set; }
    public virtual ICollection<Tag> Tags { get; set; }
    public DateTime created { get; set; } = DateTime.UtcNow;
    public DateTime Updated { get; set; } = DateTime.UtcNow;

    public Task(string title, State state)
    {
        Title = title;
        State = state;
        Tags = new HashSet<Tag>();
    }
    public  IReadOnlyCollection<string> tagsToString(){
        var strings = new List<string>();
        foreach (var item in Tags)
        {
            strings.Add(item.Name!);
        }
        return strings.AsReadOnly();
    }
}
/* Id : int
Title : string(100), required
AssignedTo : optional reference to User entity
Description : string(max), optional
State : enum (New, Active, Resolved, Closed, Removed), required
Tags : many-to-many reference to Tag entity */
