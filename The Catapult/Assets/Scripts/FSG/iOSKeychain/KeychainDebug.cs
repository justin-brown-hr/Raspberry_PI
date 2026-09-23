using UnityEngine;

namespace FSG.iOSKeychain
{
	public class KeychainDebug : MonoBehaviour
	{
		private string key = "yourKeyHere";

		private string value = string.Empty;

		private void OnGUI()
		{
			GUILayout.BeginArea(new Rect(0f, 0f, Screen.width, Screen.height));
			GUILayout.Label("Key:");
			key = GUILayout.TextField(key);
			GUILayout.Label("Value:");
			value = GUILayout.TextField(value);
			if (GUILayout.Button($"Set Value For {key}", GUILayout.MinHeight(150f)))
			{
				Keychain.SetValue(key, value);
			}
			if (GUILayout.Button($"Get Value For {key}", GUILayout.MinHeight(150f)))
			{
				value = Keychain.GetValue(key);
				UnityEngine.Debug.Log($"Retrieved Value: {value}");
			}
			if (GUILayout.Button($"Delete Value For {key}", GUILayout.MinHeight(150f)))
			{
				Keychain.DeleteValue(key);
			}
			GUILayout.EndArea();
		}
	}
}
