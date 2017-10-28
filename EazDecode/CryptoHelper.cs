using System;

namespace EazDecode
{
	internal class CryptoHelper
	{
	    public string Password { get; }

	    private Crypto2 _c2;
	    private Crypto3 _c3;

	    public CryptoHelper(string password)
        {
            Password = password;

            _c2 = new Crypto2(password);
            _c3 = new Crypto3(password);
		}

		internal string Decrypt(string input)
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
	}
}
