namespace EmpManage.DTOs
{
    public interface IResponseDTO
    {
        bool IsSuccess { get; }
        object? Message { get; set; }
        int StatusCode { get; set; }
    }

    public class ResponseDTO<T> : IResponseDTO
    {
        public int StatusCode { get; set; }
        public object? Message { get; set; }
        public T? Data { get; set; }
        public bool IsSuccess => StatusCode >= 200 && StatusCode < 300;

        public ResponseDTO(int statusCode, object? message, T? data)
        {
            StatusCode = statusCode;
            Message = message;
            Data = data;
        }
        public ResponseDTO() { }
    }

}
