using System.Collections.Generic;
using UnityEngine;

namespace Assets.Editor.ConfigEditors
{
	public sealed class ConfigRegistryAsset : ScriptableObject
	{
		public List<ConfigEntry> Entries = new();
	}
}
