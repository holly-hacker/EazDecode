using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EazDecode
{
	// Token: 0x02000002 RID: 2
	internal class CryptoHelper
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public CryptoHelper(string password)
		{
			this._symmetricAlgorithm = new RijndaelManaged
			{
				KeySize = 256,
				BlockSize = 128,
				IV = new Rfc2898DeriveBytes(password, new byte[]
				{
					28,
					136,
					27,
					216,
					83,
					147,
					140,
					207,
					60,
					153,
					41,
					107,
					117,
					164,
					37,
					157,
					94,
					233,
					51,
					48,
					146,
					108,
					127,
					191,
					30,
					226,
					250,
					88,
					109,
					7,
					132,
					15
				}).GetBytes(16),
				Key = new Rfc2898DeriveBytes(password, new byte[]
				{
					167,
					126,
					112,
					16,
					4,
					244,
					15,
					120,
					135,
					116,
					123,
					212,
					157,
					48,
					5,
					194,
					12,
					179,
					153,
					201,
					204,
					249,
					248,
					212,
					86,
					20,
					215,
					55,
					105,
					157,
					111,
					11
				}).GetBytes(32)
			};
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000020D0 File Offset: 0x000002D0
		internal string Decrypt(string input)
		{
			if (!input.StartsWith("#="))
			{
				throw new NotImplementedException("I don't support this encryption type");
			}
			string text = input.Substring(2);
			if (text.StartsWith("q"))
			{
				text = text.Substring(1);
				text = text.Replace('_', '+').Replace('$', '/');
				return this.DecryptWithXor(Convert.FromBase64String(text));
			}
			throw new NotImplementedException("I don't support this encryption type (yet), poke me about it");
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002144 File Offset: 0x00000344
		private string DecryptWithXor(byte[] toDecrypt)
		{
			MemoryStream memoryStream = new MemoryStream();
			using (ICryptoTransform cryptoTransform = this._symmetricAlgorithm.CreateDecryptor())
			{
				CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write);
				cryptoStream.Write(toDecrypt, 0, toDecrypt.Length);
				cryptoStream.FlushFinalBlock();
				cryptoStream.Close();
			}
			toDecrypt = memoryStream.ToArray();
			byte b = toDecrypt[toDecrypt.Length - 1];
			Array.Resize<byte>(ref toDecrypt, toDecrypt.Length - 1);
			for (int i = 0; i < toDecrypt.Length; i++)
			{
				byte[] array = toDecrypt;
				int num = i;
				array[num] ^= b;
			}
			return Encoding.UTF8.GetString(toDecrypt);
		}

		// Token: 0x04000001 RID: 1
		private readonly SymmetricAlgorithm _symmetricAlgorithm;
	}
}
