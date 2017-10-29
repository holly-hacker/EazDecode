using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using EazDecodeLib.Properties;

namespace EazDecodeLib
{
    /// <summary>
    /// Used by <seealso cref="Crypto3"/>
    /// </summary>
    internal class SymbolDecompressor
    {
        private const string ValidChars = @"0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_.<>`";

        private Dictionary<string, int> _wordsDic;
        private List<KeyValuePair<int, string>> _wordsList;

        private readonly byte[] _toReal = new byte[256];
        private readonly byte[] _fromReal = new byte[256];  //unused
        private readonly int _commonWordsStart;
        private readonly byte _cryptoKey;

        public SymbolDecompressor()
        {
            Read(Resources.res);

            long num = ((_wordsList.Count + 256L) * 256L / 255L >> 8) + 1L;
            int index = 1;
            foreach (char c in ValidChars)
            {
                byte b = (byte) c;
                _fromReal[b] = (byte)index;
                _toReal[index] = b;
                index++;
            }
            _commonWordsStart = index - 1;
            _cryptoKey = checked((byte) (256L - num));
        }

        public string Decompress(byte[] buffer)
        {
            using (var memoryStream = new MemoryStream(buffer, false))
            using (var binaryReader = new BinaryReader(memoryStream)) {
                var stringBuilder = new StringBuilder();

                while (memoryStream.Position < memoryStream.Length) {
                    //read encrypted index
                    int index = binaryReader.ReadByteCrypted(_cryptoKey);

                    //if the index is for a common word
                    if (index > _commonWordsStart) {
                        index -= _commonWordsStart;
                        stringBuilder.Append(_wordsList[index - 1].Value);
                    }
                    //else, it is a character
                    else {
                        char value = (char) _toReal[index];
                        stringBuilder.Append(value);
                    }
                }
                return stringBuilder.ToString();
            }
        }

        private void Read(byte[] src)
        {
            using (var ms = new MemoryStream(src, false))
            using (var br = new BinaryReader(ms))
            {
                Guid guid = br.ReadGuid();
                byte b = br.ReadByte();
                Debug.Assert(guid == new Guid("{397590B3-8E32-442F-B114-9C4C9754E169}"));
                Debug.Assert(b == 1);

                _wordsDic = new Dictionary<string, int>();
                _wordsList = new List<KeyValuePair<int, string>>();
                while (ms.Position < ms.Length)
                {
                    string word = br.ReadStringNullTerminated();
                    int id = br.ReadInt32();

                    _wordsDic[word] = _wordsDic.Count;
                    _wordsList.Add(new KeyValuePair<int, string>(id, word));
                }
            }
        }
    }
}
