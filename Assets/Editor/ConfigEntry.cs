using System;

namespace Assets.Editor
{
	[Serializable]
	public sealed class ConfigEntry
	{
		public string DisplayName;
		public string DefaultAssetPath; // e.g. Assets/Configs/Player/player_motor.json
		public string TypeName; // Assembly-qualified name

		public ConfigEntry(string displayName, string defaultAssetPath, Type type)
		{
			DisplayName = displayName;
			DefaultAssetPath = defaultAssetPath;
			TypeName = type.AssemblyQualifiedName;
		}
	}
}
