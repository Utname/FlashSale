//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Data.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.ImageProducts = new HashSet<ImageProduct>();
        }
    
        public System.Guid ID { get; set; }
        public string Name { get; set; }
        public Nullable<int> idType { get; set; }
        public Nullable<int> idGroup { get; set; }
        public Nullable<decimal> StartingPrice { get; set; }
        public Nullable<decimal> EndingPrice { get; set; }
        public Nullable<int> DiscountPercentage { get; set; }
        public Nullable<decimal> DiscountFrom { get; set; }
        public Nullable<decimal> DiscountUpTo { get; set; }
        public Nullable<int> idPolicy { get; set; }
        public Nullable<int> ShippingMethod { get; set; }
        public Nullable<decimal> ShippingFee { get; set; }
        public Nullable<double> AverageRating { get; set; }
        public Nullable<int> NumberOfReviews { get; set; }
        public Nullable<int> NumberOfLikes { get; set; }
        public string Description { get; set; }
        public Nullable<long> Quantity { get; set; }
        public Nullable<long> RemainingQuantity { get; set; }
        public string SendFrom { get; set; }
        public Nullable<int> idProductCategory { get; set; }
        public string idProductClassification { get; set; }
        public Nullable<int> idReturnAndExchangePolicy { get; set; }
        public Nullable<int> idWanrranty { get; set; }
        public Nullable<System.Guid> idShop { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public Nullable<int> StatusDel { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string CreateBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string UpdateBy { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ImageProduct> ImageProducts { get; set; }
        public virtual ReturnAndExchangePolicy ReturnAndExchangePolicy { get; set; }
        public virtual TaiKhoanShop TaiKhoanShop { get; set; }
        public virtual Warranty Warranty { get; set; }
        public virtual NhomSanPham NhomSanPham { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }
    }
}
