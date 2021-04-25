using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace grimdate
{
    public partial class UI : Form
    {
        private string[] args;
        private Thread TH;

        private bool TaskHardUpdate = false;
        private List<string> DownloadList = new List<string>();
        private int DownloadIndex = 0;

        private bool SoftExe = false;
        private bool SoftSwf = false;

        public UI()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            args = Environment.GetCommandLineArgs();
            InitializeComponent();

            this.Opacity = 0;
        }

        private void Updater_Load(object sender, EventArgs e)
        {
            if (args.Length > 1)
            {
                switch (args[1])
                {
                    case "-d":
                        Checker.DumpChecksum();
                        Console.WriteLine("\nChecksum Dumped.\n");
                        Application.Exit();
                        break;
                    default:
                        CheckUpdate();
                        break;
                }
            }
            else
            {
                CheckUpdate();
            }
        }

        private void CheckUpdate()
        {
            int PanicSignal = Checker.CheckPanic();

            if ( PanicSignal != 1 )
            {
                if ( PanicSignal == 0 )
                {
                    if (Directory.Exists("Libs")) Directory.Delete("Libs", true);
                    Directory.CreateDirectory("Libs");

                    if (Directory.Exists("Bots")) Directory.Delete("Libs", true);
                    Directory.CreateDirectory("Bots");

                    if (File.Exists("Grimoire.exe")) File.Delete("Grimoire.exe");
                    if (File.Exists("catgirl.swf")) File.Delete("catgirl.swf");
                }
                else
                {
                    if (Directory.Exists("tmp")) Directory.Delete("tmp", true);
                    Directory.CreateDirectory("tmp");
                }

                this.Opacity = 100;
            }

            switch (PanicSignal)
            {
                case 0:
                    HardUpdateInit();
                    break;
                case 1:
                    Application.Exit();
                    break;
                case 2:
                    DownloadList.Add("Grimoire.exe");
                    DownloadList.Add("catgirl.swf");
                    SoftUpdate();
                    break;
            }
        }

        private void KillGrimProcs()
        {
            Process[] Grimoire = Process.GetProcessesByName("Grimoire");
            if (Grimoire.Length != 0)
            {
                for (int i = 0; i < Grimoire.Length; i++)
                {
                    Grimoire[i].Kill();
                }
            }
        }

        private void ThreadedFileDownload(string URL, string path)
        {
            if (TH != null)
            {
                TH.Abort();
                TH = null;
            };

            TH = new Thread(() =>
            {
                WebClient Web = new WebClient();
                Web.Headers.Add("user-agent", "request");

                Console.WriteLine("Downloading: " + URL + "\nPath: " + path);
                Web.DownloadProgressChanged += new DownloadProgressChangedEventHandler(InProgressEvt);
                Web.DownloadFileCompleted   += new AsyncCompletedEventHandler(CompletedEvt);

                Web.DownloadFileAsync(new Uri(URL), path);
            });

            TH.Start();
        }

        private void HardUpdateInit()
        {
            TH = new Thread(() =>
            {
                WebClient Web = new WebClient();

                Web.Headers.Add("user-agent", "request");
                Web.DownloadStringCompleted += new DownloadStringCompletedEventHandler(CompletedStringEvt);

                Web.DownloadStringAsync(new Uri(Updater.Libs));
            });

            TH.Start();
        }

        private void GenerateConf()
        {
            if (File.Exists("Grimoire.exe.config")) File.Delete("Grimoire.exe.config");
            if ( TaskHardUpdate )
            {
                if (File.Exists("config.cfg")) File.Delete("config.cfg");

                // Generate user config
                FileInfo FIConf = new FileInfo("config.cfg");
                using (StreamWriter sw = FIConf.CreateText())
                {
                    sw.WriteLine(Updater.UserConf);
                    sw.Flush();
                    sw.Close();
                }
            }

            // Blocking Method. 2x Request.
            string[] CommitData = Checker.GetCommitData();

            // Generate the XML (Grimoire appConfig).
            XmlDocument GrimConf = new XmlDocument();

            GrimConf.LoadXml(Updater.GrimConf);

            XmlNode CommitCode = GrimConf.SelectSingleNode("configuration/appSettings/add[@key='commitcode']");
            XmlNode CommitHash = GrimConf.SelectSingleNode("configuration/appSettings/add[@key='commithash']");

            CommitCode.Attributes[1].Value = CommitData[0];
            CommitHash.Attributes[1].Value = CommitData[1];

            GrimConf.Save("Grimoire.exe.config");
            this.BeginInvoke((MethodInvoker)delegate
            {
                this.Text = TaskHardUpdate ? "Downloaded." : "Updated.";
                this.DownloadingGrim.Visible = false;
                this.HardUpdateDone.Visible = true;
            });
        }

        private void HardUpdate()
        {
            KillGrimProcs();

            string DownloadURL;
            string DownloadPath;

            if ( DownloadList[DownloadIndex] == "Grimoire.exe" || DownloadList[DownloadIndex] == "catgirl.swf" )
            {
                DownloadURL  = $"https://github.com/0zl/grimlite/raw/main/{DownloadList[DownloadIndex]}";
                DownloadPath = $@"{Checker._CurrentPath}/{DownloadList[DownloadIndex]}";
            }
            else
            {
                DownloadURL = $"https://github.com/0zl/grimlite/raw/main/libs/{DownloadList[DownloadIndex]}";
                DownloadPath = $@"{Checker._CurrentPath}/Libs/{DownloadList[DownloadIndex]}";
            }

            ThreadedFileDownload(DownloadURL, DownloadPath);
        }

        private void SoftUpdate()
        {
            KillGrimProcs();
            
            string DownloadURL = $"https://github.com/0zl/grimlite/raw/main/{DownloadList[DownloadIndex]}";
            this.BeginInvoke((MethodInvoker)delegate
            {
                this.CheckLabel.Visible = false;
                this.DownloadingGrim.Visible = true;
                this.ProgressBar.Style = ProgressBarStyle.Blocks;
                this.ProgressBar.Value = 0;
            });

            ThreadedFileDownload(DownloadURL, $@"{Checker._CurrentPath}/tmp/{DownloadList[DownloadIndex]}");
        }

        #region ThreadedFileDownload Events
        private void InProgressEvt(object sender, DownloadProgressChangedEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate {
                // StackOverflow Answer.
                double bytesIn = double.Parse(e.BytesReceived.ToString());
                double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                double percentage = bytesIn / totalBytes * 100;

                this.Text = $"[ {DownloadIndex + 1}/{DownloadList.Count} ]";
                this.ProgressBar.Value = (int)Math.Truncate(percentage);
            });
        }

        private void CompletedEvt(object sender, AsyncCompletedEventArgs e)
        {
            DownloadIndex++;
            if ( DownloadIndex < DownloadList.Count )
            {
                if (TaskHardUpdate)
                {
                    HardUpdate();
                    return;
                }
                else
                {
                    SoftUpdate();
                    return;
                }
            }

            if (TaskHardUpdate)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    this.Text = "Finishing..";
                    GenerateConf();
                });
            }
            else
            {
                if (File.Exists("Grimoire.exe")) File.Delete("Grimoire.exe");
                if (File.Exists("catgirl.swf")) File.Delete("catgirl.swf");

                File.Move(Checker._CurrentPath + "/tmp/Grimoire.exe", Checker._CurrentPath + "/Grimoire.exe");
                File.Move(Checker._CurrentPath + "/tmp/catgirl.swf", Checker._CurrentPath + "/catgirl.swf");
                Directory.Delete("tmp", true);

                this.BeginInvoke((MethodInvoker)delegate
                {
                    this.Text = "Finishing..";
                    GenerateConf();
                });
            }
        }

        private void CompletedStringEvt(object sender, DownloadStringCompletedEventArgs e)
        {
            string[] libs = e.Result.Split(
                new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None
            );

            for (int i = 0; i < libs.Length; i++)
            {
                DownloadList.Add(libs[i]);
            }

            DownloadList.Add("Grimoire.exe");
            DownloadList.Add("catgirl.swf");

            this.BeginInvoke((MethodInvoker)delegate {
                this.CheckLabel.Visible = false;
                this.DownloadingGrim.Visible = true;
                this.ProgressBar.Style = ProgressBarStyle.Blocks;
                this.ProgressBar.Value = 0;
            });

            TaskHardUpdate = true;
            HardUpdate();
        }
        #endregion
    }
}
