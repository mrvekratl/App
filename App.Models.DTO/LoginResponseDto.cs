using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Models.DTO
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
