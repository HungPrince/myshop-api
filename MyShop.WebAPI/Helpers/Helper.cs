using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyShop.WebAPI.Helpers
{
    public static class Helper
    {
        public static string RandomString()
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            return rand.Next().ToString();
        }
    }
}