using System.Windows.Forms;
using LibGit2Sharp;
using System.Threading;

namespace RVUpdateClient
{
	public partial class Form1 : Form
	{
		
		delegate void WriteLineDelegate(string text);
		delegate void ProgressBarDelegate(bool visible);
		Model model;
		public Form1()
		{
			InitializeComponent();
		}

		public void ProgressToggle(bool visible)
		{
			if (!progressBar1.InvokeRequired)
				progressBar1.Visible = visible;
			else
			{
				ProgressBarDelegate d = new ProgressBarDelegate(ProgressToggle);
				this.Invoke(d, new object[] { visible });
			}
		}
		public void WriteLine(string text)
		{
			if (!textBox1.InvokeRequired)
				textBox1.AppendText(text + "\r\n");
			else
			{
				WriteLineDelegate d = new WriteLineDelegate(WriteLine);
				this.Invoke(d, new object[] { text });
			}
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
			Agent.Initialize(WriteLine);
			model = new Model(WriteLine,ProgressToggle);
			model.SanityCheck();
			textBox1.SelectionStart = 0;
			textBox1.ReadOnly = true;
			progressBar1.Style = ProgressBarStyle.Marquee;
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
			if (backgroundWorker1.IsBusy != true)
			{
				backgroundWorker1.RunWorkerAsync();
				ProgressToggle(true);
			}
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			model.SanityCheck();
		}

		private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
		{
			model.UpdateMod();
		}

		private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
		{
			ProgressToggle(false);
		}
	}
}
