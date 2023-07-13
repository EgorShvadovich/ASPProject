using ASPProject.Models.Forum.Index;

namespace ASPProject.Models.Forum.Section
{
    public class SectionPageModel
    {
        public ForumSectionViewModel Section { get; set; } = null!;
        public Dictionary<String, String?>? ErrorMessages { get; set; }
        public List<TopicViewModel> Topics { get; set; } = null!;
    }
}
