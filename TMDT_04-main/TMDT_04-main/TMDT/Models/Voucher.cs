using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TMDT.Models
{
    public class Voucher
    {
        [Key]
        public int VoucherId { get; set; }

        [Display(Name = "Tên phiếu quà tặng")]
        public string VoucherName { get; set; }

        [Display(Name = "Giá trị")]
        public string VoucherValue { get; set; }

        [Display(Name = "Chi tiết")]
        public string VoucherDetails { get; set; }

    }
}