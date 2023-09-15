using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace OnlineDellShowroom.Models
{
    public class OrderView
    {
        public int OrderId { get; set; }
        [DisplayName("Username")]
        public string Username { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public int? Pincode { get; set; }
        public DateTime OrderDate { get; set; }
        public string ProductName { get; set; }
        public string ProductImageUrl { get; set; }
        public decimal ProductPrice { get; set; }
        public string OrderStatus { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal SelectedTotalPrice { get; set; }
        public List<int> SelectedLaptopIds { get; set; }
        // This property should hold the selected items in the cart
        public List<CartItem> SelectedItems { get; set; }

        // Payment-related fields
        [DisplayName("Credit Card Number")]
        public string CreditCardNumber { get; set; }

        [DisplayName("Expiration Month")]
        public int? ExpirationMonth { get; set; }

        [DisplayName("Expiration Year")]
        public int? ExpirationYear { get; set; }

        [DisplayName("CVV")]
        public string CVV { get; set; }


    }
}