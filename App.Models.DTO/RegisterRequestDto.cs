using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Models.DTO
{
    public class RegisterRequestDto
    {
        public string FirstName { get; set; }  // Kullanıcının adı
        public string LastName { get; set; }   // Kullanıcının soyadı
        public string Email { get; set; }      // Kullanıcının e-posta adresi
        public string Password { get; set; }   // Kullanıcının şifresi
        public string ConfirmPassword { get; set; } // Şifre doğrulama (isteğe bağlı)
    }

}
