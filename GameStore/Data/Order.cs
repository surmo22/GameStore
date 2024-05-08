﻿using System.ComponentModel.DataAnnotations;
using GameStore.Data.Cart;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GameStore.Data
{
    public class Order
    {
        [BindNever]
        public int OrderId { get; set; }

        [BindNever]
        public ICollection<CartItem> Lines { get; set; } = new List<CartItem>();

        [Length(12, 17, ErrorMessage = "Not valid card number"), Required(ErrorMessage ="Enter credit card")]
        public string? CardNumber { get; set; }

        [Required(ErrorMessage = "Please enter a name")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Please enter the first address line")]
        public string? Line1 { get; set; }

        public string? Line2 { get; set; }

        public string? Line3 { get; set; }

        [Required(ErrorMessage = "Please enter a city name")]
        public string? City { get; set; }

        [Required(ErrorMessage = "Please enter a state name")]
        public string? State { get; set; }

        public string? Zip { get; set; }

        [Required(ErrorMessage = "Please enter a country name")]
        public string? Country { get; set; }
    }
}