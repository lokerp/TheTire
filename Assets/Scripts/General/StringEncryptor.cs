using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public static class StringEncryptor
{
    private static readonly byte[] _key = { 241, 80, 102, 17, 237, 86, 203, 234, 137, 71, 229, 15, 120, 197, 89, 227, 103, 235, 95, 13, 123, 12, 45, 243, 64, 12, 253, 0, 169, 229, 183, 23 };
    private static readonly byte[] _IV = { 213, 126, 105, 12, 162, 67, 164, 131, 198, 98, 33, 21, 61, 188, 16, 106 };

    public static string EncryptString(string str)
    {
        if (string.IsNullOrEmpty(str))
            throw new ArgumentNullException("str is null or empty!");

        byte[] encrypted;

        using (AesCryptoServiceProvider aesAlg = new())
        {
            aesAlg.Key = _key;
            aesAlg.IV = _IV;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(str);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
        }

        return Convert.ToBase64String(encrypted);
    }

    public static string DecryptString(string str)
    {
        if (string.IsNullOrEmpty(str))
            throw new ArgumentNullException("str is null or empty!");

        byte[] strInBytes = Convert.FromBase64String(str);
        string res = null;

        using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
        {
            aesAlg.Key = _key;
            aesAlg.IV = _IV;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(strInBytes))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        res = srDecrypt.ReadToEnd();
                    }
                }
            }
        }

        return res;
    }
}
