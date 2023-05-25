namespace ASPProject.Services
{
    public class Validation
    {
        public bool ValidateNonEmpty(string input)
        {
            if (input == null || input.Trim().Length == 0)
            {
                return false;
            }
            return true;
        }

        public bool ValidateLogin(string login)
        {
            if (string.IsNullOrEmpty(login))
            {
                return false;
            }

            foreach (char c in login)
            {
                if (!IsValidLoginCharacter(c))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsValidLoginCharacter(char c)
        {
            return Char.IsLetterOrDigit(c) || c == '_';
        }
    } 
}
