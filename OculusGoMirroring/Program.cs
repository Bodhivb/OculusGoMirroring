using System;
using System.Diagnostics;
using System.IO;

namespace OculusGoMirroring
{
    class Program
    {
        public readonly static string VLC_FILE = "vlc.exe";

        static void Main(string[] args)
        {
            string vlc_Folder = getVLCFolder();
            if (vlc_Folder == "")
            {
                Console.WriteLine("Can't find VLC media player software. \nPlease install VLC player and try again.");
                return;
            }


            bool neededCD = false;
            string listDevice = runCommandLine("adb devices");
            if (listDevice == "")
            {
                string adb_Folder = getADBFolder();
                if (adb_Folder == "")
                {
                    Console.WriteLine("Can't find ADB. \nPlease install Android SDK (SDK Tools only) and try again.");
                    Console.ReadKey();
                    return;
                }
                listDevice = runCommandLine("cd " + getADBFolder() + " & adb devices");
                neededCD = true;
            }

            if (listDevice == "")
            {
                Console.WriteLine("There was error with ADS. \nPlease contact us for further information. You can find contact information via: http://vr-mamaproducties.com/");
                Console.ReadKey();
            }
            else if (listDevice.Replace("devices", "").Contains("device"))
            {
                runCommandLine((neededCD ? "cd " + getADBFolder() + " & " : "") + "adb exec-out \"while true; do screenrecord --bit-rate=2m --output-format=h264 --time-limit 180 -; done\" | \"" + vlc_Folder + VLC_FILE + "\" --demux h264 --h264-fps=60 --clock-jitter=0 -");
            }
            else
            {
                Console.WriteLine("USB device isn't recognized. \nMake sure that the USB cable is properly connected to the OculusGo and try again.");
                Console.ReadKey();
            }
        }

        private static string runCommandLine(string arg)
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = @"/c " + arg;
            p.StartInfo.Verb = "runas";
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return output;
        }

        private static string getVLCFolder()
        {
            string VLC_FOLDER = @"C:\Program Files (x86)\VideoLAN\VLC\";
            string VLC_FOLDER2 = @"C:\Program Files\VideoLAN\VLC\";

            if (Directory.Exists(VLC_FOLDER))
                return VLC_FOLDER;

            if (Directory.Exists(VLC_FOLDER2))
                return VLC_FOLDER2;

            return "";
        }

        private static string getADBFolder()
        {
            string ADB_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Local\Android\Sdk\platform-tools";
            string ADB_FOLDER2 = @"C:\Program Files\android-sdk-windows\platform-tools";

            if (Directory.Exists(ADB_FOLDER))
                return ADB_FOLDER;

            if (Directory.Exists(ADB_FOLDER2))
                return ADB_FOLDER2;

            return "";
        }
    }
}
