﻿using ASPProject.Entities;

namespace ASPProject.Models.Forum.Index
{
    public class ForumSectionViewModel
    {
        public String   Id          { get; set; } = null!;
        public String   Title       { get; set; } = null!;
        public String   Description { get; set; } = null!;
        public String?  ImageUrl    { get; set; }
        public String   CreateDt    { get; set; } = null!;
        public UserViewModel Author { get; set; } = null!;
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public Rate? UserRate { get; set; }
    }
}
