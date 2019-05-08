using System.Windows.Forms;
using LibGit2Sharp;
using System.Threading;

namespace RVUpdateClient
{
	public partial class Form1 : Form
	{
		
		delegate void WriteLineDelegate(string text);
		delegate void ProgressBarDelegate(bool visible);
        delegate void LaunchButtonDelegate(bool visible);

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

        public void LaunchButtonToggle(bool enabled)
        {
            if (!LaunchButton.InvokeRequired)
                LaunchButton.Enabled = enabled;
            else
            {
                LaunchButtonDelegate l = new LaunchButtonDelegate(LaunchButtonToggle);
                this.Invoke(l, new object[] { enabled });
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
			model = new Model(WriteLine,ProgressToggle,LaunchButtonToggle);
			model.SanityCheck();
			textBox1.SelectionStart = 0;
			textBox1.ReadOnly = true;
			progressBar1.Style = ProgressBarStyle.Marquee;
		}

		private void btnUpdateMod_Click(object sender, System.EventArgs e)
		{
			if (backgroundWorker1.IsBusy != true)
			{
				backgroundWorker1.RunWorkerAsync();
				ProgressToggle(true);
			} else
            {
                WriteLine("Update Already In Progress");
            }
		}

		private void btnCheckUpdates_Click(object sender, System.EventArgs e)
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

        private void LaunchButton_Click(object sender, System.EventArgs e)
        {
            if (backgroundWorker1.IsBusy != true)
                model.LaunchMod();
            else
                WriteLine("Can't launch while update is in progress");
        }
    }
}
