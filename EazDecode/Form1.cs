using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace EazDecode
{
	// Token: 0x02000004 RID: 4
	public partial class Form1 : Form
	{
		// Token: 0x06000008 RID: 8 RVA: 0x00002238 File Offset: 0x00000438
		public Form1()
		{
			this.InitializeComponent();
			if (File.Exists("password.txt"))
			{
				this.txtbPassword.Text = File.ReadAllText("password.txt");
			}
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002267 File Offset: 0x00000467
		private void txtbIn_TextChanged(object sender, EventArgs e)
		{
			if (this.cbAuto.Checked)
			{
				this.btnDecode_Click(sender, e);
			}
		}

		// Token: 0x0600000A RID: 10 RVA: 0x0000227E File Offset: 0x0000047E
		private void cbAuto_CheckedChanged(object sender, EventArgs e)
		{
			this.txtbIn_TextChanged(sender, e);
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002288 File Offset: 0x00000488
		private void txtbPassword_TextChanged(object sender, EventArgs e)
		{
			this.Crypto = new CryptoHelper(this.txtbPassword.Text);
			File.WriteAllText("password.txt", this.txtbPassword.Text);
			this.txtbIn_TextChanged(sender, e);
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000022C0 File Offset: 0x000004C0
		private void btnDecode_Click(object sender, EventArgs e)
		{
			try
			{
				this.txtbOut.Text = Decoder.DecodeAll(this.txtbIn.Text, this.Crypto);
			}
			catch (Exception ex)
			{
				this.txtbOut.Text = string.Format("An error occured while decoding: {0}", ex.Message);
				if (ex is CryptographicException)
				{
					TextBox textBox = this.txtbOut;
					textBox.Text += "\n\nPlease check if the password is correct.";
				}
			}
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002344 File Offset: 0x00000544
		private void btnFlip_Click(object sender, EventArgs e)
		{
			this.scMain.Orientation = (Orientation)Math.Abs(this.scMain.Orientation - Orientation.Vertical);
		}

		// Token: 0x04000003 RID: 3
		private CryptoHelper Crypto;
	}
}
