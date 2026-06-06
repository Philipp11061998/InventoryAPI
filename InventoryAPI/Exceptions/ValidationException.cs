namespace InventoryAPI.Exceptions
{
    public class ValidationException : Exception
    {
        public List<ValidationError> Errors { get; }

        public ValidationException(List<ValidationError> errors)
        : base("Validation failed")
        {
            Errors = errors;
        }
    }

    public class ValidationError
    {
        public string Code {get; set;}
        public string Message {get; set;}

        public ValidationError(string code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}