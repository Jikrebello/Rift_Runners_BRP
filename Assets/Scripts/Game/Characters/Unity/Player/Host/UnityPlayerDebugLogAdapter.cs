using System.Collections.Generic;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;
using UnityEngine;

namespace Assets.Scripts.Game.Characters.Unity.Player.Host
{
	public sealed class UnityPlayerDebugLogAdapter
	{
		private readonly HashSet<string> _categoryAllowlist;
		private readonly Object _context;
		private readonly int _maxPerFrame;
		private readonly DebugLogLevel _minLevel;

		public UnityPlayerDebugLogAdapter(
			Object context,
			IEnumerable<string> categoryAllowlist,
			DebugLogLevel minLevel = DebugLogLevel.Info,
			int maxPerFrame = 12
		)
		{
			_context = context;
			_categoryAllowlist = new HashSet<string>(categoryAllowlist ?? new string[0]);
			_minLevel = minLevel;
			_maxPerFrame = Mathf.Max(1, maxPerFrame);
		}

		public static HashSet<string> BuildLogCategorySet(IEnumerable<string> categories)
		{
			var set = new HashSet<string>();

			if (categories == null)
				return set;

			foreach (var category in categories)
			{
				if (!string.IsNullOrWhiteSpace(category))
				{
					set.Add(category.Trim());
				}
			}

			return set;
		}

		public void Apply(DebugLogCommands cmds)
		{
			if (cmds == null)
				return;

			int printed = 0;

			for (int i = 0; i < cmds.Items.Count; i++)
			{
				var e = cmds.Items[i];

				if (e.Level < _minLevel)
					continue;

				if (_categoryAllowlist.Count > 0 && !_categoryAllowlist.Contains(e.Category))
					continue;

				if (printed >= _maxPerFrame)
					break;

				var msg = $"[{e.Category}] {e.Message}";
				switch (e.Level)
				{
					case DebugLogLevel.Trace:
					case DebugLogLevel.Info:
						Debug.Log(msg, _context);
						break;

					case DebugLogLevel.Warn:
						Debug.LogWarning(msg, _context);
						break;

					case DebugLogLevel.Error:
						Debug.LogError(msg, _context);
						break;
				}

				printed++;
			}
		}
	}
}
