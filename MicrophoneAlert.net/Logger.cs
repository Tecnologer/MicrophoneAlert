using System;
using System.IO;

namespace MicrophoneAlert.net
{
    internal class Logger
    {
        private readonly string path;
        public Logger()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            appDataFolder = $"{appDataFolder}\\tecnologer\\MicrophoneAlert";

            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }
            path = $"{appDataFolder}\\logger.log";
        }

        internal void Error(Exception ex, string v)
        {
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine($"{DateTime.Now} \tFirst start");
                }
            }

            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine($"\n{DateTime.Now} \t{v}");
                sw.WriteLine($"\n{DateTime.Now} \t{ex.Message}");
                sw.WriteLine($"\n{DateTime.Now} \t{ex.StackTrace}");
            }
        }
    }
}