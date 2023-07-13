namespace ASPProject.Entities
{
    public class Comment
    {
        public Guid         Id          { get; set; }
        public String       Content     { get; set; } = null!;
        public Guid         AuthorId    { get; set; }
        public Guid         ThemeId     { get; set; }
        public Guid?        ReplyId     { get; set; }
        public String?      ImageUrl    { get; set; }
        public DateTime     CreateDt    { get; set; }
        public DateTime?    DeleteDt    { get; set; }


        public User Author { get; set; } = null!;
        public Theme Theme { get; set; } = null!;
    }
}
