using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TMDT.Models
{
    public enum StatusEnum
    {
        WAIT, CONFIRMED, TRANSPORTED, DELIVERED, CANCLED
    }

    public class Order
    {
        [Key]
        public int OrderID { get; set; }


        public string UserId { get; set; }

        [Display(Name = "Khách hàng")]
        public string NameRec { get; set; }

        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "Địa chỉ là bắt buộc !")]
        public string AddressOrder { get; set; }

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Số điện thoại là bắt buộc là bắt buộc !")]
        public string PhoneOrder { get; set; }

        [Display(Name = "Ngày order")]
        [Required(ErrorMessage = "Ngày order là bắt buộc !")]
        public DateTime OrderDate { get; set; }


        [Display(Name = "Trạng thái")]
        public StatusEnum Status { get; set; }

        [Display(Name = "Hình thức thanh toán")]
        public string Payment { get; set; }

        public ICollection<OrderDetail> OrderDetail { get; set; }

    }
}