using CampusCircleApi.Enums;
using CampusCircleApi.Models;
namespace CampusCircleApi.Dtos;

public class PostDto
{
    public int Id{get;set;}
    public string Content{set;get;}
    public MediaType MediaType{set;get;}=MediaType.None;
    public string? MediaPath{set;get;}
    public College College{set;get;}//used for rooms
    public DateTime CreatedAt{set;get;}
    public UserPostCommentDto User{set;get;}
    public int LikeCount{set;get;}//taken from PostLikes.Count() when returning a post
    public int CommentCount {get;set;}
    public bool IsLikedByCurrentUser{set;get;}
}