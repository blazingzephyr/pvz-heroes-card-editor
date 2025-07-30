using System;
using System.Text.Json;
using System.Threading.Tasks;
using Android.Net;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Avalonia;
using Avalonia.Android;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using Java.Util;
using Uri = Android.Net.Uri;
using System.IO;
using Environment = Android.OS.Environment;

namespace HeroesCardEditor.Android
{
    [Activity(
        Label = "HeroesCardEditor",
        Theme = "@style/MyTheme.NoActionBar",
        Icon = "@drawable/icon",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
    public class MainActivity : AvaloniaMainActivity<App>
    {
        protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
        {
            return base.CustomizeAppBuilder(builder)
                .WithInterFont()
                .UseReactiveUI()
                .AfterSetup(b => {

                    if (b.Instance is App app)
                    {
                        app.OpenOrCreate = OpenOrCreate;
                    }
                });
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent? data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok && requestCode == 69)
            {
                var uri = data.Data;
            }
        }

        /// <summary>
        /// Opens (or creates) a file in the executable's directory, then reads it and returns the contents.
        /// </summary>
        private async Task<(IStorageFile?, object?)> OpenOrCreate(IStorageProvider provider, string fileName, Type type)
        {
            object? result;
            Stream stream;

            Java.IO.File? dir = GetExternalFilesDir(string.Empty);
            Java.IO.File f = new Java.IO.File(dir, fileName);

            if (!f.Exists())
            {
                f.CreateNewFile();
                f.SetWritable(true);

                stream = OpenFileOutput(fileName, FileCreationMode.Private)!;
                result = Activator.CreateInstance(type);

                JsonSerializer.Serialize(stream, result);
            }
            else
            {
                stream = OpenFileInput(fileName)!;
                result = JsonSerializer.Deserialize(stream, type);
            }

            stream.Close();
            return (default, result);

            /*ApplicationContext.OpenFileInput(fileName);

            if (ApplicationContext?.ContentResolver is ContentResolver c)
            {
                string s = "primary:Android/data/com.DefaultCompany.TargetStuff/cache/UnityShaderCache/version";
                string url = System.Web.HttpUtility.UrlEncode(s);
                Uri u = Uri.Parse($"content://com.android.externalstorage.documents/document/{url}");
                Stream stream = c.OpenInputStream(u);
            }

            var intent = new Intent(Intent.ActionOpenDocument)
                .AddCategory(Intent.CategoryOpenable)
                .SetFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantPersistableUriPermission)
                .PutExtra(Intent.ExtraAllowMultiple, false)
                .SetType("application/*");

            StartActivityForResult(intent, 69);

            // intent.ResolveActivity(this.PackageManager)
            */
            /*
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

        return (file, result);*/

            // return (null, null);
        }
    }
}
