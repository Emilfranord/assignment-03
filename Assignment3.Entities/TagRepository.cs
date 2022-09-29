namespace Assignment3.Entities;

public sealed class TagRepository : ITagRepository
{
    private readonly KanbanContext context;
    public TagRepository(KanbanContext _context)
    {
        context = _context;
    }
    public (Response Response, int TagId) Create(TagCreateDTO tag)
    {
        var entity = context.Tags.FirstOrDefault(c => c.Name == tag.Name);
        Response status;
        if (entity is null)
        {
            entity = new Tag(tag.Name);

            context.Tags.Add(entity);
            context.SaveChanges();

            status = Created;
        }
        else
        {
            status = Conflict;
        }

        return (status, entity.Id);
    }

    public Response Delete(int tagId, bool force = false)
    {
        var tag = context.Tags.Find(tagId);

        if (tag is null) return NotFound;
        if(tag.Tasks.Count != 0 && !force) return Conflict;

        context.Tags.Remove(tag);
        return Deleted;

    }

    public TagDTO Read(int tagId)
    {
        var tag = context.Tags.Find(tagId);
        if (tag == null) return null!;
        return new TagDTO(tag.Id, tag.Name);
    }

    public IReadOnlyCollection<TagDTO> ReadAll()
    {
        var tags = from t in context.Tags
                   orderby t.Id
                   select new TagDTO(t.Id, t.Name);
        return tags.ToArray();

    }

    public Response Update(TagUpdateDTO tag)
    {
        var _tag = context.Tags.Find(tag.Id);
        if(_tag is null) return NotFound;
        _tag.Name = tag.Name;
        return Updated;
    }
}
