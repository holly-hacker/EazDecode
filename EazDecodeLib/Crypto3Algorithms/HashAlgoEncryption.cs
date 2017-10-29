using System.Security.Cryptography;

namespace EazDecodeLib.Crypto3Algorithms
{
    /// <inheritdoc />
    /// <summary>
    /// Uses a <seealso cref="SymmetricAlgorithm"/> to return an encrypted hash of an empty buffer.
    /// </summary>
    internal sealed class HashAlgoEncryption : HashAlgorithm
    {
        private readonly HashAlgorithm _hashAlgo;
        private readonly SymmetricAlgorithm _symAlgo;

        public HashAlgoEncryption(HashAlgorithm h, SymmetricAlgorithm s)
        {
            HashSizeValue = h.HashSize;
            _hashAlgo = h;
            _symAlgo = s;
        }

        public override bool CanReuseTransform => _hashAlgo.CanReuseTransform;
        public override bool CanTransformMultipleBlocks => _hashAlgo.CanTransformMultipleBlocks;

        public override void Initialize() => _hashAlgo.Initialize();

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            _hashAlgo.TransformBlock(array, ibStart, cbSize, array, ibStart);
        }

        protected override byte[] HashFinal()
        {
            //hash empty byte array
            _hashAlgo.TransformFinalBlock(new byte[0], 0, 0);

            //return encrypted hash
            using (ICryptoTransform trans = _symAlgo.CreateEncryptor())
                return trans.TransformFinalBlock(_hashAlgo.Hash, 0, _hashAlgo.Hash.Length);
        }
    }
}
