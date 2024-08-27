using Data.Helpers.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Helpers.Common
{
    public static class Constants
    {
        public static readonly String required = "(Bắt buộc)";
        public static readonly String emailNotCorrect = "(Email không đúng)";
        public static readonly String passwordWrong = "(Password không đúng)";
        public static readonly String passworNotCorrect = "(Password không đúng. Password dài 8 ký tự bao gồm 1 ký số và 1 ký tự viết hoa)";
        public static readonly String passwordOrEmailNotCorrect = "(Email hoặc mật khẩu chưa đúng)";
        public static readonly String EmailIsValid = "(Email đã tồn tại)";
        public static readonly String phoneNotCorrect = "(Số điện thoại không đúng)";
        public static readonly String existed = "(Đã tồn tại)";


        public static readonly
         List<CommonModel> listFunction =
             new List<CommonModel>{
                 new CommonModel(){ id =1,name="_Index" },
                 new CommonModel(){ id =2,name="_Insert" },
                 new CommonModel(){ id =3,name="_Edit" },
                 new CommonModel(){ id =4,name="_UpdateStatusDel" },
                 new CommonModel(){ id =5,name="_Details" },
                 new CommonModel(){ id =6,name="_Import" },
                 new CommonModel(){ id =7,name="_DownloadExcel" },
                 new CommonModel(){ id =8,name="_Export" },
             };

        public static Dictionary<string, string> functions = new Dictionary<string, string>
        {
            { "_Index", "Xem danh sách @@ChucNang@@" },
            { "_Insert", "Thêm @@ChucNang@@" },
            { "_Edit", "Cập nhật @@ChucNang@@" },
            { "_UpdateStatusDel", "Xóa và sử dụng @@ChucNang@@" },
            { "_Details", "Xem chi tiết @@ChucNang@@" },
            { "_Import", "Import danh sách @@ChucNang@@" },
            { "_DownloadExcel", "Tải file excel @@ChucNang@@" },
            { "_Export", "Export excel danh sách @@ChucNang@@" },

        };

        public static Dictionary<int, string> defaultImage = new Dictionary<int, string>
        {
            { 1, "Hệ thống" },
            { 2, "Tài khoản" },
        };
        public static readonly
        List<CommonModel> defaultImageList =
            new List<CommonModel>{
                 new CommonModel(){ id =1,name="Hệ thống" },
                 new CommonModel(){ id =2,name="Tài khoản" },

            };

        public static Dictionary<int, string> advertisement = new Dictionary<int, string>
        {
            { 1, "Trang chủ" },
            { 2, "Sản phẩm" },
            { 3, "Chi tiết sản phẩm" },
            { 4, "Banner" },


        };
        public static readonly
        List<CommonModel> advertisementList =
            new List<CommonModel>{
                 new CommonModel(){ id =1,name="Trang chủ" },
                 new CommonModel(){ id =2,name="Sản phẩm" },
                 new CommonModel(){ id =3,name="Chi tiết sản phẩm" },
                 new CommonModel(){ id =4,name="Banner" },


            };


    }
}
