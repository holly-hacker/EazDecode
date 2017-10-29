using System;
using System.Security.Cryptography;

namespace EazDecodeLib.Crypto3Algorithms
{
    /// <inheritdoc />
    /// <summary>
    /// A homebrew cipher that utilizes a big key for lookups.
    /// </summary>
    internal sealed class SymAlgoHomebrew : SymmetricAlgorithm
    {
        private static readonly byte[] BigKey = {
                0xA3, 0xD7, 0x09, 0x83, 0xF8, 0x48, 0xF6, 0xF4,
                0xB3, 0x21, 0x15, 0x78, 0x99, 0xB1, 0xAF, 0xF9,
                0xE7, 0x2D, 0x4D, 0x8A, 0xCE, 0x4C, 0xCA, 0x2E,
                0x52, 0x95, 0xD9, 0x1E, 0x4E, 0x38, 0x44, 0x28,
                0x0A, 0xDF, 0x02, 0xA0, 0x17, 0xF1, 0x60, 0x68,
                0x12, 0xB7, 0x7A, 0xC3, 0xE9, 0xFA, 0x3D, 0x53,
                0x96, 0x84, 0x6B, 0xBA, 0xF2, 0x63, 0x9A, 0x19,
                0x7C, 0xAE, 0xE5, 0xF5, 0xF7, 0x16, 0x6A, 0xA2,
                0x39, 0xB6, 0x7B, 0x0F, 0xC1, 0x93, 0x81, 0x1B,
                0xEE, 0xB4, 0x1A, 0xEA, 0xD0, 0x91, 0x2F, 0xB8,
                0x55, 0xB9, 0xDA, 0x85, 0x3F, 0x41, 0xBF, 0xE0,
                0x5A, 0x58, 0x80, 0x5F, 0x66, 0x0B, 0xD8, 0x90,
                0x35, 0xD5, 0xC0, 0xA7, 0x33, 0x06, 0x65, 0x69,
                0x45, 0x00, 0x94, 0x56, 0x6D, 0x98, 0x9B, 0x76,
                0x97, 0xFC, 0xB2, 0xC2, 0xB0, 0xFE, 0xDB, 0x20,
                0xE1, 0xEB, 0xD6, 0xE4, 0xDD, 0x47, 0x4A, 0x1D,
                0x42, 0xED, 0x9E, 0x6E, 0x49, 0x3C, 0xCD, 0x43,
                0x27, 0xD2, 0x07, 0xD4, 0xDE, 0xC7, 0x67, 0x18,
                0x89, 0xCB, 0x30, 0x1F, 0x8D, 0xC6, 0x8F, 0xAA,
                0xC8, 0x74, 0xDC, 0xC9, 0x5D, 0x5C, 0x31, 0xA4,
                0x70, 0x88, 0x61, 0x2C, 0x9F, 0x0D, 0x2B, 0x87,
                0x50, 0x82, 0x54, 0x64, 0x26, 0x7D, 0x03, 0x40,
                0x34, 0x4B, 0x1C, 0x73, 0xD1, 0xC4, 0xFD, 0x3B,
                0xCC, 0xFB, 0x7F, 0xAB, 0xE6, 0x3E, 0x5B, 0xA5,
                0xAD, 0x04, 0x23, 0x9C, 0x14, 0x51, 0x22, 0xF0,
                0x29, 0x79, 0x71, 0x7E, 0xFF, 0x8C, 0x0E, 0xE2,
                0x0C, 0xEF, 0xBC, 0x72, 0x75, 0x6F, 0x37, 0xA1,
                0xEC, 0xD3, 0x8E, 0x62, 0x8B, 0x86, 0x10, 0xE8,
                0x08, 0x77, 0x11, 0xBE, 0x92, 0x4F, 0x24, 0xC5,
                0x32, 0x36, 0x9D, 0xCF, 0xF3, 0xA6, 0xBB, 0xAC,
                0x5E, 0x6C, 0xA9, 0x13, 0x57, 0x25, 0xB5, 0xE3,
                0xBD, 0xA8, 0x3A, 0x01, 0x05, 0x59, 0x2A, 0x46
            };

        public SymAlgoHomebrew()
        {
            LegalBlockSizesValue = new[] { new KeySizes(32, 32, 0) };
            LegalKeySizesValue = new[] { new KeySizes(80, 80, 0) };
            BlockSizeValue = 32;
            KeySizeValue = 80;
            ModeValue = CipherMode.ECB;
            PaddingValue = PaddingMode.None;
        }

        public SymAlgoHomebrew(byte[] key) : this()
        {
            Key = key;
        }

