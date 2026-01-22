namespace CampusCircleApi.Dtos;
using System.ComponentModel.DataAnnotations;
public class CreateCommentDto
{
    [Range(1, int.MaxValue)]
    public int PostId{set;get;}
    [Range(1, int.MaxValue)]
    public int UserId{set;get;}
    [Required, StringLength(500, MinimumLength = 1)]
    public string Content{get;set;}
}