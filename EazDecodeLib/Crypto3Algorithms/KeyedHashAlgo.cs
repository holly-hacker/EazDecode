using System;
using System.Security.Cryptography;

namespace EazDecodeLib.Crypto3Algorithms
{
    /// <inheritdoc />
    /// <summary>
    /// Chain a hashing algorithm and call it a keyed one.
    /// </summary>
    internal sealed class KeyedHashAlgo : KeyedHashAlgorithm
    {
        private readonly HashAlgorithm _hashAlgo1;
        private readonly HashAlgorithm _hashAlgo2;

        private byte[] _buffer1;
        private byte[] _buffer2;

        private bool _seeded;   //could also mean "in use"

        private int _hashSize = 64; //default value never used

        public KeyedHashAlgo(HashAlgorithm hashAlgo, byte[] key)
        {
            SetMyHashSize(hashAlgo.HashSize / 8);
            _hashAlgo1 = hashAlgo;
            _hashAlgo2 = hashAlgo;
            SetKey(key);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                ((IDisposable)_hashAlgo1)?.Dispose();
                ((IDisposable)_hashAlgo2)?.Dispose();

                if (_buffer1 != null) Array.Clear(_buffer1, 0, _buffer1.Length);
                if (_buffer2 != null) Array.Clear(_buffer2, 0, _buffer2.Length);
            }
            base.Dispose(disposing);
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            if (!_seeded) {
                _hashAlgo1.TransformBlock(_buffer1, 0, _buffer1.Length, _buffer1, 0);
                _seeded = true;
            }
            _hashAlgo1.TransformBlock(array, ibStart, cbSize, array, ibStart);
        }

        protected override byte[] HashFinal()
        {
            //hash buffer1 contents if not done yet
            if (!_seeded) {
                _hashAlgo1.TransformBlock(_buffer1, 0, _buffer1.Length, _buffer1, 0);
                _seeded = true;
            }

            //get hash from _hashAlgo1
            byte[] zeroBuffer = new byte[0];
            _hashAlgo1.TransformFinalBlock(zeroBuffer, 0, 0);
            byte[] hash = _hashAlgo1.Hash;

            //make sure we start fresh
            if (_hashAlgo2 == _hashAlgo1)
                _hashAlgo1.Initialize();

            //hash _buffer2, hash hash, then run the original zero buffer (which is the same as hash?) through
            _hashAlgo2.TransformBlock(_buffer2, 0, _buffer2.Length, _buffer2, 0);
            _hashAlgo2.TransformBlock(hash, 0, hash.Length, hash, 0);
            _hashAlgo2.TransformFinalBlock(zeroBuffer, 0, 0);

            //reset seeded var
            _seeded = false;

            return _hashAlgo2.Hash;
        }

        public override void Initialize()
        {
            _hashAlgo1.Initialize();
            _hashAlgo2.Initialize();
            _seeded = false;
        }

        private void SetKey(byte[] key)
        {
            //reset buffers
            _buffer1 = null;
            _buffer2 = null;

            //hash key if it is too small, else use it as is
            KeyValue = key.Length > GetMyHashSize()
                ? _hashAlgo1.ComputeHash(key)
                : (byte[])key.Clone();

            //init buffers
            InitBuffers();
        }

        private void InitBuffers()
        {
            int hashSize = GetMyHashSize();

            //init buffers
            if (_buffer1 == null) _buffer1 = new byte[hashSize];
            if (_buffer2 == null) _buffer2 = new byte[hashSize];

            //fill buffers with constants
            for (int i = 0; i < hashSize; i++)
            {
                _buffer1[i] = 54;
                _buffer2[i] = 92;
            }

            //xor buffers with key, can maybe be merged with above for loop
            for (int i = 0; i < KeyValue.Length; i++)
            {
                _buffer1[i] ^= KeyValue[i];
                _buffer2[i] ^= KeyValue[i];
            }
        }

        private int GetMyHashSize() => _hashSize;
        private void SetMyHashSize(int val) => _hashSize = val;

        public override byte[] Key
        {
            get => (byte[])KeyValue.Clone();
            set => SetKey(value);
        }
    }
}
