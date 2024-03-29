﻿namespace ASPProject.Models.Forum.Theme
{
    public class CommentViewModel
    {
        public String Id { get; set; } = null!;
        public String Content { get; set; } = null!;
        public String CreateDt { get;set; } = null!;
        public UserViewModel Author { get; set; } = null!;

        public CommentViewModel()
        {

        }
        public CommentViewModel(Entities.Comment comment)
        {
            Id = comment.Id.ToString();
            Content = comment.Content;
            CreateDt = comment.CreateDt.ToShortDateString();
            Author = new(comment.Author);
        }
    }
}
