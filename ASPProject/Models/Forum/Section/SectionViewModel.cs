namespace ASPProject.Models.Forum.Section
{
    public class SectionViewModel
    {
        public string SectionId { get; set; } = null!;
        public Dictionary<String, String?>? ErrorMessages { get; set; }
    }
}
