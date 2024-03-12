using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace FPTBook.Models
{
    
    public class Book
	{
        public int Id { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Author")]
        public string Author { get; set; }

        [Display(Name = "Price")]
        public double Price { get; set; }

        [Display(Name = "Page")]
        public int Page { get; set; }

        [Display(Name = "Publisher")]
        public string? Publisher { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? PublishDate { get; set; }

        [Display(Name = "Language")]
        public string Language { get; set; }

        [Display(Name = "Image")]
        public string Image { get; set; }

        [Display(Name = "Category")]
        public int CategoryID { get; set; }
        public virtual Category? Category { get; set; }
        public virtual ICollection<OrderItem>? OrderItems { get; set; }
    }
}

