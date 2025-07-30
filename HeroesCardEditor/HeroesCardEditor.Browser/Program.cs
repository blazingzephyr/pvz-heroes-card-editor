using System;
using System.IO;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using HeroesCardEditor;
using static System.Runtime.InteropServices.JavaScript.JSType;

public static partial class IndexedDB
{
    [JSImport("globalThis.indexedDB.databases")]
    [return: JSMarshalAs<JSType.Promise<JSType.Object>>()]
    public static partial Task<JSObject> Databases();

    [JSImport("globalThis.indexedDB.open")]
    public static partial JSObject Open(string name, [JSMarshalAs<JSType.Number>] long? version);

    [JSImport("globalThis.indexedDB.deleteDatabase")]
    public static partial JSObject Delete(
        [JSMarshalAs<JSType.String>] string name);
}

public static partial class Interop
{
    [JSImport("interop.subscribeToEvent", "interop")]
    public static partial void SubscribeToEvent(
        JSObject @object,
        string type,
        [JSMarshalAs<JSType.Function<JSType.Object>>()] Action<JSObject> listener);

    [JSImport("interop.unwrapObject", "interop")]
    public static partial JSObject[] UnwrapAsJObjectArray(JSObject @object);

    [JSImport("interop.unwrapObject", "interop")]
    public static partial string[] UnwrapAsStringArray(JSObject @object);

    [JSImport("interop.item", "interop")]
    public static partial string DOMStringListItem(JSObject container, int index);

    [JSImport("interop.contains", "interop")]
    public static partial bool DOMStringListContains(JSObject container, string item);

    [JSImport("interop.createObjectStore", "interop")]
    public static partial JSObject IDBCreateObjectStore(JSObject container, string item);

    [JSImport("interop.transaction", "interop")]
    public static partial JSObject IDBDatabaseTransaction(
        JSObject container,
        string[] item,
        string mode);

    [JSImport("interop.objectStore", "interop")]
    public static partial JSObject IDBTransactionObjectStore(JSObject container, string item);

    [JSImport("interop.add", "interop")]
    public static partial void IDBObjectStoreAdd(
        JSObject container,
        [JSMarshalAs<JSType.Any>()] object? value,
        string key);

    [JSImport("interop.get", "interop")]
    public static partial JSObject IDBObjectStoreGet(JSObject container, string key);

    [JSImport("interop.count", "interop")]
    public static partial JSObject IDBObjectStoreCount(JSObject objectStore);
}

