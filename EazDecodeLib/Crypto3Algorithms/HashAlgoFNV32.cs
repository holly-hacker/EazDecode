using System.Security.Cryptography;

namespace EazDecodeLib.Crypto3Algorithms
{
    /// <inheritdoc />
    /// <summary>
    /// An implementation of the Fowler–Noll–Vo 1a hashing algorithm.
    /// </summary>
    internal sealed class HashAlgoFNV32 : HashAlgorithm
    {
        private const uint Basis = 0x811C9DC5;
        private const uint Prime = 0x01000193;
        private uint _hash;

        public HashAlgoFNV32()
        {
            HashSizeValue = 32;
            InitSeed();
        }

        public override void Initialize() => InitSeed();
        private void InitSeed() => _hash = Basis;

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

        public static int DoHash(byte[] array) => (int)DoHash(Basis, array, 0, array.Length);

        private static uint DoHash(uint hash, byte[] array, int ibStart, int cbSize)
        {
            for (int i = ibStart; i < ibStart + cbSize; i++) {
                hash ^= array[i];
                hash *= Prime;
            }

            return hash;
        }
    }
}
