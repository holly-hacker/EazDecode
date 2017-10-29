using EazDecodeLib;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace EazDecode
{
	public partial class Form1
	{
	    private CryptoHelper _crypto;

        public Form1()
		{
			InitializeComponent();

		    if (File.Exists("password.txt"))
		        txtbPassword.Text = File.ReadAllText("password.txt");
		}

		private void txtbIn_TextChanged(object sender, EventArgs e)
		{
		    if (cbAuto.Checked)
		        btnDecode_Click(sender, e);
		}

		private void cbAuto_CheckedChanged(object sender, EventArgs e) => txtbIn_TextChanged(sender, e);

	    private void txtbPassword_TextChanged(object sender, EventArgs e)
		{
			_crypto = new CryptoHelper(txtbPassword.Text);
			File.WriteAllText("password.txt", txtbPassword.Text);
			txtbIn_TextChanged(sender, e);
		}

		private void btnDecode_Click(object sender, EventArgs e)
		{
			try
			{
				txtbOut.Text = Decoder.DecodeAll(txtbIn.Text, _crypto);
			}
			catch (Exception ex)
			{
				txtbOut.Text = $"An error occured while decoding ourself: {ex.Message}\r\n";
				if (ex is CryptographicException)
				{
				    txtbOut.Text += "Please check if the password is correct.\r\n";
				}

			    txtbOut.Text += "\r\n" + new string('-', 20) + "\r\n\r\n";

			    try
                {
			        string str = Eazfuscator.NET.SDK.StackTraceDecoding.StackTraceDecoder.Run(txtbIn.Text, _crypto.Password) + "\n";
                    txtbOut.Text += str;
                }
                catch (Exception ex2)
                {
			        txtbOut.Text += "Official SDK could also not decrypt this: " + ex2.Message + "\n";
			        throw;
			    }
			}
		}

		private void btnFlip_Click(object sender, EventArgs e)
		{
			scMain.Orientation = (Orientation)Math.Abs(scMain.Orientation - Orientation.Vertical);
		}
	}
}