internal sealed partial class Program
{
    private static async Task Main()
    {
        // Ensure JS ES6 module loaded
        await JSHost.ImportAsync("interop", "/interop.js");

        // Builds the app
        await BuildAvaloniaApp()
            .WithInterFont()
            .UseReactiveUI()
            .StartBrowserAppAsync("out");
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .AfterSetup(b =>
            {
                if (b.Instance is App app)
                {
                    app.OpenOrCreate = OpenOrCreate;
                }
            });

    static private int i = 0;

    /// <summary>
    /// Opens (or creates) a file in the executable's directory, then reads it and returns the contents.
    /// </summary>
    private static Task<(IStorageFile?, object?)> OpenOrCreate(IStorageProvider provider, string fileName, Type type)
    {
        object? result = Activator.CreateInstance(type);
        int props = 0;

        // change logic???

        // var dbs = await IndexedDB.Databases();
        //var databases = Interop.UnwrapAsJObjectArray(dbs);

        //var database
        //if (databases)
        // if (true/*databases.Length > 0*/)
        // {
            TaskCompletionSource<(IStorageFile?, object?)> res = new TaskCompletionSource<(IStorageFile?, object?)>();

            // IDBDatabaseInfo;
            // var databaseInfo = databases[0];
            // if (databaseInfo is null) return default;

            // var name = databaseInfo.GetPropertyAsString("name")!;
            // var version = databaseInfo.GetPropertyAsInt32("version");

            // We should open a newer 'version' of the database on every launch so 'upgradeneeded' fires and we can update it.
            // Then again, I will probably just set this to June 2025 so it creates everything ONCE.
            // var db_version = DateTimeOffset.Now.ToUnixTimeSeconds();

            var db_version = 202507*10 + i;
            i += 1;

            // IDBOpenDBRequest <-- IDBRequest;
            var request = IndexedDB.Open("HeroesCardEditor", db_version);

            Interop.SubscribeToEvent(request, "upgradeneeded", e =>
            {
                // IDBDatabase;
                var database = e.GetPropertyAsJSObject("target")!.GetPropertyAsJSObject("result");
                var error = e.GetPropertyAsJSObject("target")!.GetPropertyAsJSObject("error");

                if (database is null || error is not null) return;

                var name = database.GetPropertyAsString("name")!;
                var version = database.GetPropertyAsInt32("version")!;
                var objectStoreNames = database.GetPropertyAsJSObject("objectStoreNames")!;

                if (!Interop.DOMStringListContains(objectStoreNames, fileName))
                {
                    Console.WriteLine("Upgrading DB");

                    // IDBObjectStore
                    var objectStore = Interop.IDBCreateObjectStore(database, fileName);

                    foreach (var property in type.GetProperties())
                    {
                        var value = property.GetValue(result);
                        Console.WriteLine($"Prop {property.Name} {value}");

                        Interop.IDBObjectStoreAdd(objectStore, value, property.Name);
                    }
                }
            });

            Interop.SubscribeToEvent(request, "success", e =>
            {
                // IDBDatabase
                var database = e.GetPropertyAsJSObject("target")!.GetPropertyAsJSObject("result");
                var error = e.GetPropertyAsJSObject("target")!.GetPropertyAsJSObject("error");

                if (database is null || error is not null) return;
                
                // IDBTransaction
                var transaction = Interop.IDBDatabaseTransaction(database, [fileName], "readwrite");
                var objectStoreNames = transaction.GetPropertyAsJSObject("objectStoreNames")!;

                // IDBObjectStore
                var objectStore = Interop.IDBTransactionObjectStore(transaction, fileName);

                // IDBRequest
                // var countReq = Interop.IDBObjectStoreCount(objectStore);
                // Interop.SubscribeToEvent(countReq, "success", e =>
                // {
                //     Console.WriteLine($"SIZE {objectStore} {e.GetPropertyAsInt32("result")} " +
                //        $"{e.GetPropertyAsJSObject("target")} {e.GetPropertyAsJSObject("target").GetPropertyAsInt32("result")}");
                // });

                foreach (var property in type.GetProperties())
                {
                    Console.WriteLine($"PP {property.Name}");

                    // IDBRequest
                    var getter = Interop.IDBObjectStoreGet(objectStore, property.Name);
                    props += 1;

                    Interop.SubscribeToEvent(getter, "success", e =>
                    {
                        var target = e.GetPropertyAsJSObject("target")!;
                        object? val = null;

                        if (property.PropertyType == typeof(bool))
                        {
                            val = target.GetPropertyAsBoolean("result");
                        }
                        else if (property.PropertyType == typeof(string))
                        {
                            val = target.GetPropertyAsString("result");
                        }
                        else if (property.PropertyType == typeof(Uri))
                        {
                            val = new Uri(target.GetPropertyAsString("result"));
                        }
                        else if (property.PropertyType == typeof(DateTime))
                        {
                            val = DateTime.Parse(target.GetPropertyAsString("result"));
                        }

                        Console.WriteLine($"{property.Name} {val}");

                        props -= 1;
                        if (props == 0)
                        {
                            res.SetResult((null, result));
                        }
                    });
                }
            });

            // console found the only database called 'database' with version 1.
            // Console.WriteLine($"{result} {result == null}");
            return res.Task;
    }
}