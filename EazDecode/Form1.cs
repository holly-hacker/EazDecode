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
			{
				txtbPassword.Text = File.ReadAllText("password.txt");
			}
		}

		private void txtbIn_TextChanged(object sender, EventArgs e)
		{
		    if (cbAuto.Checked)
		        btnDecode_Click(sender, e);
		}

		private void cbAuto_CheckedChanged(object sender, EventArgs e)
		{
			txtbIn_TextChanged(sender, e);
		}

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
				txtbOut.Text = $"An error occured while decoding: {ex.Message}";
				if (ex is CryptographicException)
				{
					TextBox textBox = txtbOut;
					textBox.Text += "\n\nPlease check if the password is correct.";
				}
			}
		}

		private void btnFlip_Click(object sender, EventArgs e)
		{
			scMain.Orientation = (Orientation)Math.Abs(scMain.Orientation - Orientation.Vertical);
		}
	}
}
