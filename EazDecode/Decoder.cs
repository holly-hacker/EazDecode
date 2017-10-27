using System;
using System.Text.RegularExpressions;

namespace EazDecode
{
	// Token: 0x02000003 RID: 3
	internal class Decoder
	{
		// Token: 0x06000004 RID: 4 RVA: 0x000021E0 File Offset: 0x000003E0
		public static string DecodeAll(string input, CryptoHelper crypto)
		{
			return Decoder.RegexObfuscated.Replace(input, (Match match) => Decoder.Evaluator(match, crypto));
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002211 File Offset: 0x00000411
		private static string Evaluator(Match match, CryptoHelper crypto)
		{
			return crypto.Decrypt(match.Value);
		}

		// Token: 0x04000002 RID: 2
		private static readonly Regex RegexObfuscated = new Regex("#=[a-zA-Z0-9_$]+={0,2}");
	}
}
