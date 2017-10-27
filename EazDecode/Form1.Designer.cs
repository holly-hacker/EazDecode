namespace EazDecode
{
	// Token: 0x02000004 RID: 4
	public partial class Form1 : global::System.Windows.Forms.Form
	{
		// Token: 0x0600000E RID: 14 RVA: 0x00002363 File Offset: 0x00000563
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002384 File Offset: 0x00000584
		private void InitializeComponent()
		{
			this.tableLayoutPanel1 = new global::System.Windows.Forms.TableLayoutPanel();
			this.scMain = new global::System.Windows.Forms.SplitContainer();
			this.txtbIn = new global::System.Windows.Forms.TextBox();
			this.txtbOut = new global::System.Windows.Forms.TextBox();
			this.panel1 = new global::System.Windows.Forms.Panel();
			this.cbAuto = new global::System.Windows.Forms.CheckBox();
			this.btnDecode = new global::System.Windows.Forms.Button();
			this.btnFlip = new global::System.Windows.Forms.Button();
			this.label1 = new global::System.Windows.Forms.Label();
			this.txtbPassword = new global::System.Windows.Forms.TextBox();
			this.tableLayoutPanel1.SuspendLayout();
			((global::System.ComponentModel.ISupportInitialize)this.scMain).BeginInit();
			this.scMain.Panel1.SuspendLayout();
			this.scMain.Panel2.SuspendLayout();
			this.scMain.SuspendLayout();
			this.panel1.SuspendLayout();
			base.SuspendLayout();
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle(global::System.Windows.Forms.SizeType.Percent, 100f));
			this.tableLayoutPanel1.Controls.Add(this.scMain, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
			this.tableLayoutPanel1.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new global::System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new global::System.Windows.Forms.RowStyle(global::System.Windows.Forms.SizeType.Absolute, 50f));
			this.tableLayoutPanel1.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new global::System.Drawing.Size(514, 358);
			this.tableLayoutPanel1.TabIndex = 0;
			this.scMain.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.scMain.Location = new global::System.Drawing.Point(3, 53);
			this.scMain.Name = "scMain";
			this.scMain.Panel1.Controls.Add(this.txtbIn);
			this.scMain.Panel2.Controls.Add(this.txtbOut);
			this.scMain.Size = new global::System.Drawing.Size(508, 302);
			this.scMain.SplitterDistance = 169;
			this.scMain.TabIndex = 1;
			this.txtbIn.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.txtbIn.Location = new global::System.Drawing.Point(0, 0);
			this.txtbIn.Multiline = true;
			this.txtbIn.Name = "txtbIn";
			this.txtbIn.Size = new global::System.Drawing.Size(169, 302);
			this.txtbIn.TabIndex = 0;
			this.txtbIn.TextChanged += new global::System.EventHandler(this.txtbIn_TextChanged);
			this.txtbOut.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.txtbOut.Location = new global::System.Drawing.Point(0, 0);
			this.txtbOut.Multiline = true;
			this.txtbOut.Name = "txtbOut";
			this.txtbOut.Size = new global::System.Drawing.Size(335, 302);
			this.txtbOut.TabIndex = 0;
			this.panel1.Controls.Add(this.cbAuto);
			this.panel1.Controls.Add(this.btnDecode);
			this.panel1.Controls.Add(this.btnFlip);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.txtbPassword);
			this.panel1.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new global::System.Drawing.Point(3, 3);
			this.panel1.Name = "panel1";
			this.panel1.Size = new global::System.Drawing.Size(508, 44);
			this.panel1.TabIndex = 2;
			this.cbAuto.AutoSize = true;
			this.cbAuto.Checked = true;
			this.cbAuto.CheckState = global::System.Windows.Forms.CheckState.Checked;
			this.cbAuto.Location = new global::System.Drawing.Point(6, 23);
			this.cbAuto.Name = "cbAuto";
			this.cbAuto.Size = new global::System.Drawing.Size(87, 17);
			this.cbAuto.TabIndex = 6;
			this.cbAuto.Text = "Auto-decode";
			this.cbAuto.UseVisualStyleBackColor = true;
			this.cbAuto.CheckedChanged += new global::System.EventHandler(this.cbAuto_CheckedChanged);
			this.btnDecode.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Right);
			this.btnDecode.FlatStyle = global::System.Windows.Forms.FlatStyle.Flat;
			this.btnDecode.Location = new global::System.Drawing.Point(399, 0);
			this.btnDecode.Name = "btnDecode";
			this.btnDecode.Size = new global::System.Drawing.Size(66, 44);
			this.btnDecode.TabIndex = 5;
			this.btnDecode.Text = "Decode";
			this.btnDecode.UseVisualStyleBackColor = true;
			this.btnDecode.Click += new global::System.EventHandler(this.btnDecode_Click);
			this.btnFlip.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Right);
			this.btnFlip.FlatStyle = global::System.Windows.Forms.FlatStyle.Flat;
			this.btnFlip.Location = new global::System.Drawing.Point(464, 0);
			this.btnFlip.Name = "btnFlip";
			this.btnFlip.Size = new global::System.Drawing.Size(44, 44);
			this.btnFlip.TabIndex = 5;
			this.btnFlip.Text = "Flip";
			this.btnFlip.UseVisualStyleBackColor = true;
			this.btnFlip.Click += new global::System.EventHandler(this.btnFlip_Click);
			this.label1.AutoSize = true;
			this.label1.Location = new global::System.Drawing.Point(3, 6);
			this.label1.Name = "label1";
			this.label1.Size = new global::System.Drawing.Size(56, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "Password:";
			this.txtbPassword.Location = new global::System.Drawing.Point(59, 3);
			this.txtbPassword.Name = "txtbPassword";
			this.txtbPassword.PasswordChar = '•';
			this.txtbPassword.Size = new global::System.Drawing.Size(150, 20);
			this.txtbPassword.TabIndex = 3;
			this.txtbPassword.TextChanged += new global::System.EventHandler(this.txtbPassword_TextChanged);
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = global::System.Drawing.Color.MediumSeaGreen;
			base.ClientSize = new global::System.Drawing.Size(514, 358);
			base.Controls.Add(this.tableLayoutPanel1);
			base.Name = "Form1";
			base.ShowIcon = false;
			this.Text = "EazDecode";
			this.tableLayoutPanel1.ResumeLayout(false);
			this.scMain.Panel1.ResumeLayout(false);
			this.scMain.Panel1.PerformLayout();
			this.scMain.Panel2.ResumeLayout(false);
			this.scMain.Panel2.PerformLayout();
			((global::System.ComponentModel.ISupportInitialize)this.scMain).EndInit();
			this.scMain.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			base.ResumeLayout(false);
		}

		// Token: 0x04000004 RID: 4
		private global::System.ComponentModel.IContainer components;

		// Token: 0x04000005 RID: 5
		private global::System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;

		// Token: 0x04000006 RID: 6
		private global::System.Windows.Forms.SplitContainer scMain;

		// Token: 0x04000007 RID: 7
		private global::System.Windows.Forms.TextBox txtbIn;

		// Token: 0x04000008 RID: 8
		private global::System.Windows.Forms.TextBox txtbOut;

		// Token: 0x04000009 RID: 9
		private global::System.Windows.Forms.Panel panel1;

		// Token: 0x0400000A RID: 10
		private global::System.Windows.Forms.Label label1;

		// Token: 0x0400000B RID: 11
		private global::System.Windows.Forms.TextBox txtbPassword;

		// Token: 0x0400000C RID: 12
		private global::System.Windows.Forms.Button btnFlip;

		// Token: 0x0400000D RID: 13
		private global::System.Windows.Forms.Button btnDecode;

		// Token: 0x0400000E RID: 14
		private global::System.Windows.Forms.CheckBox cbAuto;
	}
}
