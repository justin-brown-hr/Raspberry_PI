using System;
using UnityEngine;

namespace Promo
{
	[Serializable]
	public class DataAds
	{
		public string PackageName;

		public string NameGame;

		public string NameImage;

		public string urlImage;

		public string iosID;

		public int VersionImage;

		public static DataAds CreateFromJSON(string jsonString)
		{
			return JsonUtility.FromJson<DataAds>(jsonString);
		}
	}
}
