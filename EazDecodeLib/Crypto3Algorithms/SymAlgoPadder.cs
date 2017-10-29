using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace EazDecodeLib.Crypto3Algorithms
{
    /// <inheritdoc />
    /// <summary>
    /// Encrypts an incrementing number one byte at the time. Since the
    /// BlockSize is 8 bits, it can be used for padding.
    /// </summary>
    internal sealed class SymAlgoPadder : SymmetricAlgorithm
    {
        private readonly SymmetricAlgorithm _algo;

        public SymAlgoPadder(SymmetricAlgorithm algo)
        {
            LegalBlockSizesValue = new[] { new KeySizes(8, 8, 0) };
            LegalKeySizesValue = algo.LegalKeySizes;
            BlockSizeValue = 8;     //1 byte
            Mode = CipherMode.ECB;
            Padding = PaddingMode.None;
            algo.Mode = CipherMode.ECB;
            algo.Padding = PaddingMode.None;
            _algo = algo;
        }

        public override ICryptoTransform CreateEncryptor(byte[] key, byte[] iv) => new Transform(_algo, key);
        public override ICryptoTransform CreateDecryptor(byte[] key, byte[] iv) => new Transform(_algo, key);

        public override void GenerateKey() => KeyValue = _algo.Key;
        public override void GenerateIV() => IVValue = new byte[0];

        private sealed class Transform : ICryptoTransform, IDisposable
        {
            private readonly byte[] _block;
            private readonly ICryptoTransform _encryptor;
            private readonly Queue<byte> _queue = new Queue<byte>();

            public int InputBlockSize => 1;
            public int OutputBlockSize => 1;
            public bool CanTransformMultipleBlocks => true;
            public bool CanReuseTransform => false;

            public Transform(SymmetricAlgorithm algo, byte[] key)
            {
                _block = new byte[algo.BlockSize / 8];
                _encryptor = algo.CreateEncryptor(key, new byte[algo.BlockSize / 8]);
            }

            public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
            {
                byte[] array = new byte[inputCount];
                TransformBlock(inputBuffer, inputOffset, inputCount, array, 0);
                return array;
            }

            public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
            {
                //xor every input byte with the encrypted one
                for (int i = 0; i < inputCount; i++)
                    outputBuffer[i + outputOffset] = (byte)(inputBuffer[i + inputOffset] ^ Dequeue());

                return inputCount;
            }

            /// <summary>
            /// Dequeue a byte, enqueueing new ones if needed.
            /// </summary>
            /// <returns></returns>
            private byte Dequeue()
            {
                //if the queue is empty, fill it again
                if (_queue.Count == 0)
                    EnqueueBytes();

                //dequeue a byte and return it
                return _queue.Dequeue();
            }

            /// <summary>
            /// Create a new block and enqueue its bytes for use in the crypto.
            /// </summary>
            private void EnqueueBytes()
            {
                //encrypt the block
                byte[] encrypted = new byte[_block.Length];
                _encryptor.TransformBlock(_block, 0, _block.Length, encrypted, 0);

                //increment the block by 1
                IncrementBlock();

                //enqueue all encrypted bytes
                foreach (byte item in encrypted)
                    _queue.Enqueue(item);
            }

            /// <summary>
            /// Treat <seealso cref="_block"/> as an integer and increase it by 1.
            /// </summary>
            private void IncrementBlock()
            {
                for (int i = _block.Length - 1; i >= 0; i--)
                {
                    //increment buffer[i]
                    _block[i]++;

                    //if it is not not zero, break
                    if (_block[i] != 0) break;
                }
            }

            public void Dispose() { }
        }
    }
}
