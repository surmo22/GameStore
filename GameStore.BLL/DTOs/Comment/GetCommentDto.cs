namespace GameStore.BLL.DTOs.Comment;

public class GetCommentDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Body { get; set; }

    public List<GetCommentDto> ChildComments { get; set; } = [];
}