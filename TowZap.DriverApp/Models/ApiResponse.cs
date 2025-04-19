using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowZap.DriverApp.Models
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }  // NEW: support multiple errors
        public T? Data { get; set; }

        public static ApiResponse<T> Success(T data) =>
            new ApiResponse<T> { IsSuccess = true, Data = data };

        public static ApiResponse<T> Fail(string message, List<string>? errors = null) =>
            new ApiResponse<T> { IsSuccess = false, Message = message, Errors = errors };
    }

}
