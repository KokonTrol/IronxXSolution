using Library.Models;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Library.Functions
{
    public static class PasswordHash
    {
        public static string GetHashed(string password)
        {
            MD5 MD5Hash = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(password);
            byte[] hash = MD5Hash.ComputeHash(inputBytes);
            return Convert.ToHexString(hash);
        }
        public static bool CheckPassword(string maybePassword, string password)
        {
            maybePassword = GetHashed(maybePassword);
            return password == maybePassword;
        }
        public static void SavePassword(string password)
        {
            IronContext ironContext = new IronContext();
            string pswd = GetHashed(password);
            ironContext.SetNewConfigPassword(GetHashed(password));
        }
    }
}
