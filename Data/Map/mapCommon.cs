using Data.Entity;
using Data.Helpers.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Data
{
    public class mapCommon
    {


        public FlashSaleEntities db = new FlashSaleEntities();
        public  string phonePattern = @"^(\+?[0-9]{1,3})?[-. ]?(\(?[0-9]{1,4}\)?)?[-. ]?[0-9]{1,4}[-. ]?[0-9]{1,4}[-. ]?[0-9]{1,9}$";
        public  string anhMacDinh = "~/Areas/Admin/Content/FileUpload/Images/2024-7-7/design_patten.jpg";
        public  string pathFileUploadExcel = "~/Areas/Admin/Content/FileUpload/Excel/";
        public  string pathFileUpLoadImamge = "/Areas/Admin/Content/FileUpload/Images/";
        public  string pathFileUpLoadDowload = "~/Areas/Admin/Content/Templates/Excel/";
        public  string tatCa = "Tất cả";
        public  string imagePath = "~/Areas/Admin/Content/FileUpload/Images/";
      

        public List<CommonModel> GetListStatusDel()
        {
            var result = new List<CommonModel>()
            {
                new CommonModel(){id = 1,name="Sử dụng"},
                new CommonModel(){id = 2,name="Ngưng sử dụng"}
            };
            return result;
        }

        public string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }

        public bool IsValidEmail(string email)
        {
            // Regular expression for validating an Email
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            // Instantiate the regular expression object.
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            // Validate the email address.
            return regex.IsMatch(email);
        }

        public bool IsValidPassword(string password)
        {
            // Regular expression for validating a password
            string pattern = @"^(?=.*[A-Z])(?=.*\d).{8,}$";

            // Instantiate the regular expression object.
            Regex regex = new Regex(pattern);

            // Validate the password.
            return regex.IsMatch(password);
        }

        public  string encrypt(string encryptString)
        {
            string EncryptionKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
             });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptString = Convert.ToBase64String(ms.ToArray());
                }
            }
            return encryptString;
        }
        public  string GetUserId()
        {
            HttpCookie authCookie = HttpContext.Current.Request.Cookies["TaiKhoan"];
            if (authCookie == null) return null;
            string idTaiKhoan = authCookie.Values["IdTaiKhoan"].ToString();
            return idTaiKhoan;
        }

    }
}
