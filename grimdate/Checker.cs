using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Web.Script.Serialization;

namespace grimdate
{
    public static class Checker
    {
        public static string _CurrentPath = System.AppDomain.CurrentDomain.BaseDirectory;
        public static string DumpFile = $@"{_CurrentPath}/checksums.txt";

        public static IDictionary<string, string> GetChecksum()
        {
            IDictionary<string, string> Checksum = new Dictionary<string, string>();

            FileStream exe, swf;
            string     hexe, hswf;
            byte[]     bexe, bswf;
            
            exe  = File.OpenRead($@"{_CurrentPath}/Grimoire.exe");
            bexe = new SHA256CryptoServiceProvider().ComputeHash(exe);
            hexe = BitConverter.ToString(bexe).Replace("-", String.Empty);

            swf  = File.OpenRead($@"{_CurrentPath}/catgirl.swf");
            bswf = new SHA256CryptoServiceProvider().ComputeHash(swf);
            hswf = BitConverter.ToString(bswf).Replace("-", String.Empty);

            Checksum.Add("exe", hexe);
            Checksum.Add("swf", hswf);

            return Checksum;
        }

        public static void DumpChecksum()
        {
            IDictionary<string, string> Checksums = GetChecksum();
            FileInfo FI = new FileInfo(DumpFile);

            if (File.Exists(DumpFile)) File.Delete(DumpFile);
            using (StreamWriter SW = FI.CreateText())
            {
                SW.WriteLine(Checksums["exe"]);
                SW.WriteLine(Checksums["swf"]);
            }
        }

        /// <summary>
        /// Index 0: Commit Code<br/>
        /// Index 1: Commit Hash<br/>
        /// </summary>
        /// <returns></returns>
        public static string[] GetCommitData()
        {
            // Won't Work if only using single-client. idk why
            WebClient Web1 = new WebClient();
            WebClient Web2 = new WebClient();

            Web1.Headers.Add("user-agent", "request");
            Web2.Headers.Add("user-agent", "request");

            var ObjLC = new JavaScriptSerializer().Deserialize<List<Commits>>(@Web1.DownloadString(Updater.CommitAPI));
            var ObjCC = new JavaScriptSerializer().Deserialize<Compares>(@Web2.DownloadString(Updater.CompareAPI + ObjLC[0].sha));

            Web1.Dispose();
            Web2.Dispose();

            return new string[] {
                "C" + ObjCC.total_commits,
                ObjLC[0].sha
            };
        }

        public static bool CheckVersion()
        {
            bool exe, swf;
            
            WebClient Web = new WebClient();
            Web.Headers.Add("user-agent", "request");

            IDictionary<string, string> LocalHash = GetChecksum();
            string[] CommitHash = Web.DownloadString(Updater.Checksum).Split(
                new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None
            );

            exe = CommitHash[0] == LocalHash["exe"];
            swf = CommitHash[1] == LocalHash["swf"];

            if (exe && swf)
                return true;

            return false;
        }

        /// <summary>
        /// 
        /// Return Integer:<br/>
        /// 0 = First Run.<br/>
        /// 1 = Latest Update.<br/>
        /// 2 = Outdated.<br/>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int CheckPanic()
        {
            if (!Directory.Exists("Libs"))
                return 0;
            else
                if (CheckVersion())
                    return 1;
                else
                    return 2;
        }
    }

    public class Commits
    {
        public string sha { get; set; }
    }

    public class Compares
    {
        public string total_commits { get; set; }
    }
}
