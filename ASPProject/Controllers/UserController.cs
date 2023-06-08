using ASPProject.Data;
using ASPProject.Models.User;
using ASPProject.Services.Hash;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace ASPProject.Controllers
{
    public class UserController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IHashService _hashService;

        public UserController(DataContext dataContext, IHashService hashService)
        {
            _dataContext = dataContext;
            _hashService = hashService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login(SignUpFormModel? formModel)
        {
            SignUpViewModel viewModel;
            if (Request.Method == "POST" && formModel != null)
            {
                // передача формы
                viewModel = ValidateSignUpForm(formModel);
                viewModel.FormModel = formModel;
                HttpContext.Session.SetString("FromData", System.Text.Json.JsonSerializer.Serialize(viewModel));
                return RedirectToAction(nameof(Login));
            }
            else
            {
                if (HttpContext.Session.Keys.Contains("FromData"))
                {
                    String? data = HttpContext.Session.GetString("FromData");
                    if(data != null)
                    {
                        viewModel = System.Text.Json.JsonSerializer.Deserialize<SignUpViewModel>(data)!;
                        HttpContext.Session.Remove("FromData");
                    }
                    else
                    {
                        viewModel = new();
                        viewModel.FormModel = null;  // нечего проверять
                    }
                    
                }
                else
                {
                    // первый заход - начало заполнения формы
                    viewModel = new();
                    viewModel.FormModel = null;  // нечего проверять
                }
               
            }
            return View(viewModel);  // передаем модель в представление
        }

        private SignUpViewModel ValidateSignUpForm(SignUpFormModel formModel)
        {
            SignUpViewModel viewModel = new();
            if (String.IsNullOrEmpty(formModel.Login))
            {
                viewModel.LoginMessage = "Логин не может быть пустым";
            }
            else if (formModel.Login.Length < 3)
            {
                viewModel.LoginMessage = "Логин слишком короткий (3 символа минимум)";
            }
            else if (_dataContext.Users.Any(u => u.Login == formModel.Login))
            {
                viewModel.LoginMessage = "Данный логин уже занят";
            }
            else
            {
                viewModel.LoginMessage = null;  // все проверки логина пройдены
            }


            if (String.IsNullOrEmpty(formModel.Password))
            {
                viewModel.PasswordMessage = "Пароль не может быть пустым";
            }
            else if (formModel.Password.Length < 3)
            {
                viewModel.PasswordMessage = "Пароль слишком короткий (3 символа минимум)";
            }
            else if (!Regex.IsMatch(formModel.Password, @"\d"))
            {
                viewModel.PasswordMessage = "Пароль должен содержать цифры";
            }
            else
            {
                viewModel.PasswordMessage = null;  // все проверки пароля пройдены
            }


            if (String.IsNullOrEmpty(formModel.RepeatPassword))
            {
                viewModel.PasswordMessage = "Вы должны подтвердить свой пароль";
            }
            else if (formModel.Password != formModel.RepeatPassword)
            {
                viewModel.PasswordMessage = "Пароли не совпадают";
            }
            else
            {
                viewModel.PasswordMessage = null;  // все проверки пароля пройдены
            }

            if(formModel.RealName != null)
            {
                if (Regex.IsMatch(formModel.RealName, @"\d|[\W_]"))
                {
                    viewModel.RealNameMessage = "Имя содержит цифры или символы";
                }
                else
                {
                    viewModel.PasswordMessage = null;  // все проверки пароля пройдены
                }
            }
            


            if (String.IsNullOrEmpty(formModel.Email))
            {
                viewModel.PasswordMessage = "E-mail не может быть пустым";
            }
            else if (_dataContext.Users.Any(u => u.Email == formModel.Email))
            {
                viewModel.PasswordMessage = "На данную почту уже есть регистрация";
            }
            else
            {
                viewModel.PasswordMessage = null;  // все проверки пароля пройдены
            }

            String nameAvatar = null!;
            // сохранение файла
            if (formModel.Avatar != null)  // файл передан
            {
                /* При приема файла важно:
                 * - проверить допустимые расширения (тип)
                 * - проверить максимальный размер
                 * - заменить имя файла
                 * Создаем папку (вручную) wwwroot/avatars/
                 *  и в нее будем сохранять файлы, расширения - оставляем
                 *  какие есть, а вместо имени - используем GUID
                 */
                if (formModel.Avatar.Length > 1048576)
                {
                    viewModel.AvatarMessage = "Файл слишком большой (макс 1 МБ)";
                }
                // определяем расширение файла
                String ext = Path.GetExtension(formModel.Avatar.FileName);
                // проверить расширение на перечень допустимых

                // формируем имя для файла
                nameAvatar = Guid.NewGuid().ToString() + ext;

                formModel.Avatar.CopyTo(
                    new FileStream("wwwroot/avatars/" + nameAvatar, FileMode.Create));
            }


            
            if(viewModel.LoginMessage == null &&
                viewModel.PasswordMessage == null &&
                viewModel.AvatarMessage == null)
            {
                _dataContext.Users.Add(new()
                {
                    Id = Guid.NewGuid(),
                    Login = formModel.Login,
                    PasswordHash = _hashService.GetHash(formModel.Password),
                    Email = formModel.Email!,
                    CreatedDt = DateTime.Now,
                    Name = formModel.RealName!,
                    Avatar = nameAvatar
                });
                _dataContext.SaveChanges();
            }
            return viewModel;
        }
    }
}
