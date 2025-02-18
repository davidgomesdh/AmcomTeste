namespace Questao5.Application
{
    public class ResponseBase
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorType { get; set; }
    }
}
