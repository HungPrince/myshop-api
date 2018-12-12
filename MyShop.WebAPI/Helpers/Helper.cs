using System;


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