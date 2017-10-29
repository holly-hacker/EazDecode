using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;

namespace EazDecodeLib.Crypto3Algorithms
{
    /// <inheritdoc />
    /// <summary>
    /// Chain various algorithms in order of descending blocksizes, to prevent 
    /// padding as much as possible.
    /// </summary>
    internal sealed class SymAlgoLengthOptimized : SymmetricAlgorithm
    {
        public int IVSize;

        private readonly SymmetricAlgorithm[] _algos;

        public SymAlgoLengthOptimized(IEnumerable<SymmetricAlgorithm> algos)
        {
            //sort input by blocksize
            var l = algos.ToList();
            l.Sort((x, y) => y.BlockSize.CompareTo(x.BlockSize));
            _algos = l.ToArray();

            //set all algos to ECB and get total key size
            int totalKeySize = 0;
            int lastBlockSize = -1;
            foreach (var alg in _algos)
            {
                Debug.Assert(lastBlockSize != alg.BlockSize, $"BlockSize being equal to {nameof(lastBlockSize)} would throw an exception");
                lastBlockSize = alg.BlockSize;
                totalKeySize += alg.KeySize;
                alg.Mode = CipherMode.ECB;
                alg.Padding = PaddingMode.None;
            }

            //set algo settings
            BlockSizeValue = _algos[_algos.Length - 1].BlockSize;   //TODO: also last blocksize?
            LegalBlockSizesValue = new[] { new KeySizes(BlockSizeValue, BlockSizeValue, 0) };
            KeySizeValue = totalKeySize;
            LegalKeySizesValue = new[] { new KeySizes(totalKeySize, totalKeySize, 0) };
            IVSize = _algos[0].BlockSize;
            Mode = CipherMode.ECB;
            Padding = PaddingMode.None;
        }

        public override byte[] IV
        {
            get => base.IV;
            set => IVValue = (byte[])value.Clone();
        }

        public override ICryptoTransform CreateEncryptor(byte[] key, byte[] iv) => GetCryptoTransform(key, iv, true);
        public override ICryptoTransform CreateDecryptor(byte[] key, byte[] iv) => GetCryptoTransform(key, iv, false);
        private ICryptoTransform GetCryptoTransform(byte[] key, byte[] iv, bool encrypt) => new CryptTrans(_algos, key, iv, encrypt);

        public override void GenerateKey() => throw new NotSupportedException();
        public override void GenerateIV() => throw new NotSupportedException();

        private sealed class CryptTrans : ICryptoTransform, IDisposable
        {
            private readonly SymmetricAlgorithm[] _algos;
            private readonly int _blockSize;
            private readonly bool _encrypt;
            private readonly byte[] _iv;
            private readonly byte[] _key;
            private ICryptoTransform[] _transforms;

            public int InputBlockSize => _blockSize;
            public int OutputBlockSize => _blockSize;
            public bool CanTransformMultipleBlocks => true;
            public bool CanReuseTransform => true;

            public CryptTrans(SymmetricAlgorithm[] algos, byte[] key, byte[] iv, bool encrypt)
            {
                _algos = algos;
                _key = key;
                _encrypt = encrypt;
                _iv = iv;
                _blockSize = algos[algos.Length - 1].BlockSize / 8;
            }

            public void Dispose()
            {
                if (_transforms == null) return;
                foreach (ICryptoTransform t in _transforms) t?.Dispose();
                _transforms = null;
            }

            public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
            {
                //copy input to output
                Buffer.BlockCopy(inputBuffer, inputOffset, outputBuffer, outputOffset, inputCount);

                //create ICryptoTransforms
                PopulateTransforms();

                //encrypt or decrypt
                if (_encrypt)
                    Encrypt(outputBuffer, outputOffset, inputCount);
                else
                    Decrypt(outputBuffer, outputOffset, inputCount);

                //kind of useless in this case, but return input size
                return inputCount;
            }

