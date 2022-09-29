namespace Assignment3.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public List<Task> Tasks { get; set; }

    public User(string name, string email){
        Name = name;
        Email = email;
        Tasks = new List<Task>();
    }
}
/* Id : int
Name : string(100), required
Email : string(100), required, unique
Tasks : list of Task entities belonging to User */