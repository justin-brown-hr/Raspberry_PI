using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Promo
{
	public class PromoModule : MonoBehaviour
	{
		[Serializable]
		public class fields
		{
			public string PackageName;

			public string NameGame;

			public string NameImage;

			public int VersionImage;

			public string urlImage;

			public string iosID;
		}

		[Serializable]
		public class RootObject
		{
			public List<fields> fields;
		}

		public static PromoModule instance;

		public GameObject PromoCanvas;

		[Header("в какой сцене показывать модуль")]
		public string sceneName;

		public string UrlJson;

		private string packageName;

		private string nameImage;

		private string nameGame;

		private string urlImage;

		private string iosID;

		private int versionImage;

		private DateTime targetTime;

		private TimeSpan timeLeft;

		private long temp;

		public RootObject myPlayerStatsList = new RootObject();

		public int countConectionToServer;

		private void Awake()
		{
			if (!PlayerPrefs.HasKey("DatePromo"))
			{
				timeLeft = TimeSpan.Zero;
			}
			else
			{
				temp = Convert.ToInt64(PlayerPrefs.GetString("DatePromo"));
				timeLeft = DateTime.FromBinary(temp) - DateTime.Now;
			}
			if (!instance)
			{
				instance = this;
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
				StartCoroutine(ConnectToDataBase());
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(base.gameObject);
			}
		}

		private void OnLevelWasLoaded(int level)
		{
			if (level == SceneManager.GetSceneByName(sceneName).buildIndex)
			{
				//if (Unity_PurchaiseIAP.Instance.Status_Admob_Purchase)
				{
					PromoCanvas.SetActive(value: true);
				}
			}
			else
			{
				PromoCanvas.SetActive(value: false);
			}
		}

		private bool CheckClosed()
		{
			if (!PlayerPrefs.HasKey("close"))
			{
				PlayerPrefs.SetString("close", "no");
				return false;
			}
			if (PlayerPrefs.GetString("close") == "no")
			{
				return false;
			}
			if (PlayerPrefs.GetString("close") == "yes")
			{
				return true;
			}
			return false;
		}

		public void DownloadImage(string nameImage, string packageName, string nameGame, string urlImage, string iosID, bool isSave)
		{
			if (!CheckClosed())
			{
				string absoluteImagePath = Application.persistentDataPath + "/" + nameImage;
				StartCoroutine(downloadIMage(urlImage, absoluteImagePath, packageName, nameGame, iosID, isSave));
			}
		}

		private void saveImage(string urlImage, string absoluteImagePath, string packageName, string nameGame, string iosID, byte[] imageBytes)
		{
			string text = absoluteImagePath + ".png";
			if (!Directory.Exists(Path.GetDirectoryName(text)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(text));
			}
			try
			{
				File.WriteAllBytes(text, imageBytes);
				UnityEngine.Debug.Log("Saved Data to: " + text.Replace("/", "\\"));
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogWarning("Failed To Save Data to: " + text.Replace("/", "\\"));
				UnityEngine.Debug.LogWarning("Error: " + ex.Message);
			}
			Texture2D texture2D = new Texture2D(2, 2);
			texture2D.LoadImage(imageBytes);
			Sprite spr = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
			contentController.Instance.LoadImageToPromo(spr, packageName, nameGame, iosID);
		}

		private byte[] loadImage(string urlImage, string absoluteImagePath, string packageName, string nameGame, string iosID, bool isSave)
		{
			if (CheckPressPromo(packageName))
			{
				return null;
			}
			string text = absoluteImagePath + ".png";
			byte[] array = null;
			if (!Directory.Exists(Path.GetDirectoryName(text)))
			{
				UnityEngine.Debug.LogWarning("Directory does not exist");
				return null;
			}
			if (!File.Exists(text))
			{
				return null;
			}
			try
			{
				array = File.ReadAllBytes(text);
				UnityEngine.Debug.Log("Loaded Data from: " + text.Replace("/", "\\"));
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogWarning("Failed To Load Data from: " + text.Replace("/", "\\"));
				UnityEngine.Debug.LogWarning("Error: " + ex.Message);
			}
			if (isSave)
			{
				return null;
			}
			Texture2D texture2D = new Texture2D(2, 2);
			texture2D.LoadImage(array);
			Sprite spr = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
			contentController.Instance.LoadImageToPromo(spr, packageName, nameGame, iosID);
			return array;
		}

		private IEnumerator ReadJson()
		{
			string url = "file://" + Application.persistentDataPath + "/Promo.json";
			WWW www = new WWW(url);
			yield return www;
			if (www.error == null)
			{
				byte[] bytes = www.bytes;
				Processjson(Encoding.Default.GetString(bytes));
			}
		}

		public IEnumerator downloadIMage(string urlImage, string absoluteImagePath, string packageName, string nameGame, string iosID, bool isSave)
		{
			if (loadImage(urlImage, absoluteImagePath, packageName, nameGame, iosID, isSave) == null || isSave)
			{
				WWW www = new WWW(urlImage);
				yield return www;
				saveImage(urlImage, absoluteImagePath, packageName, nameGame, iosID, www.bytes);
			}
		}

		public IEnumerator ConnectToDataBase(bool permit = false)
		{
			if (countConectionToServer >= 2)
			{
				yield break;
			}
			countConectionToServer++;
			if (timeLeft <= TimeSpan.Zero || permit)
			{
				string url = UrlJson;
				WWW www = new WWW(url);
				yield return www;
				if (www.error == null)
				{
					string text = Application.persistentDataPath + "/Promo.json";
					if (!Directory.Exists(Path.GetDirectoryName(text)))
					{
						Directory.CreateDirectory(Path.GetDirectoryName(text));
					}
					try
					{
						File.WriteAllBytes(text, www.bytes);
						UnityEngine.Debug.Log("Saved Data to: " + text.Replace("/", "\\"));
					}
					catch (Exception ex)
					{
						UnityEngine.Debug.LogWarning("Failed To Save Data to: " + text.Replace("/", "\\"));
						UnityEngine.Debug.LogWarning("Error: " + ex.Message);
					}
					StartCoroutine(ReadJson());
					targetTime = DateTime.Now;
					targetTime = targetTime.AddDays(1.0);
					PlayerPrefs.SetString("DatePromo", targetTime.ToBinary().ToString());
				}
				else
				{
					UnityEngine.Debug.Log("ERROR: " + www.error);
					yield return new WaitForSeconds(30f);
					StartCoroutine(ConnectToDataBase());
				}
			}
			else
			{
				StartCoroutine(ReadJson());
			}
		}

		public void Processjson(string json)
		{
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				if (PromoCanvas.active)
				{
					PromoCanvas.SetActive(value: false);
				}
				return;
			}
			if (!PromoCanvas.active && SceneManager.GetActiveScene().name == "PlayModeSelect")//Pj && Unity_PurchaiseIAP.Instance.Status_Admob_Purchase)
			{
				PromoCanvas.SetActive(value: true);
			}
			else
			{
				PromoCanvas.SetActive(value: false);
			}
			JsonUtility.FromJsonOverwrite(json, myPlayerStatsList);
			foreach (fields field in myPlayerStatsList.fields)
			{
				packageName = field.PackageName;
				nameImage = field.NameGame;
				versionImage = field.VersionImage;
				nameGame = field.NameGame;
				urlImage = field.urlImage;
				iosID = field.iosID;
				if (!PlayerPrefs.HasKey(packageName + "_ImageVersion"))
				{
					if (CheckClosed())
					{
						PlayerPrefs.SetString("close", "no");
					}
					if (CheckPressPromo(packageName))
					{
						PlayerPrefs.SetString("close_" + packageName, "no");
					}
					PlayerPrefs.SetInt(packageName + "_ImageVersion", versionImage);
					DownloadImage(nameImage, packageName, nameGame, urlImage, iosID, isSave: true);
				}
				else if (PlayerPrefs.GetInt(packageName + "_ImageVersion") == versionImage)
				{
					if (!CheckPressPromo(packageName))
					{
						StartCoroutine(downloadIMage(urlImage, Application.persistentDataPath + "/" + nameImage, packageName, nameGame, iosID, isSave: false));
					}
				}
				else
				{
					if (PlayerPrefs.HasKey("close_" + packageName))
					{
						PlayerPrefs.SetString("close_" + packageName, "no");
					}
					else if (PlayerPrefs.GetString("close_" + packageName) == "yes")
					{
						PlayerPrefs.SetString("close_" + packageName, "no");
					}
					else
					{
						PlayerPrefs.SetString("close_" + packageName, "no");
					}
					PlayerPrefs.SetInt(packageName + "_ImageVersion", versionImage);
					if (CheckClosed())
					{
						PlayerPrefs.SetString("close", "no");
						Processjson(json);
						break;
					}
					DownloadImage(nameImage, packageName, nameGame, urlImage, iosID, isSave: true);
				}
			}
		}

		public IEnumerator LoadSprite(string absoluteImagePath, string packageName, string nameGame)
		{
			if (!CheckClosed() && !CheckPressPromo(packageName))
			{
				string finalPath = "file://" + absoluteImagePath;
				WWW localFile = new WWW(finalPath);
				yield return localFile;
				Texture texture = localFile.texture;
				if ((bool)texture)
				{
					Sprite sprite = Sprite.Create(texture as Texture2D, new Rect(0f, 0f, texture.width, texture.height), Vector2.zero);
					contentController.Instance.LoadImageToPromo(sprite, packageName, nameGame, iosID);
				}
			}
		}

		public void ClosePromo()
		{
			contentController.Instance.countImage = 0;
			PlayerPrefs.SetString("close", "yes");
		}

		public static void PressPromo(string packageName)
		{
			if (!PlayerPrefs.HasKey("close_" + packageName))
			{
				PlayerPrefs.SetString("close_" + packageName, "yes");
			}
			else
			{
				PlayerPrefs.SetString("close_" + packageName, "yes");
			}
		}

		public bool CheckPressPromo(string packageName)
		{
			UnityEngine.Debug.Log(PlayerPrefs.GetString("close_" + packageName));
			if (!PlayerPrefs.HasKey("close_" + packageName))
			{
				PlayerPrefs.SetString("close_" + packageName, "no");
				return false;
			}
			if (PlayerPrefs.GetString("close_" + packageName) == "no")
			{
				return false;
			}
			if (PlayerPrefs.GetString("close_" + packageName) == "yes")
			{
				return true;
			}
			return false;
		}

		public void Activate_PromoCanvas(bool stat)
		{
			PromoCanvas.SetActive(stat);
		//PJ	if (!Unity_PurchaiseIAP.Instance.Status_Admob_Purchase)
			{
				PromoCanvas.SetActive(value: false);
			}
		}
	}
}
