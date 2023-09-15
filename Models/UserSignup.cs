using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineDellShowroom.Models
{
    public class UserSignup
    {
        [Key]
        public int UserSignupId { get; set; }

        [DisplayName("First name")]
        [Required(ErrorMessage = "First name can't be empty")]
        public string Firstname { get; set; }

        [DisplayName("Last name")]
        [Required(ErrorMessage = "Last name can't be empty")]
        public string Lastname { get; set; }

        [DisplayName("Date of birth")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Date of birth can't be empty")]
        public DateTime? Dateofbirth { get; set; }

        [DisplayName("Gender")]
        [Required(ErrorMessage = "Gender is can't be empty")]
        public string Gender { get; set; }

        [DisplayName("Mobile number")]
        [Required(ErrorMessage = "Mobile number can't be empty")]

        public string Mobilenumber { get; set; }

        [DisplayName("Email")]
        [Required(ErrorMessage = "Email is can't be empty")]
        public string Email { get; set; }

        [DisplayName("Address")]
        [Required(ErrorMessage = "Address is can't be empty")]
        public string Address { get; set; }

        [DisplayName("State")]
        [Required(ErrorMessage = "State is can't be empty")]
        public string State { get; set; }

        [DisplayName("City")]
        [Required(ErrorMessage = "City is can't be empty")]
        public string City { get; set; }
        public int StateId { get; set; }
        public List<SelectListItem> States { get; set; }
        public List<SelectListItem> Cities { get; set; }
        /// <summary>
        /// Constructor to initialize the lists
        /// </summary>
        public UserSignup()
        {
            States = new List<SelectListItem>();
            Cities = new List<SelectListItem>();
        }

        [DisplayName("Username")]
        [Required(ErrorMessage = "Username can't be empty")]
        public string Username { get; set; }

        [DisplayName("Password")]
        public string Password { get; set; }

        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }

    }
    
}