using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Security.Cryptography;
using System.Text;
using System.IO;

public class SaveData 
{
    static public void SavePlayerData<ClassType>(ClassType instance, string path, string aesIV, string aesKey)
    {
        string jsonData = JsonUtility.ToJson(instance);//セーブしたいクラスをJsonに変換

        string cipher = Encrypt(jsonData, aesIV, aesKey);//暗号化

        using (StreamWriter writer = new StreamWriter(path, false, Encoding.GetEncoding("utf-8")))
        {
            writer.WriteLine(cipher);//書き込む
        }
    }


    static public void LoadPlayerData<ClassType>(ref ClassType instance, string path, string aesIV, string aesKey)
    {
        if (!File.Exists(path))//ファイルが存在しないなら早期リターン
        {
            Debug.Log("ファイルは存在しません");
            return;
        }

        string cipher;
        using (StreamReader sr = new StreamReader(path, Encoding.GetEncoding("utf-8")))
        {
            cipher = sr.ReadToEnd();//暗号文の読み込み
        }

        string plain = Decrypt(cipher, aesIV, aesKey);//復号

        try
        {
            instance = JsonUtility.FromJson<ClassType>(plain);//ref したクラスのインスタンスに情報を格納する
        }
        catch
        {
            Debug.Log("不正なセーブデータ、あるいはバグです");
        }


        //Debug.Log(playerData);
    }




    //==================================================================================
    //以下、https://qiita.com/kz-rv04/items/62a56bd4cd149e36ca70 のスクリプトを使用
    //==================================================================================

    /// <summary>
    /// 対称鍵暗号を使って文字列を暗号化する
    /// </summary>
    /// <param name="text">暗号化する文字列</param>
    /// <param name="iv">対称アルゴリズムの初期ベクター</param>
    /// <param name="key">対称アルゴリズムの共有鍵</param>
    /// <returns>暗号化された文字列</returns>
    public static string Encrypt(string text, string iv, string key)
    {

        using (RijndaelManaged rijndael = new RijndaelManaged())
        {
            rijndael.BlockSize = 128;
            rijndael.KeySize = 128;
            rijndael.Mode = CipherMode.CBC;
            rijndael.Padding = PaddingMode.PKCS7;

            rijndael.IV = Encoding.UTF8.GetBytes(iv);
            rijndael.Key = Encoding.UTF8.GetBytes(key);

            ICryptoTransform encryptor = rijndael.CreateEncryptor(rijndael.Key, rijndael.IV);

            byte[] encrypted;
            using (MemoryStream mStream = new MemoryStream())
            {
                using (CryptoStream ctStream = new CryptoStream(mStream, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(ctStream))
                    {
                        sw.Write(text);
                    }
                    encrypted = mStream.ToArray();
                }
            }
            return (System.Convert.ToBase64String(encrypted));
        }
    }

    /// <summary>
    /// 対称鍵暗号を使って暗号文を復号する
    /// </summary>
    /// <param name="cipher">暗号化された文字列</param>
    /// <param name="iv">対称アルゴリズムの初期ベクター</param>
    /// <param name="key">対称アルゴリズムの共有鍵</param>
    /// <returns>復号された文字列</returns>
    public static string Decrypt(string cipher, string iv, string key)
    {
        using (RijndaelManaged rijndael = new RijndaelManaged())
        {
            rijndael.BlockSize = 128;
            rijndael.KeySize = 128;
            rijndael.Mode = CipherMode.CBC;
            rijndael.Padding = PaddingMode.PKCS7;

            rijndael.IV = Encoding.UTF8.GetBytes(iv);
            rijndael.Key = Encoding.UTF8.GetBytes(key);

            ICryptoTransform decryptor = rijndael.CreateDecryptor(rijndael.Key, rijndael.IV);

            string plain = string.Empty;
            using (MemoryStream mStream = new MemoryStream(System.Convert.FromBase64String(cipher)))
            {
                using (CryptoStream ctStream = new CryptoStream(mStream, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(ctStream))
                    {
                        plain = sr.ReadLine();
                    }
                }
            }
            return plain;
        }
    }
}
