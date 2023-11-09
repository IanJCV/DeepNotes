using Newtonsoft.Json;
using System.Globalization;
using System.IO.Compression;

namespace Updater
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool needsUpdate = NeedsUpdate();
            if (needsUpdate)
            {
                Console.WriteLine("Updating...");

                using HttpClient http = new();
                var result = http.GetAsync("https://github.com/IanJCV/DeepNotes/releases/download/latest/deepnotes.zip").Result;

                using Stream stream = result.Content.ReadAsStreamAsync().Result;

                string filePath = "latest.zip";

                using FileStream fileStream = new(filePath, FileMode.Create, FileAccess.Write);
                stream.CopyTo(fileStream);

                string tempPath = Path.GetTempPath();
                
                ZipFile.ExtractToDirectory(filePath, tempPath);

                foreach (var file in Directory.EnumerateFiles(tempPath))
                {
                    File.Copy(file, Path.Combine(AppContext.BaseDirectory, Path.GetFileName(file)), true);
                }

                Directory.Delete(tempPath, true);
                File.Delete(filePath);

                Console.WriteLine("Update complete.");
            }
            else
            {
                Console.WriteLine("No update necessary.");
            }

        }

        private static bool NeedsUpdate()
        {
            var time = GetTime().Result;
            Console.WriteLine(time.Message);

            if (time.Succeeded)
            {
                return time.Time > GetLastLocalUpdate();
            }
            else
            {
                return false;
            }
        }

        private static DateTime GetLastLocalUpdate()
        {
            long time = GetInfo().latestUpdate;
            return DateTime.FromBinary(time);
        }

        private static UpdateInfo GetInfo()
        {
            UpdateInfo info;
            if (File.Exists("./updateinfo.dat"))
            {
                info = JsonConvert.DeserializeObject<UpdateInfo>(File.ReadAllText("./updateinfo.dat"));
            }
            else
            {
                info = new UpdateInfo(DateTime.Now.ToBinary());
                WriteInfo(info);
            }
            return info;
        }

        private static void WriteInfo(UpdateInfo info)
        {
            File.WriteAllText("./updateinfo.dat", JsonConvert.SerializeObject(info));
        }

        private static async Task<OptionalTime> GetTime()
        {
            using HttpClient http = new();

            http.DefaultRequestHeaders.UserAgent.ParseAdd("DeepNotesUpdate");

            var commits = await http.GetAsync("https://api.github.com/repos/IanJCV/DeepNotes/commits");

            if (commits.IsSuccessStatusCode)
            {
                dynamic content = JsonConvert.DeserializeObject(await commits.Content.ReadAsStringAsync());

                CultureInfo provider = CultureInfo.InvariantCulture;

                try
                {
                    string dateTime = (string)content[0].commit.committer.date;
                    DateTime result = DateTime.ParseExact(dateTime, "dd/MM/yyyy HH:mm:ss", provider);

                    return new OptionalTime(true, "Success", result);
                }
                catch (Exception e)
                {
                    return new OptionalTime(false, e.Message, DateTime.Now);
                }
            }
            else
            {
                return new OptionalTime(false, $"Failed to retrieve the latest update time. Error: {(int)commits.StatusCode}/{commits.ReasonPhrase}", DateTime.Now);
            }
        }
    }

    internal struct UpdateInfo
    {
        public readonly long latestUpdate;

        public UpdateInfo(long latestUpdate)
        {
            this.latestUpdate = latestUpdate;
        }
    }

    internal struct OptionalTime
    {
        public readonly bool Succeeded;
        public readonly string Message;
        public readonly DateTime Time;

        public OptionalTime(bool succeeded, string message, DateTime time)
        {
            Succeeded = succeeded;
            Message = message;
            Time = time;
        }
    }
}