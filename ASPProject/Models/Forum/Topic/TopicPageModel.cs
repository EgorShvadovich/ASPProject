using ASPProject.Models.Forum.Section;

namespace ASPProject.Models.Forum.Topic
{
    public class TopicPageModel
    {
        public TopicViewModel Topic { get; set; } = null!;
        public List<ThemeViewModel> Theme { get; set; } = null!;
        public ThemeFormModel? FormModel { get; set; }
        public Dictionary<String, String?>? ErrorMessages { get;set;}
    }
}
