using System;
using System.ComponentModel.DataAnnotations;

namespace FPTBook.Models
{
    public class Cart
    {
        public int ID { get; set; }
        public Book Book { get; set; }
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }
    }
}

