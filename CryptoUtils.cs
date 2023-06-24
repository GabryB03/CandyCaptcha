using System.Security.Cryptography;
using System.Text;
using System;
using System.IO;
using System.IO.Compression;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Newtonsoft.Json.Linq;
using Microsoft.VisualBasic;
using System.Linq;
using System.Net;

public class CryptoUtils
{
    private static char[] numbers = "0123456789".ToCharArray();

    public static int GetUniqueInt(int size)
    {
        try
        {
            byte[] data = new byte[4 * size];

            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(data);
            }

            StringBuilder result = new StringBuilder(size);

            for (int i = 0; i < size; i++)
            {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % numbers.Length;

                result.Append(numbers[idx]);
            }

            return int.Parse(result.ToString());
        }
        catch
        {
            return 0;
        }
    }

    public static string EncryptAES256(string input, string pass)
    {
        var AES = new RijndaelManaged();

        var hash = new byte[32];
        var temp = new MD5CryptoServiceProvider().ComputeHash(Encoding.Unicode.GetBytes(pass));

        Array.Copy(temp, 0, hash, 0, 16);
        Array.Copy(temp, 0, hash, 15, 16);

        AES.Key = hash;
        AES.Mode = CipherMode.ECB;

        var Buffer = Encoding.Unicode.GetBytes(input);

        return Convert.ToBase64String(AES.CreateEncryptor().TransformFinalBlock(Buffer, 0, Buffer.Length));
    }

    public static string GetMD5(string input)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }

            return sb.ToString();
        }
    }
}