using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;

namespace HeroesCardEditor.Desktop
{
    internal sealed class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .UseReactiveUI()
                .AfterSetup(b => {
                    if (b.Instance is App app)
                    {
                        app.OpenOrCreate = OpenOrCreate;
                    }
                })
                .LogToTrace();

        /// <summary>
        /// Opens (or creates) a file in the executable's directory, then reads it and returns the contents.
        /// </summary>
        private static async Task<(IStorageFile?, object?)> OpenOrCreate(IStorageProvider provider, string fileName, Type type)
        {
            string folderPath = Environment.CurrentDirectory;
            IStorageFile? file = await provider.TryGetFileFromPathAsync($"{folderPath}/{fileName}");
            object? result;

            if (file == null)
            {
                IStorageFolder? folder = await provider.TryGetFolderFromPathAsync(folderPath);
                if (folder == null) return default;

                file = await folder.CreateFileAsync(fileName);
                if (file == null) return default;

                Stream writeStream = await file.OpenWriteAsync();
                result = Activator.CreateInstance(type);

                JsonSerializer.Serialize(writeStream, result);
                writeStream.Close();
            }
            else
            {
                Stream readStream = await file.OpenReadAsync();
                result = JsonSerializer.Deserialize(readStream, type);
                readStream.Close();
            }

            return (file, result);
        }
    }
}
