using System;
using System.Linq;

namespace Authentication.Services
{
    public abstract class BaseDAO
    {
        //Bỏ so sánh chuỗi bằng Lower
        protected bool CompareString(string strA, string strB)
        {
            return string.Equals(strA, strB, StringComparison.CurrentCultureIgnoreCase);
        }       
        public string GenerateUserCODE()
        {
            long ticks = DateTime.Now.Ticks;
            string result = $"{RandomString(2)}{ticks}";
            return result;
        }
        private string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}