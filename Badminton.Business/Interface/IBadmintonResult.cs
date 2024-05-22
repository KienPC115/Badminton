using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Badminton.Business.Interface {
    public interface IBadmintonResult {
        
            int Status { get; set; }
            string? Message { get; set; }
            object? Data { get; set; }
    }

    public class BadmintonResult : IBadmintonResult {
        public int Status { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }

        public BadmintonResult() {
            Status = -1;
            Message = "Action fail";
        }

        public BadmintonResult(int status, string message) {
            Status = status;
            Message = message;
        }

        public BadmintonResult(int status, string message, object data) {
            Status = status;
            Message = message;
            Data = data;
        }
    }
}
