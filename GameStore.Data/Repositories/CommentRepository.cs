using GameStore.BLL.Interfaces.Repositories;
using GameStore.Data.Data;
using GameStore.Domain.Entities.CoreEntities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Data.Repositories;

public class CommentRepository(GameStoreContext context) : ICommentRepository
{
    public async Task AddCommentAsync(Comment comment, CancellationToken token)
    {
        await context.Comments.AddAsync(comment, token);
    }

    public async Task<IEnumerable<Comment>> GetCommentsByGameKeyAsync(string gameKey, CancellationToken token)
    {
        var rootComments = await context.Comments
            .AsNoTracking()
            .Where(c => c.Game.Key == gameKey && c.ParentId == null)
            .Include(c => c.User)
            .Include(c => c.ChildComments)
            .AsSplitQuery()
            .ToListAsync(token);

        // Loading all levels of comments recursively
        foreach (var comment in rootComments)
        {
            await LoadChildCommentsRecursiveAsync(comment, token);
        }

        return rootComments;
    }

    public async Task<Comment> GetCommentByIdAsync(Guid id, CancellationToken token)
    {
        var comment = await context.Comments
            .AsNoTracking()
            .Include(c => c.ChildComments)
            .Include(c => c.User)
            .AsSplitQuery()
            .FirstOrDefaultAsync(c => c.Id == id, token);

        if (comment is null)
        {
            return null;
        }

        await LoadChildCommentsRecursiveAsync(comment, token);

        return comment;
    }

    public void UpdateComment(Comment comment)
    {
        context.Comments.Update(comment);
    }

    private async Task LoadChildCommentsRecursiveAsync(Comment parentComment, CancellationToken token)
    {
        var childComments = await context.Comments
            .AsNoTracking()
            .Where(c => c.ParentId == parentComment.Id)
            .Include(c => c.ChildComments)
            .Include(c => c.User)
            .AsSplitQuery()
            .ToListAsync(token);

        parentComment.ChildComments = childComments;

        // go deeper
        foreach (var child in childComments)
        {
            await LoadChildCommentsRecursiveAsync(child, token);
        }
    }
}