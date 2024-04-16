using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class CryotoModule
{
    private static readonly string _PASSWORD = "GGMisGreatestOfAllTime1234abcdefgqwnjebaslwqe12"; //최소 32글자는 되어야 해

    private byte[] _saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
    public string AESEncrypt256(string plainText)
    {
        try
        {
            //암호화할 내용을 넣는다.
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(_PASSWORD, _saltBytes);
            Aes myAES = Aes.Create();
            myAES.Mode = CipherMode.CBC;
            myAES.Padding = PaddingMode.PKCS7;
            myAES.KeySize = 256;
            myAES.BlockSize = 128;

            myAES.Key = key.GetBytes(myAES.KeySize / 8);
            myAES.IV = key.GetBytes(myAES.BlockSize / 8);

            MemoryStream memoryStream = new MemoryStream();

            //인크립터를 만들어주고 해당인크립터에 작성한 키와 IV를 넣는다. 
            ICryptoTransform encryptor = myAES.CreateEncryptor(myAES.Key, myAES.IV);

            //암호화 스트림에 메모리스트림과 인크립터를 넣어주고 써준다.
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainBytes, 0, plainBytes.Length);
            cryptoStream.FlushFinalBlock();

            //쓰여진 메모리를 바이트 어레이로 받고
            byte[] encryptBytes = memoryStream.ToArray();
            //받아온 바이트 어레이를 base64로 인코딩한다.
            string encryptString = Convert.ToBase64String(encryptBytes);

            cryptoStream.Close();
            memoryStream.Close();

            return encryptString;
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }

    public string Decrypt(string cryptedText)
    {
        try
        {
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(_PASSWORD, _saltBytes);
            Aes myAES = Aes.Create();
            myAES.Mode = CipherMode.CBC;
            myAES.Padding = PaddingMode.PKCS7;
            myAES.KeySize = 256;
            myAES.BlockSize = 128;

            myAES.Key = key.GetBytes(myAES.KeySize / 8);
            myAES.IV = key.GetBytes(myAES.BlockSize / 8);

            MemoryStream memoryStream = new MemoryStream();
            ICryptoTransform decryptor = myAES.CreateDecryptor();

            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write);

            byte[] crytedBytes = Convert.FromBase64String(cryptedText); //base64디코딩해서 암호화된 원문 가져오고 
            cryptoStream.Write(crytedBytes, 0, crytedBytes.Length); //해석기를 장착해서 메모리에 해석해서 써주고
            cryptoStream.FlushFinalBlock(); //버퍼에 있는거 밀어내기

            byte[] buffer = memoryStream.ToArray();
            string Output = Encoding.UTF8.GetString(buffer); //해석된 버퍼에 있는 내용을문장으로 바꿔 리턴 
            return Output;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}