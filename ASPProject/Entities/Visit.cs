namespace ASPProject.Entities
{
    public class Visit
    {
        public Guid     Id      { get; set; }
        public Guid     ItemId  { get; set; }
        public Guid     Userid  { get; set; }
        public DateTime Moment  { get; set; }
    }
}