            private void PopulateTransforms()
            {
                //don't do this if we already did before
                if (_transforms != null) return;

                _transforms = new ICryptoTransform[_algos.Length];

                int keyOffset = 0;
                for (int i = 0; i < _algos.Length; i++)
                {
                    SymmetricAlgorithm algo = _algos[i];

                    //get array with key and iv
                    int keySizeBytes = algo.KeySize / 8;
                    byte[] key = new byte[keySizeBytes];
                    Buffer.BlockCopy(_key, keyOffset, key, 0, keySizeBytes);
                    byte[] iv = new byte[algo.BlockSize / 8];

                    //increment key offset
                    keyOffset += keySizeBytes;

                    //get actual transform
                    ICryptoTransform cryptoTransform = _encrypt
                        ? algo.CreateEncryptor(key, iv)
                        : algo.CreateDecryptor(key, iv);

                    //some checks
                    Debug.Assert(cryptoTransform.CanReuseTransform);
                    Debug.Assert(algo.BlockSize == cryptoTransform.InputBlockSize * 8);
                    Debug.Assert(cryptoTransform.InputBlockSize == cryptoTransform.OutputBlockSize);

                    //store in array
                    _transforms[i] = cryptoTransform;
                }
            }

            private void Encrypt(byte[] buffer, int offset, int count)
            {
                //store iv in block
                byte[] block = new byte[_iv.Length];
                Buffer.BlockCopy(_iv, 0, block, 0, block.Length);

                int lastOffset = 0;
                foreach (ICryptoTransform transform in _transforms)
                {
                    //calculate size of current "chunk"
                    int blockSize = transform.InputBlockSize;
                    int currentCount = count - lastOffset & ~(blockSize - 1);  //count - rounded lastOffset, eg: count - lastOffset & 0b11111111_11000000
                    int nextOffset = lastOffset + currentCount;

                    for (int i = lastOffset; i < nextOffset; i += blockSize)
                    {
                        //xor buffer with block
                        int bufferOffset = i + offset;
                        CryptoHelper.XorArray(buffer, bufferOffset, block, 0, blockSize);

                        //decrypt buffer to buffer
                        if (transform.TransformBlock(buffer, bufferOffset, blockSize, buffer, bufferOffset) != blockSize) throw new Exception();

                        //copy buffer to block
                        Buffer.BlockCopy(buffer, bufferOffset, block, 0, blockSize);
                    }

                    //update lastOffset
                    lastOffset = nextOffset;

                    //if we're at the end, stop
                    if (nextOffset == count)
                        break;
                }
            }

            private void Decrypt(byte[] buffer, int offset, int count)
            {
                //allocate buffers
                byte[] block = new byte[_iv.Length];
                Buffer.BlockCopy(_iv, 0, block, 0, block.Length);
                byte[] tempBuffer = new byte[block.Length];

                int lastOffset = 0;
                foreach (ICryptoTransform transform in _transforms)
                {
                    //calculate size of current "chunk"
                    int blockSize = transform.InputBlockSize;
                    int currentCount = count - lastOffset & ~(blockSize - 1);   //how much we're doing now
                    int nextOffset = lastOffset + currentCount;

                    for (int i = lastOffset; i < nextOffset; i += blockSize)
                    {
                        //copy buffer to tempBuffer
                        int bufferOffset = i + offset;
                        Buffer.BlockCopy(buffer, bufferOffset, tempBuffer, 0, blockSize);

                        //decrypt buffer into buffer
                        if (transform.TransformBlock(buffer, bufferOffset, blockSize, buffer, bufferOffset) != blockSize) throw new Exception();

                        //xor buffer with block
                        CryptoHelper.XorArray(buffer, bufferOffset, block, 0, blockSize);

                        //copy tempBuffer to block to xor the next time
                        Buffer.BlockCopy(tempBuffer, 0, block, 0, blockSize);
                    }

                    //update lastOffset
                    lastOffset = nextOffset;

                    //if we're at the end, stop
                    if (nextOffset == count)
                        break;
                }
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
