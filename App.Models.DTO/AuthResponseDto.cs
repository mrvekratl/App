using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Models.DTO
{
    public class AuthResponseDto
    {
        public string JwtToken { get; set; }  // Kullanıcıya verilen JWT token
        public string FirstName { get; set; } // Kullanıcının adı
        public string LastName { get; set; }  // Kullanıcının soyadı
        public string Email { get; set; }     // Kullanıcının e-posta adresi
        public bool IsAdmin { get; set; }     // Kullanıcının admin olup olmadığı
    }
}
