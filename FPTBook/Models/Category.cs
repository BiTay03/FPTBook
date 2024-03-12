using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace FPTBook.Models
{
    
    public class Category
	{
        public int Id { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        public virtual ICollection<Book>? Book { get; set; }
    }
}

