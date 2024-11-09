using Authentication.Services;
using System;

namespace Authentication.Utilities
{
    //1 lớp giả tưởng về mã hóa bảo mật thông tin đơn giản
    public class SimpleHash : BaseDAO
    {
        private static SimpleHash instance;
        public static SimpleHash GetInstance()
        {
            if (instance == null)
            {
                instance = new SimpleHash();
            }
            return instance;
        }
        public string Hash(string input)
        {
            int hash = 0;
            foreach (char c in input)
            {
                // Tạo hash bằng cách sử dụng phép toán bitwise và cộng dồn
                hash = (hash << 5) - hash + c;
            }
            return Math.Abs(hash).ToString("X"); // Chuyển sang chuỗi hex
        }
        public bool CheckHash(string input, string savedHash)
        {
            // Mã hóa chuỗi người dùng nhập vào và so sánh với hash đã lưu
            string inputHash = Hash(input);
            return CompareString(inputHash, savedHash);
        }
    }
}