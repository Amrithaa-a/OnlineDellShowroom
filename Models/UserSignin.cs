﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineDellShowroom.Models
{
    public class UserSignin
    {
        public int UserSigninId { get; set; }

        [Required(ErrorMessage = "Username can't be empty")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password can't be empty")]
        public string Password { get; set; }
    }
}