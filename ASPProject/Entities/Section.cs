﻿namespace ASPProject.Entities
{
    public class Section
    {
        public Guid         Id          { get; set; }
        public String       Title       { get; set; } = null!;
        public String       Description { get; set; } = null!;
        public String?      ImageUrl    { get; set; }
        public Guid         AuthorId    { get; set; }
        public DateTime     CreateDt    { get; set; }
        public DateTime?    DeleteDt    { get; set; }

        public IEnumerable<Rate> Rates { get; set; }
        public User Author { get; set; }
    }
}
