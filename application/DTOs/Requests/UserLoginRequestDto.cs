using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace application.DTOs.Requests
{
    public class UserLoginRequestDto
    {
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        public string VerificationCode { get; set; }

    }
}
