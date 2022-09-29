namespace Assignment3.Entities;

public class UserRepository : IUserRepository
{
    private readonly KanbanContext context;
    public UserRepository(KanbanContext _context)
    {
        context = _context;
    }
    public (Response Response, int UserId) Create(UserCreateDTO user)
    {
        var entity = context.Users.FirstOrDefault(c => c.Email == user.Email);
        if (entity is not null) return (Conflict, entity.Id);

        entity = new User(user.Name, user.Email);
        
        context.Users.Add(entity);
        context.SaveChanges();
        return (Created, entity.Id);
    }

    public Response Delete(int userId, bool force = false)
    {
        var user = context.Users.Find(userId);

        if (user is null) return NotFound;
        if(user.Tasks.Count != 0 && !force) return Conflict;

        context.Users.Remove(user);
        return Deleted;
    }

    public UserDTO Read(int userId)
    {
        var user = context.Users.Find(userId);
        if (user == null) return null!;
        return new UserDTO(user.Id, user.Name, user.Email);
    }

    public IReadOnlyCollection<UserDTO> ReadAll()
    {
       var users = from u in context.Users
                   orderby u.Id
                   select new UserDTO(u.Id, u.Name, u.Email);
        return users.ToArray();
    }

    public Response Update(UserUpdateDTO user)
    {
        var _user = context.Users.Find(user.Id);
        if(_user is null) return NotFound;
        _user.Name = user.Name;
        if(context.Users.Any(c => c.Email == user.Email)) return Conflict;
        _user.Email = user.Email;
        return Updated;
    }
}
