using System;
using UnityEngine;

namespace Logic
{
	public class PromoCheckingCat2 : MonoBehaviour
	{
		public static PromoCheckingCat2 instance;

		private int promoShowCountTotal;

		private int promoMaxShowCountTotal = 3;

		private int promoShowCountSession;

		private int promoMaxShowCountSession = 1;

		private string InstallCheckBundle => "com.byv.TheCatapult2";

		private string ShowingProposeSaveString => "TheCatapult2";

		private string ShowingProposeCountSaveString => "TheCatapult2Count";

		public bool ShowingAllowed
		{
			get;
			private set;
		}

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		public void Start()
		{
			ShowingAllowed = false;
			Invoke("CheckShowPermissions", 1f);
		}

		private void CheckShowPermissions()
		{
			//if (!Unity_PurchaiseIAP.Instance.Status_Admob_Purchase)
			{
				UnityEngine.Debug.Log("< -------------------- CAT 2 AD BOUGHT ---------------------->");
				ShowingAllowed = false;
				return;
			}
			if (PlayerPrefs.HasKey(ShowingProposeSaveString) && PlayerPrefs.GetInt(ShowingProposeSaveString) == 1)
			{
				UnityEngine.Debug.Log("< -------------------- CAT 2 BLOCKED ---------------------->");
				ShowingAllowed = false;
				return;
			}
			if (IsAppInstalled(InstallCheckBundle))
			{
				UnityEngine.Debug.Log("< -------------------- CAT 2 APP INSTALLED ---------------------->");
				BlockPromoPanel();
				ShowingAllowed = false;
				return;
			}
			if (PlayerPrefs.HasKey(ShowingProposeCountSaveString))
			{
				promoShowCountTotal = PlayerPrefs.GetInt(ShowingProposeCountSaveString);
				promoShowCountTotal++;
				PlayerPrefs.SetInt(ShowingProposeCountSaveString, promoShowCountTotal);
				if (promoShowCountTotal > promoMaxShowCountTotal)
				{
					UnityEngine.Debug.Log("< -------------------- CAT 2 SHOWED AT 3 SESSIONS ---------------------->");
					ShowingAllowed = false;
					BlockPromoPanel();
					return;
				}
			}
			else
			{
				PlayerPrefs.SetInt(ShowingProposeCountSaveString, 1);
				promoShowCountTotal = 1;
			}
			ShowingAllowed = true;
		}

		public void PromoOpened()
		{
			promoShowCountSession++;
			if (promoShowCountSession >= promoMaxShowCountSession)
			{
				UnityEngine.Debug.Log("< -------------------- CAT 2 SHOWED 2 TIMES ---------------------->");
				ShowingAllowed = false;
			}
		}

		private void BlockPromoPanel()
		{
			PlayerPrefs.SetInt(ShowingProposeSaveString, 1);
		}

		public bool CheckAppInstalled()
		{
			if (IsAppInstalled(InstallCheckBundle))
			{
				UnityEngine.Debug.Log("< -------------------- CAT 2 APP INSTALLED RECHECK ---------------------->");
				BlockPromoPanel();
				ShowingAllowed = false;
				return true;
			}
			return false;
		}

		private bool IsAppInstalled(string bundleID)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject androidJavaObject = @static.Call<AndroidJavaObject>("getPackageManager", new object[0]);
			UnityEngine.Debug.Log(" ********LaunchOtherApp ");
			AndroidJavaObject androidJavaObject2 = null;
			try
			{
				androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getLaunchIntentForPackage", new object[1]
				{
					bundleID
				});
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log("exception" + ex.Message);
			}
			if (androidJavaObject2 == null)
			{
				return false;
			}
			return true;
		}
	}
}
