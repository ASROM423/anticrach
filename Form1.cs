using System;
using System.IO;
using System.Windows.Forms;

namespace AntiCrackBuilder
{
    public partial class Form1 : Form
    {
        string selectedExe = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "EXE Files|*.exe";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                selectedExe = dlg.FileName;
                lblFile.Text = selectedExe;
            }
        }

        private void btnBuild_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedExe) || string.IsNullOrEmpty(txtWebhook.Text))
            {
                MessageBox.Show("Please select EXE and enter Webhook!");
                return;
            }

            string stubCode = File.ReadAllText("StubTemplate.cs");
            stubCode = stubCode.Replace("WEBHOOK_HERE", txtWebhook.Text);

            File.WriteAllText("FinalStub.cs", stubCode);

            // Compile stub (????? csc.exe)
            var output = Path.Combine(Path.GetDirectoryName(selectedExe), "Protected_" + Path.GetFileName(selectedExe));
            var args = $"/target:winexe /out:\"{output}\" FinalStub.cs";
            System.Diagnostics.Process.Start("csc.exe", args);

            MessageBox.Show("Build done! File: " + output);
        }
    }
}
