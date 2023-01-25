using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace CheckInProject.App.Models
{
    public class Settings
    {
        public string? PasswordMD5 { get; set; }
        public bool IsFirstRun { get; set; } = false;
        public static Settings CreateSettings()
        {
            using (var settingStream = File.Open("Settings.json", FileMode.OpenOrCreate))
            {
                using (var streamReader = new StreamReader(settingStream))
                {
                    var resultText = streamReader.ReadToEnd();
                    if (!string.IsNullOrEmpty(resultText))
                    {
                        var resultSettings = JsonSerializer.Deserialize<Settings>(resultText);
                        if (resultSettings != null)
                            return resultSettings;
                        else return new Settings { IsFirstRun = true, PasswordMD5 = null };
                    }
                    else return new Settings { IsFirstRun = true, PasswordMD5 = null };
                }
            }

        }
        public async Task SaveSettings()
        {
            using (var settingStream = File.Open("Settings.json", FileMode.OpenOrCreate))
            {
                using (var streamWriter = new StreamWriter(settingStream))
                {
                    var resultText = JsonSerializer.Serialize(this);
                    await streamWriter.WriteAsync(resultText);
                }
            }
        }
    }
}
