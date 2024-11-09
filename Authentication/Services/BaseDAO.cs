using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Authentication.Services
{
    public abstract class BaseDAO
    {
        protected string Lower(string value)
        {
            return value = value.ToLowerInvariant();
        }       
        protected string GenerateUserCODE()
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