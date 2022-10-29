using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nkv.GetAutomator.Data.Models
{
    [Table("cart_details")] 
    public class CartDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartDetailID { get; set; }
        public int CartID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public int PriceTypeID { get; set; }
        public DateTime? CreatedOn { get; set; }
        [NotMapped]
        public virtual Product Product { get; set; }
        [NotMapped]
        public virtual  ProductPrice Price { get; set; }

    }
    [Table("cart_user_mapping")]
    public class CartUserMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartID { get; set; }
        public int UserID { get; set; }
        public int StatusID { get; set; }
        public int CouponID { get; set; }
        public double TotalInvoice { get; set; }
        public double TotalPaid { get; set; }
        public string? PaymentDetails { get; set; }
        public string? Remarks { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? PaidOn { get; set; }
        [NotMapped]
        public virtual List<CartDetails> CartDetails { get; set; }
        [NotMapped]
        public virtual Users? User { get; set; }
        [NotMapped]
        public virtual DateTime LastAccessed { get; set; }
        [NotMapped]
        public virtual int TimeToLiveInSeconds { get; set; } = 48 * 60 * 100;
        public CartUserMapping()
        {
            CartDetails = new List<CartDetails>();
        }
    }
}
