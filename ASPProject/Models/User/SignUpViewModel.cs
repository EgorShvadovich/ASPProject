namespace ASPProject.Models.User
{
    public class SignUpViewModel
    {
        public string? LoginMessage { get; set; }
        public string? PasswordMessage { get; set; }
        public string? RepeatMessage { get; set; }
        public string? RealNameMessage { get; set; }
        public string? EmailMessage { get; set; }
        public string? ConfirmMessage { get; set; }
        public string? AvatarMessage { get; set; }

        public SignUpFormModel? FormModel { get; set; }

    }
}

/*
  Модель представления - данные для участия
в отображении
Классы-модели более предпочтительны, чем множество параметров
ViewBag или ViewData
 
 */