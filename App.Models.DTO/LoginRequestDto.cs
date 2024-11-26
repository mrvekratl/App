using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Models.DTO
{
    public class LoginRequestDto
    {
        public string Email { get; set; }      // Kullanıcının e-posta adresi
        public string Password { get; set; }   // Kullanıcının şifresi
    }

}
