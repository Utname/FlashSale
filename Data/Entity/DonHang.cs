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
    
    public partial class DonHang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DonHang()
        {
            this.DonHangChiTiets = new HashSet<DonHangChiTiet>();
        }
    
        public System.Guid ID { get; set; }
        public Nullable<System.Guid> idShop { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChiGiaoHang { get; set; }
        public Nullable<System.DateTime> ThoiGianDatHang { get; set; }
        public Nullable<int> TrangThai { get; set; }
        public Nullable<decimal> TongTienSanPham { get; set; }
        public Nullable<decimal> TrietKhau { get; set; }
        public Nullable<decimal> PhiShip { get; set; }
        public Nullable<decimal> Vat { get; set; }
        public Nullable<decimal> TongThanhToan { get; set; }
        public Nullable<System.DateTime> NgayCapNhat { get; set; }
        public string NguoiCapNhat { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public Nullable<int> StatusDel { get; set; }
        public string KhacHangGhiChu { get; set; }
        public Nullable<int> idVat { get; set; }
    
        public virtual TaiKhoanShop TaiKhoanShop { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DonHangChiTiet> DonHangChiTiets { get; set; }
    }
}
