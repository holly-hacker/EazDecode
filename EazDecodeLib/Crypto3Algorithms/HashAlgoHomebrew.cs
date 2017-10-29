using System.Security.Cryptography;

namespace EazDecodeLib.Crypto3Algorithms
{
    /// <inheritdoc />
    /// <summary>
    /// A homebrew hashing algorithm that consists of xoring and multiplying.
    /// </summary>
    internal sealed class HashAlgoHomebrew : HashAlgorithm
    {
        private const uint IV = 0x811C9DC5;
        private const uint MultKey = 0x01000193;
        private uint _hash;

        public HashAlgoHomebrew()
        {
            HashSizeValue = 32;
            InitSeed();
        }

        public override void Initialize() => InitSeed();
        private void InitSeed() => _hash = IV;

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            _hash = DoHash(_hash, array, ibStart, cbSize);
        }

        protected override byte[] HashFinal()
        {
            return new[] {
                (byte) (_hash >> 24),
                (byte) (_hash >> 16),
                (byte) (_hash >> 8),
                (byte) (_hash >> 0)
            };
        }

        public static int DoHash(byte[] array) => (int)DoHash(IV, array, 0, array.Length);

        private static uint DoHash(uint seed, byte[] array, int ibStart, int cbSize)
        {
            for (int i = ibStart; i < ibStart + cbSize; i++)
                seed = (seed ^ array[i]) * MultKey;

            return seed;
        }
    }
}
