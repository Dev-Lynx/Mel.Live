using Mel.Live.Extensions.Encryption;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Extensions.Configuration
{
    #region Extensions
    public static class ProtectedConfigProviderExtensions
    {
        public static IConfigurationBuilder AddProtectedProvider(this IConfigurationBuilder builder, string path, string cipher)
        {
            return AddProtectedProvider(builder, provider: null, path: path, optional: false, reloadOnChange: false, cipher: cipher);
        }

        public static IConfigurationBuilder AddProtectedProvider(this IConfigurationBuilder builder, string path, bool optional, string cipher)
        {
            return AddProtectedProvider(builder, provider: null, path: path, optional: optional, reloadOnChange: false, cipher: cipher);
        }

        public static IConfigurationBuilder AddProtectedProvider(this IConfigurationBuilder builder, string path, bool optional, bool reloadOnChange, string cipher)
        {
            return AddProtectedProvider(builder, provider: null, path: path, optional: optional, reloadOnChange: reloadOnChange, cipher: cipher);
        }

        public static IConfigurationBuilder AddProtectedProvider(this IConfigurationBuilder builder, IFileProvider provider, string path, bool optional, bool reloadOnChange, string cipher)
        {
            if (provider == null && Path.IsPathRooted(path))
            {
                provider = new PhysicalFileProvider(Path.GetDirectoryName(path));
                path = Path.GetFileName(path);
            }
            var source = new ProtectedConfigurationSource
            {
                FileProvider = provider,
                Path = path,
                Optional = optional,
                ReloadOnChange = reloadOnChange,
                Cipher = cipher
            };
            builder.Add(source);
            return builder;
        }
    }
    #endregion

    #region Source
    public class ProtectedConfigurationSource : FileConfigurationSource
    {
        public string Cipher { private get; set; }

        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            FileProvider = FileProvider ?? builder.GetFileProvider();
            return new ProtectedConfigurationProvider(Cipher, this);
        }
    }
    #endregion

    #region Provider
    public class ProtectedConfigurationProvider : FileConfigurationProvider
    {
        #region Properties
        string Cipher { get; set; }
        #endregion

        #region Constructors
        public ProtectedConfigurationProvider(string cipher, ProtectedConfigurationSource source) : base(source)
        {
            Cipher = cipher;
        }
        #endregion

        #region Methods
        public override void Load(Stream stream)
        {
            try
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();
                    var data = JsonConvert.DeserializeObject<IDictionary<string, string>>(json);

                    foreach (var key in data.Keys.ToArray())
                        try { data[key] = Crypt.DecryptString(data[key], Cipher); }
                        catch (Exception ex) { Core.Log.Error($"Error decrypting data\n{ex}"); }

                    Data = data;
                }
            }
            catch (Exception ex)
            {
                Core.Log.Debug($"Failed to load protected configuration.\n{ex}");
            }
        }

        public static string Protect(Stream stream, string cipher)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                string json = reader.ReadToEnd();
                var data = JsonConvert.DeserializeObject<IDictionary<string, string>>(json);

                foreach (var key in data.Keys.ToArray())
                    try { data[key] = Crypt.EncryptString(data[key], cipher); }
                    catch (Exception ex) { Core.Log.Error($"Error encrypting data\n{ex}"); }

                return JsonConvert.SerializeObject(data, Formatting.Indented);
            }
        }

        public override void Set(string key, string value)
        {
            base.Set(key, value);
        }

        public override bool TryGet(string key, out string value)
        {
            bool success = base.TryGet(key, out value);
            return success;
        }
        #endregion
    }
    #endregion
}
