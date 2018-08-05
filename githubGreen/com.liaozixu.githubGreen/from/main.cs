using githubGreen.com.liaozixu.githubGreen.util;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace com.liaozixu.githubGreen.from
{
    public partial class main : Form
    {
        public main()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = comboBox1.Items.IndexOf("orderliness");
            Console.WriteLine(comboBox1.SelectedItem);
        }

        private void textBox3_MouseClick(object sender, MouseEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "please select the directory. When clone is start, the system will add another project folder under the selected directory.";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    return;
                }
                string filePath = dialog.SelectedPath;
                textBox3.Text = filePath;
            }
        }


        private void textBox4_MouseClick(object sender, MouseEventArgs e)
        {
            string path = string.Empty;
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filter = "Files (*.exe)|*.exe"
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox4.Text = dialog.FileName;
            }
        }

        public void runningStatus(Boolean status)
        {
            if (!status)
            {
                label12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
                label12.Text = "READY";
                button2.Text = "start";
                button2.Enabled = true;
            }
            else
            {
                label12.ForeColor = System.Drawing.Color.Red;
                label12.Text = "RUNNING...";
                button2.Text = "RUNNING";
                button2.Enabled = false;
            }
            System.Threading.Thread.Sleep(1 * 1000);
        }


        public String runGit(string gitPath, string shell)
        {
            Process p = new Process();
            p.StartInfo.FileName = gitPath;
            p.StartInfo.Arguments = shell;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.Start();
            p.StandardInput.WriteLine(shell);
            /* 这个问题解决了我5个小时!!! */
            String outputDataString = p.StandardOutput.ReadToEnd();
            String outputErrorDataString = p.StandardError.ReadToEnd();
            p.WaitForExit();
            p.Close();
            return (outputDataString != "") ? outputDataString : outputErrorDataString;
        }

        private void setDate(int year, int month, int day)
        {
            DateTime t = DateTime.Now;
            t = Convert.ToDateTime(year + "-" + month + "-" + day + " " + t.Hour + ":" + t.Minute + ":" + t.Second);
            SystemTime st = new SystemTime();
            st.FromDateTime(t);
            Win32API.SetLocalTime(ref st);
            textBox2.AppendText("set the system time as:" + DateTime.Now.ToString() + Environment.NewLine);
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            textBox2.Text = "";
            if (textBox3.Text == null || textBox3.Text == "")
            {
                MessageBox.Show("temp path is none");
                return;
            }
            if (textBox4.Text == null || textBox4.Text == "")
            {
                MessageBox.Show("git exe path is none");
                return;
            }
            if (textBox1.Text == null || textBox1.Text == "")
            {
                MessageBox.Show("clone url is none");
                return;
            }
            if (!Directory.Exists(textBox3.Text))
            {
                MessageBox.Show("folder does not exist");
                return;
            }
            String gitPath = textBox4.Text;
            String tempPath = textBox3.Text;
            String cloneUrl = textBox1.Text;
            DateTime startTime = Convert.ToDateTime(dateTimePicker1.Text);
            DateTime endTime = Convert.ToDateTime(dateTimePicker2.Text);
            if (startTime == endTime || endTime < startTime)
            {
                MessageBox.Show("the date is wrong");
                return;
            }
            String gitVersion = runGit(gitPath, "--version");
            Console.WriteLine(gitVersion);
            if (!gitVersion.Contains("git version"))
            {
                MessageBox.Show("git exe is error");
                return;
            }
            else
            {
                textBox2.AppendText("Git version:" + gitVersion + Environment.NewLine);
            }
            runningStatus(true);
            textBox2.AppendText("cloneing.Please wait a little later..." + Environment.NewLine);
            String gitCloneStr = gitClone(gitPath, cloneUrl, tempPath);
            if (gitCloneStr.Equals("error"))
            {
                runningStatus(false);
                return;
            }
            textBox2.AppendText("clone complete!" + Environment.NewLine);
            String tempFileUrl = tempPath + "/githubGreen-csharp.txt";
            FileStream fs = new FileStream(tempFileUrl, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine("githubGreen");
            sw.Flush();
            sw.Close();
            fs.Close();
            runGit(gitPath, "-C " + tempPath + " add *");
            if (!runGit(gitPath, "-C " + tempPath + " commit -m \"init\"").Contains("changed"))
            {
                MessageBox.Show("commit error");
                runningStatus(false);
                return;
            }
            String commitType = (string)comboBox1.SelectedItem;
            Boolean loopingCreateCommitStatus = loopingCreateCommit(gitPath, tempPath, tempFileUrl, commitType, startTime, endTime);
            if (!loopingCreateCommitStatus)
            {
                MessageBox.Show("commit error");
                runningStatus(false);
                return;
            }
            else {
                textBox2.AppendText("push to master" + Environment.NewLine);
                String pushStr = runGit(gitPath, "-C " + tempPath + " push origin master");
                if (!pushStr.Contains("master"))
                {
                    MessageBox.Show("push error");
                    textBox2.AppendText("push error" + Environment.NewLine);
                }
                else {
                    MessageBox.Show("ok");
                    textBox2.AppendText("push complete!!!" + Environment.NewLine);
                }
                runningStatus(false);
            }
            //
        }
        private Boolean changeFileAndCommit(String gitPath, String tempPath, String tempFileUrl, String tp)
        {
            textBox2.AppendText(tp + " edit file" + Environment.NewLine);
            FileStream fs = new FileStream(tempFileUrl, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(tp);
            sw.Flush();
            sw.Close();
            fs.Close();
            String commitString = runGit(gitPath, "-C " + tempPath + " commit -a -m \"" + tp + "\"'");
            if (!commitString.Contains("changed"))
            {
                textBox2.AppendText(tp + " commit error" + Environment.NewLine);
                return false;
            }
            else
            {
                textBox2.AppendText(tp + commitString + Environment.NewLine);
                return true;
            }
        }
        private Boolean loopingCreateCommit(String gitPath, String tempPath, String tempFileUrl, String commitType, DateTime startTime, DateTime endTime)
        {
            TimeSpan ts = endTime - startTime;
            int sub = ts.Days;
            textBox2.AppendText(startTime + "-" + endTime + Environment.NewLine);
            textBox2.AppendText("difference " + sub + " days" + Environment.NewLine);
            for (int i = 0; i < sub; i++)
            {
                //设置时间
                startTime = startTime.AddDays(1);
                setDate(startTime.Year, startTime.Month, startTime.Day);
                if (commitType.Equals("orderliness"))
                {
                    if (!changeFileAndCommit(gitPath, tempPath, tempFileUrl, startTime.ToShortDateString()))
                    {
                        return false;
                    }
                }
                else
                {
                    int rd = new Random().Next(1, 20);
                    for (int a = 0; a < sub; a++)
                    {
                        if (!changeFileAndCommit(gitPath, tempPath, tempFileUrl, startTime.ToShortDateString() + a))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        private String gitClone(String gitPath, String cloneUrl, String tempPath)
        {
            return gitClone(gitPath, cloneUrl, tempPath, false);
        }
        private String gitClone(String gitPath, String cloneUrl, String tempPath, bool ag)
        {
            String gitCloneStr = runGit(gitPath, "clone " + @cloneUrl + " " + @tempPath);
            if (gitCloneStr.Contains("fatal"))
            {
                if (gitCloneStr.Contains("already exists and is not an empty directory") && !ag)
                {
                    try
                    {
                        Directory.Delete(tempPath + @"\.git", true);
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        MessageBox.Show("No folder permissions");
                        textBox2.AppendText("No folder permissions");
                        return "error";
                    }
                    return gitClone(gitPath, cloneUrl, tempPath, true);
                }
                MessageBox.Show(gitCloneStr);
                textBox2.AppendText("clone error:" + gitCloneStr + Environment.NewLine);
                return "error";
            }
            return gitCloneStr;
        }

        private void label7_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/liaozixu");
        }

        private void label9_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://liaozixu.com/");
        }

        private void label8_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/liaozixu/githubGreen-csharp");
        }
    }
}
