using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OnlineDellShowroom.Models
{
    public class Laptop
    {
        public int LaptopId { get; set; }

        [Required(ErrorMessage = "Laptop Name is required.")]
        public string LaptopName { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please select an image.")]
        public HttpPostedFileBase ImageFile { get; set; }

        public string ImageUrl { get; set; }
        

        [Required(ErrorMessage = "Price is required.")]
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

}