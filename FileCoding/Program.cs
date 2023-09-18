using System;
using System.Globalization;
using System.IO;
using System.Threading;

namespace FileCoding
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                byte[] Key;
                byte[] Bytes = new byte[16];
                int Length = 0;
                FileStream AES128_File;
                FileStream AES128_Key;
                byte[] Bytes_Decryot;
                byte[] Bytes_Encrypt;
                FileCoding.AES AES128 = new FileCoding.AES();

                if (!File.Exists(args[0]))
                {
                    Console.WriteLine("FileCoding : Error the file does not exist \"" + args[0] + "\" .");
                    return;
                }
                else
                {
                    AES128_File = new FileStream(args[0], FileMode.Open);
                    Length = (int)AES128_File.Length;
                    if((Length%16) != 0)
                    {
                        Length += (16 - (Length % 16));
                    }
                    Bytes_Decryot = new byte[Length];
                    Array.Clear(Bytes_Decryot, 0, Length);
                    Bytes_Encrypt = new byte[Length];
                    Array.Clear(Bytes_Encrypt, 0, Length);
                    AES128_File.Read(Bytes_Decryot, 0, (int)AES128_File.Length);
                    AES128_File.Close();
                }

                
                if(args[1].Length != 32)
                {
                    Console.WriteLine("FileCoding : Error the key property does not exist .");
                    return;
                }
                else
                {
                    Key = ConvertHexStringToByteArray(args[1]); 
                }

                File.Delete(args[0]);

                AES128_File = new FileStream(args[0], FileMode.OpenOrCreate);
                for (int Index=0; Index< Length; )
                {
                    
                    Buffer.BlockCopy(Bytes_Decryot, Index, Bytes, 0, 16);
                    AES128.AES128_ECB_encrypt(Bytes, Key, Bytes);
                    Buffer.BlockCopy(Bytes, 0, Bytes_Encrypt, Index, 16);
                    Index += 16;
                }
                AES128_File.Write(Bytes_Encrypt, 0, Length);
                AES128_File.Flush();
                Thread.Sleep(5000);
                AES128_File.Close();

                Console.WriteLine("FileCoding : Success .");

            }
            catch (Exception er)
            {
                Console.WriteLine("FileCoding : " + er.Message.ToString());
            }
        }

        public static byte[] ConvertHexStringToByteArray(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", hexString));
            }

            byte[] data = new byte[hexString.Length / 2];
            for (int index = 0; index < data.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                data[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return data;
        }
    }
}
