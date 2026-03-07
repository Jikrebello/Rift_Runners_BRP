using UnityEditor;
using UnityEngine;

namespace Assets.Editor.ConfigEditors
{
	public static class ConfigRegistryMenu
	{
		private const string RegistryPath = "Assets/Editor/ConfigEditors/ConfigRegistry.asset";

		[MenuItem("Game/Configs/Create Registry Asset")]
		public static void CreateRegistry()
		{
			var existing = AssetDatabase.LoadAssetAtPath<ConfigRegistryAsset>(RegistryPath);
			if (existing != null)
			{
				EditorUtility.DisplayDialog("Config Registry", "Registry already exists.", "OK");
				Selection.activeObject = existing;
				return;
			}

			var asset = ScriptableObject.CreateInstance<ConfigRegistryAsset>();
			AssetDatabase.CreateAsset(asset, RegistryPath);
			AssetDatabase.SaveAssets();

			Selection.activeObject = asset;
		}

		[MenuItem("Game/Configs/Open Registry Asset")]
		public static void OpenRegistry()
		{
			var asset = AssetDatabase.LoadAssetAtPath<ConfigRegistryAsset>(RegistryPath);
			if (asset == null)
			{
				EditorUtility.DisplayDialog(
					"Config Registry",
					"No registry found. Create one first: Game/Configs/Create Registry Asset",
					"OK"
				);
				return;
			}

			Selection.activeObject = asset;
		}

		public static ConfigRegistryAsset LoadRegistryOrNull()
		{
			return AssetDatabase.LoadAssetAtPath<ConfigRegistryAsset>(RegistryPath);
		}
	}
}
