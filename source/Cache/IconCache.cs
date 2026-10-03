using Playnite.SDK;
using Playnite.SDK.Models;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DuplicateHider.Cache
{
    class IconCache : IGeneratorCache<ImageSource, Game>, IReadOnlyDictionary<object, ImageSource>
    {
        public List<string> UserIconFolderPaths { get; set; } = new List<string>();

        public IEnumerable<object> Keys => ((IReadOnlyDictionary<object, ImageSource>)cache).Keys;

        public IEnumerable<ImageSource> Values => ((IReadOnlyDictionary<object, ImageSource>)cache).Values;

        public int Count => ((IReadOnlyCollection<KeyValuePair<object, ImageSource>>)cache).Count;

        public ImageSource this[object key] => cache.ContainsKey(key) ? ((IReadOnlyDictionary<object, ImageSource>)cache)[key] : null;

        internal ConcurrentDictionary<object, ImageSource> cache = new ConcurrentDictionary<object, ImageSource>();

        internal BitmapImage generate(Game game)
        {
            foreach (var path in GetSourceIconPaths(game))
            {
                try
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    image.UriSource = new Uri(path);
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                    image.Freeze();
                    return image;
                } catch (Exception ex)
                {
                    // BP1: Exception loggen statt stumm verschlucken
                    DuplicateHiderPlugin.logger.Warn(ex, $"Failed to load icon from '{path}'");
                }
            }

            return default(BitmapImage);
        }

        public void Clear()
        {
            cache.Clear();
        }

        public void LoadIcons(IPlayniteAPI api)
        {
            foreach(var source in api.Database.Sources)
            {
                
            }
        }

        public ImageSource GetOrGenerate(Game game)
        {
            if (game == null) return null;
            var source = game.Source ?? Constants.DEFAULT_SOURCE;
            var platform = game.Platforms?.FirstOrDefault()?.Name ?? Constants.UNDEFINED_PLATFORM;
            var key = source + platform;
            // B3: GetOrAdd ist atomar → verhindert parallele generate()-Aufrufe für denselben Key
            return cache.GetOrAdd(key, _ => generate(game));
        }

        private static readonly Dictionary<Playnite.SDK.BuiltinExtension, string> builtinToName = new Dictionary<Playnite.SDK.BuiltinExtension, string>()
        {
            {Playnite.SDK.BuiltinExtension.AmazonGamesLibrary, "amazon"          },
            {Playnite.SDK.BuiltinExtension.BattleNetLibrary,   "battle.net"      },
            {Playnite.SDK.BuiltinExtension.BethesdaLibrary,    "bethesda"        },
            {Playnite.SDK.BuiltinExtension.EpicLibrary,        "epic"            },
            {Playnite.SDK.BuiltinExtension.GogLibrary,         "gog"             },
            {Playnite.SDK.BuiltinExtension.HumbleLibrary,      "humble"          },
            {Playnite.SDK.BuiltinExtension.ItchioLibrary,      "itchio"          },
            {Playnite.SDK.BuiltinExtension.OriginLibrary,      "origin"          },
            {Playnite.SDK.BuiltinExtension.PSNLibrary,         "psn"             },
            {Playnite.SDK.BuiltinExtension.SteamLibrary,       "steam"           },
            {Playnite.SDK.BuiltinExtension.TwitchLibrary,      "twitch"          },
            {Playnite.SDK.BuiltinExtension.UplayLibrary,       "ubisoft connect" },
            {Playnite.SDK.BuiltinExtension.XboxLibrary,        "xbox"            },
        };
        

        internal static string GetResourceIconUri(Game game)
        {
            if (game != null) // BP3: war 'game is Game' — testet nur auf null bei gleichem Typ
            {
                var pluginId = game.PluginId;
                var source = game.Source?.Name ?? Constants.UNDEFINED_SOURCE;
                if (builtinToName.TryGetValue(Playnite.SDK.BuiltinExtensions.GetExtensionFromId(pluginId), out var builtinSource))
                {
                    source = builtinSource;
                }
                source = Uri.EscapeDataString(source);
                var names = GetResourceNames()
                    .Where(n => n.EndsWith(".ico", StringComparison.OrdinalIgnoreCase));
                var name = names.FirstOrDefault(n => System.IO.Path.GetFileName(n).StartsWith(source, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrEmpty(name))
                {
                    return $"pack://application:,,,/DuplicateHider;component/{name}";
                }
            }
            return null;
        }

        // O3: Assembly-Manifest einmalig einlesen und cachen – war zuvor bei jedem Aufruf teuer
        private static string[] _resourceNamesCache;

        // https://stackoverflow.com/a/2517799
        internal static string[] GetResourceNames()
        {
            if (_resourceNamesCache != null) return _resourceNamesCache;
            var asm = Assembly.GetAssembly(typeof(DuplicateHiderPlugin));
            string resName = asm.GetName().Name + ".g.resources";
            using (var stream = asm.GetManifestResourceStream(resName))
            using (var reader = new System.Resources.ResourceReader(stream))
            {
                _resourceNamesCache = reader.Cast<System.Collections.DictionaryEntry>().Select(entry => (string)entry.Key).ToArray();
            }
            return _resourceNamesCache;
        }

        protected IEnumerable<string> GetSourceIconPaths(Game game)
        {
            var name = game.Source != null ? game.Source.Name : Constants.UNDEFINED_SOURCE;
            bool enableThemeIcons = DuplicateHiderPlugin.Instance.settings.EnableThemeIcons;
            bool preferUserIcons = DuplicateHiderPlugin.Instance.settings.PreferUserIcons;

            List<string> paths = new List<string>();

            var userIconPath = GetUserIconPath(game);
            var themeIconPath = enableThemeIcons ? GetThemeIconPath(game) : null;
            var resourceIconPath = GetResourceIconUri(game);
            var pluginIconPath = GetPluginIconPath(game);
            var platformIconPath = GetPlatformIconPath(game);
            if (preferUserIcons) paths.Add(userIconPath);
            if (enableThemeIcons) paths.Add(themeIconPath);
            // #93: Platform-Icon ist spezifischer als Library/Source-Icon (z.B. Switch vs. Nintendo)
            // → Platform-Icon hat Vorrang vor resourceIconPath und pluginIconPath
            paths.Add(platformIconPath);
            paths.Add(resourceIconPath);
            if (!preferUserIcons) paths.Add(userIconPath);
            paths.Add(pluginIconPath);

            return paths.Where(p => !string.IsNullOrEmpty(p) && Uri.TryCreate(p, UriKind.RelativeOrAbsolute, out var _))
                        .Concat(GetDefaultIconPaths());
        }

        private string GetPlatformIconPath(Game game)
        {
            var path = game?.Platforms?.FirstOrDefault()?.Icon;
            if (!string.IsNullOrEmpty(path))
            {
                if (!System.IO.Path.IsPathRooted(path))
                {
                    path = DuplicateHiderPlugin.API.Database.GetFullFilePath(path);
                }
                return path;
            }
            return null;
        }

        private static string GetThemeIconPath(Game game)
        {
            var sourceName = game?.PluginId.ToString() ?? "Default";
            if (builtinToName.TryGetValue(Playnite.SDK.BuiltinExtensions.GetExtensionFromId(game?.PluginId ?? Guid.Empty), out var builtinSource))
            {
                sourceName = builtinSource;
            }
            // sourceName = Uri.EscapeDataString(sourceName);
            if (DuplicateHiderPlugin.API.Resources.GetResource($"DuplicateHider_{sourceName}_Icon") is BitmapImage img)
            {
                return img.UriSource.ToString();
            }
            if (DuplicateHiderPlugin.API.Resources.GetResource($"DuplicateHider_{sourceName.ToLower()}_Icon") is BitmapImage img2)
            {
                return img2.UriSource.ToString();
            }
            if (DuplicateHiderPlugin.API.Resources.GetResource($"DuplicateHider_{sourceName.Capitalize()}_Icon") is BitmapImage img3)
            {
                return img3.UriSource.ToString();
            }
            return null;
        }

        private string GetUserIconPath(Game game)
        {
            if (UserIconFolderPaths.Count == 0) UserIconFolderPaths.Add(DuplicateHiderPlugin.Instance.GetUserIconFolderPath());

            // #136: Check for platform-specific user icons first (e.g. "PlayStation 5.png", "PlayStation 4.png", "Switch.png")
            if (game?.Platforms != null)
            {
                foreach (var platform in game.Platforms)
                {
                    if (string.IsNullOrEmpty(platform?.Name)) continue;
                    var platformMatch = UserIconFolderPaths
                        .Where(p => System.IO.Directory.Exists(p))
                        .SelectMany(s => System.IO.Directory.GetFiles(s))
                        .Where(f => System.IO.Path.GetFileNameWithoutExtension(f).Equals(platform.Name, StringComparison.OrdinalIgnoreCase))
                        .FirstOrDefault(f =>
                            f.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                         || f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                         || f.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase)
                         || f.EndsWith(".ico", StringComparison.OrdinalIgnoreCase));
                    if (!string.IsNullOrEmpty(platformMatch))
                    {
                        return platformMatch;
                    }
                }
            }

            var sourceName = game?.Source?.Name ?? "Default";
            var paths = UserIconFolderPaths
                .Where(p => System.IO.Directory.Exists(p))
                .SelectMany(s => System.IO.Directory.GetFiles(s))
                .Where(f => System.IO.Path.GetFileNameWithoutExtension(f).Equals(sourceName, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault(f =>
                    f.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                 || f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                 || f.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase)
                 || f.EndsWith(".ico", StringComparison.OrdinalIgnoreCase));
            if ((paths?.Count() ?? 0) == 0)
            {
                if (builtinToName.TryGetValue(Playnite.SDK.BuiltinExtensions.GetExtensionFromId(game?.PluginId ?? Guid.Empty), out var builtinSource))
                {
                    sourceName = builtinSource;
                    paths = UserIconFolderPaths
                        .Where(p => System.IO.Directory.Exists(p))
                        .SelectMany(s => System.IO.Directory.GetFiles(s))
                        .Where(f => System.IO.Path.GetFileNameWithoutExtension(f).Equals(sourceName, StringComparison.OrdinalIgnoreCase))
                        .FirstOrDefault(f =>
                            f.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                         || f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                         || f.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase)
                         || f.EndsWith(".ico", StringComparison.OrdinalIgnoreCase));
                }
            }
            return paths;
        }

        protected IEnumerable<string> GetDefaultIconPaths()
        {
            bool enableThemeIcons = DuplicateHiderPlugin.Instance.settings.EnableThemeIcons;
            bool preferUserIcons = DuplicateHiderPlugin.Instance.settings.PreferUserIcons;

            List<string> paths = new List<string>();

            var userIconPath = GetUserIconPath(null);
            var themeIconPath = enableThemeIcons ? GetThemeIconPath(null) : null;
            var resourceIconPath = GetResourceIconUri(null);
            if (preferUserIcons) paths.Add(userIconPath);
            if (enableThemeIcons) paths.Add(themeIconPath);
            paths.Add(resourceIconPath);
            if (!preferUserIcons) paths.Add(userIconPath);

            paths.Add("pack://application:,,,/DuplicateHider;component/icons/playnite.ico");

            return paths.Where(p => !string.IsNullOrEmpty(p) && Uri.TryCreate(p, UriKind.RelativeOrAbsolute, out var _));
        }

        protected string GetPluginIconPath(Game game)
        {
            if (game.PluginId is Guid id)
            {
                var plugin = DuplicateHiderPlugin.API.Addons.Plugins
                    .OfType<Playnite.SDK.Plugins.LibraryPlugin>()
                    .FirstOrDefault(p => p.Id == id);

                if (plugin is Playnite.SDK.Plugins.LibraryPlugin lp)
                {
                    var path = lp.LibraryIcon;
                    if (path is string && !System.IO.Path.IsPathRooted(path))
                    {
                        var configPath = DuplicateHiderPlugin.API.Paths.ConfigurationPath;
                        path = System.IO.Path.Combine(configPath, "Extensions", plugin.Id.ToString(), path);
                    }
                    return path;
                }
            }

            return null;
        }

        public bool ContainsKey(object key)
        {
            return ((IReadOnlyDictionary<object, ImageSource>)cache).ContainsKey(key);
        }

        public bool TryGetValue(object key, out ImageSource value)
        {
            return ((IReadOnlyDictionary<object, ImageSource>)cache).TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<object, ImageSource>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<object, ImageSource>>)cache).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)cache).GetEnumerator();
        }
    }
}