        public override ICryptoTransform CreateDecryptor(byte[] key, byte[] iv) => GetTransform(key, iv, false);
        public override ICryptoTransform CreateEncryptor(byte[] key, byte[] iv) => GetTransform(key, iv, true);

        private ICryptoTransform GetTransform(byte[] key, byte[] iv, bool encrypt) => new CryptTrans(key, encrypt);

        public override void GenerateIV() => IV = new byte[BlockSize / 8];
        public override void GenerateKey() => Key = new byte[0];

        public byte[] Encrypt(byte[] bytes) => DoCrypto(bytes, true);
        public byte[] Decrypt(byte[] bytes) => DoCrypto(bytes, false);

        public uint Encrypt(uint val) => GetUInt(Encrypt(GetBytes(val)));
        public uint Decrypt(uint val) => GetUInt(Decrypt(GetBytes(val)));

        public int Encrypt(int val) => GetInt(Encrypt(GetBytes(val)));
        public int Decrypt(int val) => GetInt(Decrypt(GetBytes(val)));

        private static byte[] GetBytes(uint val) => new[] { (byte)(val >> 24), (byte)(val >> 16), (byte)(val >> 8), (byte)val };
        private static byte[] GetBytes(int val) => new[] { (byte)(val >> 24), (byte)(val >> 16), (byte)(val >> 8), (byte)val };

        private static uint GetUInt(byte[] val) => (uint)(val[0] << 24 | val[1] << 16 | val[2] << 8 | val[3]);
        private static int GetInt(byte[] val) => val[0] << 24 | val[1] << 16 | val[2] << 8 | val[3];

        private static ushort BadCrypto(byte[] key, int keyIndex, ushort seed)
        {
            byte hi = (byte)(seed >> 8 & 255);
            byte lo = (byte)(seed & 255);
            byte b1 = (byte)(BigKey[lo ^ key[(4 * keyIndex + 0) % 10]] ^ hi);
            byte b2 = (byte)(BigKey[b1 ^ key[(4 * keyIndex + 1) % 10]] ^ lo);
            byte b3 = (byte)(BigKey[b2 ^ key[(4 * keyIndex + 2) % 10]] ^ b1);
            byte b4 = (byte)(BigKey[b3 ^ key[(4 * keyIndex + 3) % 10]] ^ b2);
            return (ushort)((b3 << 8) + b4);
        }

        private byte[] DoCrypto(byte[] buffer, bool encrypt)
        {
            byte[] array = new byte[buffer.Length];
            for (int i = 0; i < buffer.Length; i += 4)
            {
                TransformBlock(Key, buffer, i, array, i, encrypt);
            }
            return array;
        }

        private static void TransformBlock(byte[] key, byte[] inputBuffer, int inputOffset, byte[] outputBuffer, int outputOffset, bool encrypt)
        {
            int num1 = encrypt ? 1 : -1;
            int num2 = encrypt ? 0 : 23;

            ushort num3 = (ushort)((inputBuffer[inputOffset + 0] << 8) + inputBuffer[inputOffset + 1]);
            ushort num4 = (ushort)((inputBuffer[inputOffset + 2] << 8) + inputBuffer[inputOffset + 3]);

            for (int i = 0; i < 12; i++)
            {
                num4 ^= (ushort)(BadCrypto(key, num2, num3) ^ num2);
                num2 += num1;
                num3 ^= (ushort)(BadCrypto(key, num2, num4) ^ num2);
                num2 += num1;
            }

            outputBuffer[outputOffset + 0] = (byte)(num4 >> 8);
            outputBuffer[outputOffset + 1] = (byte)(num4 & 255);
            outputBuffer[outputOffset + 2] = (byte)(num3 >> 8);
            outputBuffer[outputOffset + 3] = (byte)(num3 & 255);
        }

        private sealed class CryptTrans : ICryptoTransform, IDisposable
        {
            private readonly byte[] _key;
            private readonly bool _encrypt;

            public int InputBlockSize => 4;
            public int OutputBlockSize => 4;
            public bool CanTransformMultipleBlocks => true;
            public bool CanReuseTransform => true;

            public CryptTrans(byte[] key, bool encrypt)
            {
                _key = key;
                _encrypt = encrypt;
            }

            public void Dispose() { }

            public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
            {
                for (int i = 0; i < inputCount; i += 4)
                    SymAlgoHomebrew.TransformBlock(_key, inputBuffer, inputOffset + i, outputBuffer, outputOffset + i, _encrypt);

                return inputCount;
            }

            public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
            {
                byte[] array = new byte[inputCount];
                TransformBlock(inputBuffer, inputOffset, inputCount, array, 0);
                return array;
            }
        }
    }
}
