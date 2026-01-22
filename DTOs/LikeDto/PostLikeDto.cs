namespace CampusCircleApi.Dtos
{
    public class PostLikeDto
    {
        //ارحع البوستات الي عملهم لايك
        public PostDto Post{set;get;}
        public DateTime LikedAt { get; set; }
    }
}