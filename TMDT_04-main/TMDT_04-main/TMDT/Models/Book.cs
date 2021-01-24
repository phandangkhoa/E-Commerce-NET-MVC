using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace TMDT.Models
{
    public class Book
    {
        public Book()
        {
            Image = "~/Content/Images/add.png";
        }
        [Key]
        public int BookID { get; set; }
        [Display(Name = "Tên sách")]
        [Required(ErrorMessage = "Tên sách là bắt buộc")]
        public string BookName { get; set; }
        [Display(Name = "Tồn kho"), Range(0, 1000)]
        public int InStock { get; set; }

        [Display(Name = "Giá"), Range(0, 10000000)]
        public float BookPrice { get; set; }
        [Display(Name = "Mô tả")]
        public string BookDescription { get; set; }

        [Display(Name = "Ngày sản xuất")]
        public DateTime? PublisherDate { get; set; }

        [Display(Name = "Hình ảnh")]
        public string Image { get; set; }
        [NotMapped]
        public HttpPostedFileBase ImageUpLoad { get; set; }

        [Display(Name = "Tác giả")]
        //foreignKey
        [ForeignKey("Author")]
        public int AuthorID { get; set; }
        public virtual Author Author { get; set; }

        [Display(Name = "Nhà xuất bản")]

        [ForeignKey("Publisher")]
        public int PublisherID { get; set; }
        public virtual Publisher Publisher { get; set; }

        [Display(Name = "Nhà cung cấp")]

        [ForeignKey("Provider")]
        public int ProviderID { get; set; }
        public virtual Provider Provider { get; set; }

        [Display(Name = "Loại")]
        [ForeignKey("Category")]
        public int CateID { get; set; }
        public virtual Category Category { get; set; }
    }


}