using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TMDT.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        [Display(Name = "CommentID")]
        public int BookID { get; set; }
        [Display(Name = "BookID")]
        public string UserId { get; set; }
        [Display(Name = "UserID")]
        public string Name { get; set; }
        [Display(Name = "Name")]
        public string Avatar { get; set; }
        [Display(Name = "Avatar")]
        public string Content { get; set; }
        public DateTime Time { get; set; }
        public ICollection<Book> Book { get; set; }
    }
}