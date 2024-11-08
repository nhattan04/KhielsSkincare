﻿using System.ComponentModel.DataAnnotations;

namespace KhielsSkincare.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Không được để trống")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Không được để trống"),EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password),Required(ErrorMessage = "Không được để trống")]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string ConfirmPassword { get; set; }
    }
}
