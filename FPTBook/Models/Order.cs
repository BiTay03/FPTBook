using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace FPTBook.Models
{
    
    public class Order
	{
        
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public double Price { get; set; }
        
        public int BookId { get; set; }
        public virtual Book? Book { get; set; }
        public List<OrderItem> OrderItem { get; set; } = new();
    }
}

