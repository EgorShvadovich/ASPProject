namespace ASPProject.Services.Validations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidationRules : Attribute
    {
        public ValidationRule[] Rules { get; init; }
        public ValidationRules( params ValidationRule[] rules) 
        {
            Rules = rules;
        }

    }
}
