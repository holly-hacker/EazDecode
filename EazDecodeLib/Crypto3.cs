using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using EazDecodeLib.Crypto3Algorithms;

namespace EazDecodeLib
{
    /// <summary>
    /// Encryption used in Eazfuscator 5.8+. Designed to produce compact 
    /// output.
    /// </summary>
    internal class Crypto3
    {
        private const string base64Salt = "dZ58E5Xa0RKqscx+HA3eLBcOcAExpKXCkF9MODmm1wVk8NynKuzorgv8y50USvuaLvlpLbwJWb9hQQSGoZx9kw==";
        private static byte[] salt = Convert.FromBase64String(base64Salt);

        private SymmetricAlgorithm[] _algos = new SymmetricAlgorithm[5];
        private SymmetricAlgorithm _padder;
        private KeyedHashAlgorithm _kha;
        private HashAlgorithm _homebrewHasher;

        private readonly SymbolDecompressor _sc;

        public Crypto3(string password)
        {
            //create SymbolDecompressor
            _sc = new SymbolDecompressor();

            //get a random provider
            var deriveBytes = new Rfc2898DeriveBytes(password, salt, 3000);

            //fill our list of algorithms
            for (int i = 0; i < _algos.Length; i++)
            {
                var algo = new SymAlgoLengthOptimized(new SymmetricAlgorithm[]
                {
                    new RijndaelManaged(),  //blocksize prob 128 or 256
                    new SymAlgoBlowfish(),  //blocksize 64 (8b)
                    new SymAlgoHomebrew(),  //blocksize 32 (4b)
                });

                algo.Key = deriveBytes.GetBytes(algo.KeySize / 8);
                algo.IV = deriveBytes.GetBytes(algo.IVSize / 8);
                _algos[i] = algo;
            }

            //create padder algorithm
            _padder = new SymAlgoPadder(new RijndaelManaged()) {Key = deriveBytes.GetBytes(32)};

            //create kha
            _kha = new KeyedHashAlgo(new HashAlgoPadder(new HashAlgoEncryption(new HashAlgoHomebrew(), new SymAlgoHomebrew(new byte[] {
                0xA3, 0x73, 0xF3, 0x68,
                0xA0, 0x4A, 0x89, 0xE9,
                0x92, 0xEC
            }))), deriveBytes.GetBytes(2));

            //create another hasher
            //this is not used?
            _homebrewHasher = new HashAlgoEncryption(new HashAlgoHomebrew(), new SymAlgoHomebrew(new byte[]
            {
                0xEA, 0x5F, 0x88, 0xF2,
                0xA2, 0x9C, 0x0F, 0xA9,
                0x70, 0x9E
            }));
        }

        public string Decrypt(byte[] enc)
        {
            enc = TrimChecksumByte(enc);
            enc = RunThroughAlgoArray(enc, false);
            enc = Depad(enc);
            enc = CryptoHelper.RemoveTrailingNullBytes(enc);
            enc = CryptoHelper.XorArray(enc);
            return _sc.Decompress(enc);
        }

        /// <summary>
        /// Mostly used for verification. Trims the last byte and uses it as a 
        /// checksum.
        /// </summary>
        /// <param name="input">Input byte[], including the checksum byte at 
        /// the end</param>
        /// <returns>Input without checksum byte</returns>
        private byte[] TrimChecksumByte(byte[] input)
        {
            if (input.Length == 0) return input;
            
            int newLen = input.Length - 1;
            lock (_kha) {
                byte hashFirstChar = _kha.ComputeHash(input, 0, newLen)[0];
                byte encLastChar = input[input.Length - 1];
                Debug.Assert(hashFirstChar == encLastChar);
            }

            byte[] ret = new byte[newLen];
            Buffer.BlockCopy(input, 0, ret, 0, newLen);
            return ret;
        }

        /// <summary>
        /// Uses the <seealso cref="_algos"/> array to encrypt and decrypt a 
        /// bunch of times.
        /// </summary>
        /// <param name="buffer">Bytes to process</param>
        /// <param name="encrypt">Whether to encrypt or decrypt</param>
        /// <returns>Encrypted or decrypted bytes, depending on <paramref name="encrypt"/></returns>
        private byte[] RunThroughAlgoArray(byte[] buffer, bool encrypt)
        {
            if (encrypt) {
                for (int i = 0; i < _algos.Length; i++)
                {
                    using (ICryptoTransform t = encrypt ? _algos[i].CreateEncryptor() : _algos[i].CreateDecryptor())
                        buffer = t.TransformFinalBlock(buffer, 0, buffer.Length);

                    encrypt = !encrypt;
                }
            } else {
                for (int i = _algos.Length - 1; i >= 0; i--)
                {
                    using (ICryptoTransform t = encrypt ? _algos[i].CreateEncryptor() : _algos[i].CreateDecryptor())
                        buffer = t.TransformFinalBlock(buffer, 0, buffer.Length);

                    encrypt = !encrypt;
                }
            }

            return buffer;
        }

        /// <summary>
        /// Uses the <seealso cref="_padder"/>'s decryptor to decrypt the 
        /// padding bytes.
        /// </summary>
        /// <param name="enc">Bytes to decrypt</param>
        /// <returns>The decrypted bytes</returns>
        private byte[] Depad(byte[] enc)
        {
            using (var memoryStream = new MemoryStream()) {
                using (var cryptoStream = new CryptoStream(memoryStream, _padder.CreateDecryptor(), CryptoStreamMode.Write)) {
                    cryptoStream.Write(enc, 0, enc.Length);
                }
                return memoryStream.ToArray();
            }
        }
    }
}
