using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace FPTBook.Models
{
    
    public class OrderItem
    {
        
        public int Id { get; set; }

        public int Quantity { get; set; }
        public double Price { get; set; }
        public int? OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public int? BookId { get; set; }

        public Order? Order { get; set; }
        public Book? Book { get; set; }

        // một order có nhiều Orderitem, => Một book có nhiều OrderItem
    }
}

