using System;
using System.Collections.Generic;
using UnityEngine;

public static class DictionaryExtensions
{
	/// <summary>
	/// Attempts to retrieve a boolean value from a dictionary.
	/// </summary>
	/// <param name="dictionary">The dictionary containing parameters.</param>
	/// <param name="key">The key to look up.</param>
	/// <returns>The boolean value if found; otherwise, false.</returns>
	public static bool GetBool(
		this Dictionary<string, object> dictionary,
		string key,
		bool defaultValue = false
	)
	{
		return
			dictionary != null
			&& dictionary.TryGetValue(key, out object value)
			&& value is bool boolValue
			? boolValue
			: defaultValue;
	}

	/// <summary>
	/// Attempts to retrieve a Vector2 value from a dictionary.
	/// </summary>
	public static Vector2 GetVector2(
		this Dictionary<string, object> dictionary,
		string key,
		Vector2 defaultValue = default
	)
	{
		return dictionary.TryGetValue(key, out object value) && value is Vector2 vector
			? vector
			: defaultValue;
	}

	/// <summary>
	/// Attempts to retrieve a float value from a dictionary.
	/// </summary>
	/// <param name="dictionary">The dictionary containing parameters.</param>
	/// <param name="key">The key to look up.</param>
	/// <param name="defaultValue">The default value if the key is not found or is not a float.</param>
	/// <returns>The float value if found and valid; otherwise, the default value.</returns>
	public static float GetFloat(
		this Dictionary<string, object> dictionary,
		string key,
		float defaultValue = 0f
	)
	{
		if (dictionary != null && dictionary.TryGetValue(key, out object value))
		{
			if (value is float floatValue)
				return floatValue;
			if (value is int intValue)
				return (float)intValue;
			if (value is double doubleValue)
				return (float)doubleValue;
		}

		return defaultValue;
	}
}
