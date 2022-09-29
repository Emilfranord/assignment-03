namespace Assignment3.Entities;

public class Tag
{
    public int Id { get; set; }
    [StringLength(5)] //Dosent Work
    public string Name { get; set; }
    public virtual ICollection<Task> Tasks { get; set; }

    public Tag(string name)
    {
        Tasks = new HashSet<Task>();
        Name = name;
    }
}
/* Id : int
Name : string(50), required, unique
Tasks : many-to-many reference to Task entity */