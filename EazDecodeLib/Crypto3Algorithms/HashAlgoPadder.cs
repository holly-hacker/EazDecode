using System.Security.Cryptography;

namespace EazDecodeLib.Crypto3Algorithms
{
    /// <inheritdoc />
    /// <summary>
    /// Hashes a 0 byte using a passed <seealso cref="HashAlgorithm"/>.
    /// </summary>
    internal sealed class HashAlgoPadder : HashAlgorithm
    {
        private readonly HashAlgorithm _hashAlgo;

        public override bool CanReuseTransform => _hashAlgo.CanReuseTransform;
        public override bool CanTransformMultipleBlocks => _hashAlgo.CanTransformMultipleBlocks;

        public HashAlgoPadder(HashAlgorithm hashAlgo)
        {
            HashSizeValue = 8;
            _hashAlgo = hashAlgo;
        }

        public override void Initialize() => _hashAlgo.Initialize();

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            _hashAlgo.TransformBlock(array, ibStart, cbSize, array, ibStart);
        }

        protected override byte[] HashFinal()
        {
            _hashAlgo.TransformFinalBlock(new byte[0], 0, 0);

            return new[] { _hashAlgo.Hash[0] };
        }
    }
}
