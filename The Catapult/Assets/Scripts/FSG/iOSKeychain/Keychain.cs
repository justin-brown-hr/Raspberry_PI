using FSG.Utilities;
using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace FSG.iOSKeychain
{
	public static class Keychain
	{
		private static Crypto mCryptoInstance;

		static Keychain()
		{
			mCryptoInstance = new Crypto("oPXJN744LGH5v2pX3BVj", "KlUiCgYcoHBzB8sjYA4z");
		}

		private static string GetPath(string key)
		{
			return Path.Combine(Application.persistentDataPath, $"keychain-{key}.dat");
		}

		public static string GetValue(string key)
		{
			try
			{
				string path = GetPath(key);
				if (File.Exists(path))
				{
					byte[] input = File.ReadAllBytes(path);
					return Encoding.UTF8.GetString(mCryptoInstance.Decrypt(input));
				}
				return string.Empty;
			}
			catch (Exception exception)
			{
				UnityEngine.Debug.LogException(exception);
				return string.Empty;
			}
		}

		public static void SetValue(string key, string value)
		{
			try
			{
				string path = GetPath(key);
				File.WriteAllBytes(path, mCryptoInstance.Encrypt(Encoding.UTF8.GetBytes(value)));
			}
			catch (Exception exception)
			{
				UnityEngine.Debug.LogException(exception);
			}
		}

		public static void DeleteValue(string key)
		{
			try
			{
				string path = GetPath(key);
				if (File.Exists(path))
				{
					File.Delete(path);
				}
			}
			catch (Exception exception)
			{
				UnityEngine.Debug.LogException(exception);
			}
		}
	}
}
