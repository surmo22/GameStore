using GameStore.Domain.Entities.UserEntities;

namespace GameStore.Domain.Entities.CoreEntities;

public class Comment
{
    public Guid Id { get; set; }

    public string Name
    {
        get
        {
            return User.UserName;
        }
    }

    public string Body { get; set; }

    public Guid GameId { get; set; }

    public Game Game { get; set; }

    public Guid? ParentId { get; set; }

    public Comment? Parent { get; set; }
    
    public Guid UserId { get; set; }
    
    public User User { get; set; }

    public ICollection<Comment> ChildComments { get; set; }
}