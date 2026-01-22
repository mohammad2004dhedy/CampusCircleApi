namespace CampusCircleApi.Dtos;
public class CommentDto
{
    public int Id{set;get;}
    public int PostId{set;get;}
    public string Content{get;set;}
    public DateTime CommentDate{set;get;}
    public UserPostCommentDto User{set;get;}
}