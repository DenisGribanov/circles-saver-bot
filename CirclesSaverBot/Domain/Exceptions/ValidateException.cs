namespace Domain.Exceptions
{
    public class ValidateException : Exception
    {
        public ValidateException()
        {
        }

        public ValidateException(string? message) : base(message)
        {
        }
    }
}