using System.Collections.Concurrent;
using System.IO;
using System.Text.Json;

namespace Assets.Scripts.Game.SharedKernel.Config
{
	public static class JsonFileConfigLoader
	{
		private static readonly ConcurrentDictionary<string, object> Cache = new();

		private static readonly JsonSerializerOptions Options = new()
		{
			PropertyNameCaseInsensitive = true,
			ReadCommentHandling = JsonCommentHandling.Skip,
			AllowTrailingCommas = true,
			WriteIndented = true,
		};

		public static T Load<T>(string path)
			where T : class, new()
		{
			var fullPath = ResolvePath(path);

			if (Cache.TryGetValue(fullPath, out var cached) && cached is T typed)
				return typed;

			if (!File.Exists(fullPath))
			{
				var created = new T();
				Save(path, created);
				Cache[fullPath] = created;
				return created;
			}

			var json = File.ReadAllText(fullPath);
			var cfg = JsonSerializer.Deserialize<T>(json, Options) ?? new T();
			Cache[fullPath] = cfg;
			return cfg;
		}

		public static void Save<T>(string path, T config)
			where T : class
		{
			var fullPath = ResolvePath(path);
			Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ?? ".");
			var json = JsonSerializer.Serialize(config, Options);
			File.WriteAllText(fullPath, json);
			Cache[fullPath] = config;
		}

		public static bool TryReload<T>(string path, out T config)
			where T : class, new()
		{
			var fullPath = ResolvePath(path);
			config = null;

			if (!File.Exists(fullPath))
				return false;

			var json = File.ReadAllText(fullPath);
			config = JsonSerializer.Deserialize<T>(json, Options) ?? new T();
			Cache[fullPath] = config;
			return true;
		}

		private static string ResolvePath(string path)
		{
			// If absolute, keep it.
			if (Path.IsPathRooted(path))
				return path;

			// Resolve relative to project root (works in Editor and most dev workflows)
			return Path.Combine(Directory.GetCurrentDirectory(), path);
		}
	}
}
