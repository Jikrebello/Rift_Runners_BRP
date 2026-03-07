using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.ConfigEditors
{
	public sealed class JsonConfigEditorWindow : EditorWindow
	{
		private ConfigRegistryAsset _registry;

		private int _selectedIndex;
		private string _assetPath;
		private object _configObject;
		private Type _configType;

		private Vector2 _scroll;
		private string _lastError;

		[MenuItem("Game/Configs/Open Config Editor")]
		public static void Open()
		{
			GetWindow<JsonConfigEditorWindow>("JSON Config Editor");
		}

		private void OnEnable()
		{
			_registry = ConfigRegistryMenu.LoadRegistryOrNull();
			if (_registry != null && _registry.Entries.Count > 0)
			{
				_selectedIndex = Mathf.Clamp(_selectedIndex, 0, _registry.Entries.Count - 1);
				_assetPath = _registry.Entries[_selectedIndex].DefaultAssetPath;
				LoadSelectedType();
				Load();
			}
		}

		private void OnGUI()
		{
			EditorGUILayout.LabelField(
				"JSON Config Editor (JSON is source of truth)",
				EditorStyles.boldLabel
			);

			_registry = (ConfigRegistryAsset)
				EditorGUILayout.ObjectField(
					"Registry",
					_registry,
					typeof(ConfigRegistryAsset),
					false
				);

			if (_registry == null)
			{
				EditorGUILayout.HelpBox(
					"No registry assigned. Create one: Game/Configs/Create Registry Asset",
					MessageType.Warning
				);
				return;
			}

			if (_registry.Entries == null || _registry.Entries.Count == 0)
			{
				EditorGUILayout.HelpBox(
					"Registry has no entries. Select the registry asset and add entries (DisplayName, Path, TypeName).",
					MessageType.Info
				);
				return;
			}

			DrawEntryPicker();

			DrawPathRow();

			DrawButtonsRow();

			if (!string.IsNullOrEmpty(_lastError))
				EditorGUILayout.HelpBox(_lastError, MessageType.Error);

			EditorGUILayout.Space();

			if (_configObject == null || _configType == null)
			{
				EditorGUILayout.HelpBox("No config loaded.", MessageType.Warning);
				return;
			}

			_scroll = EditorGUILayout.BeginScrollView(_scroll);
			DrawObjectFields(_configObject, _configType);
			EditorGUILayout.EndScrollView();
		}

		private void DrawEntryPicker()
		{
			var names = new string[_registry.Entries.Count];
			for (int i = 0; i < names.Length; i++)
				names[i] = _registry.Entries[i].DisplayName;

			var newIndex = EditorGUILayout.Popup("Config", _selectedIndex, names);
			if (newIndex != _selectedIndex)
			{
				_selectedIndex = newIndex;
				_assetPath = _registry.Entries[_selectedIndex].DefaultAssetPath;
				LoadSelectedType();
				Load();
			}
		}

		private void DrawPathRow()
		{
			using (new EditorGUILayout.HorizontalScope())
			{
				_assetPath = EditorGUILayout.TextField("Path", _assetPath);

				if (GUILayout.Button("Pick...", GUILayout.Width(70)))
				{
					var picked = EditorUtility.OpenFilePanel(
						"Select JSON config",
						Application.dataPath,
						"json"
					);
					if (!string.IsNullOrEmpty(picked))
					{
						_assetPath = ToProjectRelativeIfPossible(picked);
					}
				}
			}
		}

		private void DrawButtonsRow()
		{
			using (new EditorGUILayout.HorizontalScope())
			{
				if (GUILayout.Button("Load"))
					Load();
				if (GUILayout.Button("Save"))
					Save();
				if (GUILayout.Button("Open File"))
					OpenInDefaultEditor();
			}
		}

		private void LoadSelectedType()
		{
			_lastError = null;
			_configObject = null;
			_configType = null;

			try
			{
				var entry = _registry.Entries[_selectedIndex];
				_configType = Type.GetType(entry.TypeName, throwOnError: true);

				// Require [Serializable] so JsonUtility works.
				if (!_configType.IsSerializable)
					throw new InvalidOperationException(
						$"{_configType.FullName} is not [Serializable]."
					);

				_configObject = Activator.CreateInstance(_configType);
			}
			catch (Exception ex)
			{
				_lastError = ex.Message;
			}
		}

		private void Load()
		{
			_lastError = null;

			try
			{
				if (_configType == null)
					throw new InvalidOperationException("No config type selected/loaded.");

				var fullPath = ResolveFullPath(_assetPath);
				if (!File.Exists(fullPath))
					throw new FileNotFoundException("Config file not found", fullPath);

				var json = File.ReadAllText(fullPath);

				// JsonUtility can't deserialize into object directly; use FromJsonOverwrite
				var obj = Activator.CreateInstance(_configType);
				JsonUtility.FromJsonOverwrite(json, obj);

				_configObject = obj;
			}
			catch (Exception ex)
			{
				_lastError = ex.ToString();
			}
		}

		private void Save()
		{
			_lastError = null;

			try
			{
				if (_configObject == null)
					throw new InvalidOperationException("No config loaded.");

				var fullPath = ResolveFullPath(_assetPath);

				Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ?? ".");
				var json = JsonUtility.ToJson(_configObject, prettyPrint: true);
				File.WriteAllText(fullPath, json);

				// Reimport if under Assets/
				if (
					_assetPath
						.Replace('\\', '/')
						.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase)
				)
				{
					AssetDatabase.ImportAsset(_assetPath, ImportAssetOptions.ForceUpdate);
				}
			}
			catch (Exception ex)
			{
				_lastError = ex.ToString();
			}
		}

		private void OpenInDefaultEditor()
		{
			var fullPath = ResolveFullPath(_assetPath);
			if (File.Exists(fullPath))
				EditorUtility.OpenWithDefaultApp(fullPath);
		}

		private static void DrawObjectFields(object obj, Type type)
		{
			var fields = type.GetFields(
				System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public
			);
			for (int i = 0; i < fields.Length; i++)
			{
				var f = fields[i];
				var ft = f.FieldType;
				var label = ObjectNames.NicifyVariableName(f.Name);

				if (ft == typeof(int))
					f.SetValue(obj, EditorGUILayout.IntField(label, (int)f.GetValue(obj)));
				else if (ft == typeof(float))
					f.SetValue(obj, EditorGUILayout.FloatField(label, (float)f.GetValue(obj)));
				else if (ft == typeof(bool))
					f.SetValue(obj, EditorGUILayout.Toggle(label, (bool)f.GetValue(obj)));
				else if (ft == typeof(string))
					f.SetValue(obj, EditorGUILayout.TextField(label, (string)f.GetValue(obj)));
				else if (ft.IsEnum)
					f.SetValue(obj, EditorGUILayout.EnumPopup(label, (Enum)f.GetValue(obj)));
				else
				{
					// Nested serializable object (basic support)
					EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
					var nested = f.GetValue(obj);
					if (nested == null)
					{
						if (ft.IsClass && ft.GetConstructor(Type.EmptyTypes) != null)
						{
							nested = Activator.CreateInstance(ft);
							f.SetValue(obj, nested);
						}
						else
						{
							EditorGUILayout.HelpBox(
								$"Unsupported field: {f.Name} ({ft.Name})",
								MessageType.Info
							);
							continue;
						}
					}

					using (new EditorGUI.IndentLevelScope())
					{
						DrawObjectFields(nested, ft);
					}
				}
			}
		}

		private static string ResolveFullPath(string path)
		{
			if (Path.IsPathRooted(path))
				return path;

			var projectRoot = Directory.GetCurrentDirectory();
			return Path.Combine(projectRoot, path);
		}

		private static string ToProjectRelativeIfPossible(string absolutePath)
		{
			var abs = absolutePath.Replace('\\', '/');
			var data = Application.dataPath.Replace('\\', '/');

			if (abs.StartsWith(data, StringComparison.OrdinalIgnoreCase))
				return "Assets" + abs.Substring(data.Length);

			return absolutePath;
		}
	}
}
