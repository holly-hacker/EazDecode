using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EazDecodeLib
{
    /// <summary>
    /// Cryptographic algorithm used by Eazfuscator before 5.8. Consists of a
    /// round of AES and an xor.
    /// </summary>
    internal class Crypto2
    {
        private readonly SymmetricAlgorithm _symmetricAlgorithm;

        public Crypto2(string password)
        {
            _symmetricAlgorithm = new RijndaelManaged
            {
                KeySize = 256,
                BlockSize = 128,
                IV = new Rfc2898DeriveBytes(password, new byte[]
                {
                    0x1C, 0x88, 0x1B, 0xD8, 0x53, 0x93, 0x8C, 0xCF,
                    0x3C, 0x99, 0x29, 0x6B, 0x75, 0xA4, 0x25, 0x9D,
                    0x5E, 0xE9, 0x33, 0x30, 0x92, 0x6C, 0x7F, 0xBF,
                    0x1E, 0xE2, 0xFA, 0x58, 0x6D, 0x07, 0x84, 0x0F
                }).GetBytes(16),
                Key = new Rfc2898DeriveBytes(password, new byte[]
                {
                    0xA7, 0x7E, 0x70, 0x10, 0x04, 0xF4, 0x0F, 0x78,
                    0x87, 0x74, 0x7B, 0xD4, 0x9D, 0x30, 0x05, 0xC2,
                    0x0C, 0xB3, 0x99, 0xC9, 0xCC, 0xF9, 0xF8, 0xD4,
                    0x56, 0x14, 0xD7, 0x37, 0x69, 0x9D, 0x6F, 0x0B
                }).GetBytes(32)
            };
        }

        internal string Decrypt(byte[] toDecrypt)
        {
            //decrypt with AES
            using (var ms = new MemoryStream()) {
                using (CryptoStream cs = new CryptoStream(ms, _symmetricAlgorithm.CreateDecryptor(), CryptoStreamMode.Write))
                    cs.Write(toDecrypt, 0, toDecrypt.Length);

                //decrypt xor
                toDecrypt = CryptoHelper.XorArray(ms.ToArray());
                return Encoding.UTF8.GetString(toDecrypt);
            }
        }
    }
}
