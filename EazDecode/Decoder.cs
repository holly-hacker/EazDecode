using EazDecodeLib;
using System;
using System.Text.RegularExpressions;

namespace EazDecode
{
	internal class Decoder
	{
	    private static readonly Regex RegexObfuscated = new Regex("#=[a-zA-Z0-9_$]+={0,2}");

        public static string DecodeAll(string input, CryptoHelper crypto) => RegexObfuscated.Replace(input, match => Evaluator(match, crypto));
	    private static string Evaluator(Capture match, CryptoHelper crypto) => crypto.Decrypt(match.Value);
	}
}
