using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineDellShowroom.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }
        public int LaptopId { get; set; }
        public string LaptopName { get; set; }
        public string ImageUrl { get; set; } 
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public decimal Subtotal => Price * Quantity;

    }
}