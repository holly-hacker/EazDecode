using System;
using System.Diagnostics;

namespace EazDecodeLib
{
	public class CryptoHelper
	{
	    public string Password { get; }

	    private readonly Crypto2 _c2;
	    private readonly Crypto3 _c3;

	    public CryptoHelper(string password)
        {
            Password = password;

            _c2 = new Crypto2(password);
            _c3 = new Crypto3(password);
		}

        /// <summary>
        /// Attempt to decrypt the input by finding the correct algorithm and
        /// applying it.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
		public string Decrypt(string input)
		{
			if (!input.StartsWith("#="))
			{
				throw new NotImplementedException("I don't support this encryption type");
			}
		    char c = input[2];
			string b64 = input.Substring(3);
		    b64 = b64.Replace('_', '+').Replace('$', '/');    //TODO: not always!
		    byte[] bytes = Convert.FromBase64String(b64);

            switch (c) {
                case 'q':
                    return _c2.Decrypt(bytes);
		        case 'z':
		            return _c3.Decrypt(bytes);
            }
			throw new NotImplementedException("I don't support this encryption type (yet), poke me about it");
		}

	    /// <summary>
	    /// Take the last byte of input <paramref name="data"/> and xor it with
	    /// the rest of the array.
	    /// </summary>
	    /// <param name="data">Array including xor byte</param>
	    /// <returns>Xored array w/o last byte</returns>
	    internal static byte[] XorArray(byte[] data)
	    {
            //don't do anything to empty arrays
	        if (data.Length == 0) return data;

            //last byte of the array is the xor key
	        byte xorKey = data[data.Length - 1];
            
            //resize the original array to trim off the xor byte
	        Array.Resize(ref data, data.Length - 1);

            //xor the entire array with this byte
            for (int i = 0; i < data.Length; i++)
                data[i] ^= xorKey;

	        return data;
	    }

        /// <summary>
        /// Xor an array <paramref name="arr"/> at offset 
        /// <paramref name="arrOffset"/> with another array
        /// <paramref name="xor"/> at offset <paramref name="xorOffset"/> for
        /// <paramref name="size"/> bytes.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="arrOffset"></param>
        /// <param name="xor"></param>
        /// <param name="xorOffset"></param>
        /// <param name="size"></param>
	    internal static void XorArray(byte[] arr, int arrOffset, byte[] xor, int xorOffset, int size)
	    {
	        for (int i = 0; i < size; i++)
	        {
	            arr[arrOffset + i] ^= xor[xorOffset + i];
	        }
	    }

	    /// <summary>
        /// Remove all null-bytes at the end of the array.
        /// </summary>
        /// <param name="input"></param>
        /// <returns><paramref name="input"/> without null bytes at the end</returns>
	    internal static byte[] RemoveTrailingNullBytes(byte[] input)
	    {
	        int lastNonZero = input.Length;
	        while (input[lastNonZero - 1] == 0)
	        {
	            lastNonZero--;
	            Debug.Assert(lastNonZero != 0);
	        }

	        byte[] ret = new byte[lastNonZero];
	        Buffer.BlockCopy(input, 0, ret, 0, lastNonZero);
	        return ret;
	    }
	}
}
