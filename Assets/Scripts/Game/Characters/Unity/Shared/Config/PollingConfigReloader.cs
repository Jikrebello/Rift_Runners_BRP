using System;
using System.IO;
using Assets.Scripts.Game.SharedKernel.Config;

namespace Assets.Scripts.Game.Characters.Unity.Shared.Config
{
	public sealed class PollingConfigReloader<T>
		where T : class, new()
	{
		private readonly string _absolutePath;
		private DateTime _lastWriteUtc;

		public T Current { get; private set; }

		public PollingConfigReloader(string absolutePath)
		{
			if (string.IsNullOrWhiteSpace(absolutePath))
				throw new ArgumentException("Config path is null/empty.", nameof(absolutePath));

			_absolutePath = absolutePath;

			Current = JsonFileConfigLoader.Load<T>(_absolutePath);
			_lastWriteUtc = GetWriteTimeUtcSafe(_absolutePath);
		}

		public bool PollAndReloadIfChanged(out T updated)
		{
			updated = null;

			var nowWrite = GetWriteTimeUtcSafe(_absolutePath);
			if (nowWrite <= _lastWriteUtc)
				return false;

			_lastWriteUtc = nowWrite;

			if (!JsonFileConfigLoader.TryReload<T>(_absolutePath, out var cfg))
				return false;

			Current = cfg;
			updated = cfg;
			return true;
		}

		private static DateTime GetWriteTimeUtcSafe(string path)
		{
			try
			{
				return File.Exists(path) ? File.GetLastWriteTimeUtc(path) : DateTime.MinValue;
			}
			catch
			{
				return DateTime.MinValue;
			}
		}
	}
}
