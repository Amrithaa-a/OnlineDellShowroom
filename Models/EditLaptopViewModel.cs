using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineDellShowroom.Models
{
    public class EditLaptopViewModel
    {
        public Laptop ExistingLaptop { get; set; } 
        public HttpPostedFileBase NewImageFile { get; set; }
        public int LaptopId { get; set; }

        [Required(ErrorMessage = "Laptop Name is required.")]
        public string LaptopName { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        public string ExistingImageUrl { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        public decimal Price { get; set; }
    }

}