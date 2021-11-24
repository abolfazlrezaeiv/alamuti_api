using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace application.DTOs.Requests
{
    public class UserRegistrationDto
    {
        [Required]
        public string PhoneNumber { get; set; }
     
     
    }
}
