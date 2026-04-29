using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace _1292886_MVC_CRUD_Project.Helpers
{
    public class PasswordHelper
    {
        public static void CreateHash(string password, out string hash, out string salt)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] saltBytes = new byte[16];
                rng.GetBytes(saltBytes);

                using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100000))
                {
                    byte[] hashBytes = pbkdf2.GetBytes(32);

                    salt = Convert.ToBase64String(saltBytes);
                    hash = Convert.ToBase64String(hashBytes);
                }
            }
        }

        public static bool Verify(string password, string storedHash, string storedSalt)
        {
            byte[] saltBytes = Convert.FromBase64String(storedSalt);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100000))
            {
                byte[] hashBytes = pbkdf2.GetBytes(32);
                return Convert.ToBase64String(hashBytes) == storedHash;
            }
        }
    }
}