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
    
    public partial class ShopNhomSanPham
    {
        public System.Guid idShop { get; set; }
        public int idNhomSanPham { get; set; }
        public string GhiChu { get; set; }
        public Nullable<int> StatusDel { get; set; }
        public Nullable<System.DateTime> NgayCapNhat { get; set; }
        public string NguoiCapNhat { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public System.Guid ID { get; set; }
    
        public virtual TaiKhoanShop TaiKhoanShop { get; set; }
    }
}
