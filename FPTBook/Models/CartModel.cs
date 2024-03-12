using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace FPTBook.Models
{
    
    public class CartModel
	{
        
        public int Id { get; set; }
		public int Quantity { get; set; }
	}
}

