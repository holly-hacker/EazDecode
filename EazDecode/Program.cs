using System;
using System.Windows.Forms;

namespace EazDecode
{
	// Token: 0x02000005 RID: 5
	internal static class Program
	{
		// Token: 0x06000010 RID: 16 RVA: 0x00002AE2 File Offset: 0x00000CE2
		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
		}
	}
}
