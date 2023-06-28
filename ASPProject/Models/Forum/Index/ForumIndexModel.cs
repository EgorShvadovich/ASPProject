namespace ASPProject.Models.Forum.Index
{
    public class ForumIndexModel
    {
        public IEnumerable<ForumSectionViewModel> Sections { get; set; } = null!;
        public Dictionary<String,String?>? ErrorValidationMesages { get; set; }
        public ForumSectionFormModel? FormModel { get; set; }
    }
}
