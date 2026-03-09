using System.Collections.Generic;

namespace Assets.Scripts.Game.Characters.Core.Player.Outputs
{
	public enum DebugLogLevel
	{
		Trace,
		Info,
		Warn,
		Error,
	}

	public readonly struct DebugLogCmd
	{
		public readonly DebugLogLevel Level;
		public readonly string Category;
		public readonly string Message;

		public DebugLogCmd(DebugLogLevel level, string category, string message)
		{
			Level = level;
			Category = category;
			Message = message;
		}
	}

	public sealed class DebugLogCommands
	{
		private readonly List<DebugLogCmd> _items = new();

		public IReadOnlyList<DebugLogCmd> Items => _items;

		public void Add(DebugLogLevel level, string category, string message) =>
			_items.Add(new DebugLogCmd(level, category, message));

		public void Trace(string category, string message) =>
			Add(DebugLogLevel.Trace, category, message);

		public void Info(string category, string message) =>
			Add(DebugLogLevel.Info, category, message);

		public void Warn(string category, string message) =>
			Add(DebugLogLevel.Warn, category, message);

		public void Error(string category, string message) =>
			Add(DebugLogLevel.Error, category, message);

		public void Clear() => _items.Clear();
	}
}
