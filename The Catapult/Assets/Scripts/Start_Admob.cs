using Assets.All_Scripts;
using com.F4A.MobileThird;
using Logic;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Start_Admob : MonoBehaviour
{
	public static Start_Admob Instance;

	private int TimerShowInter = 130;

	[Header("---------------- Android --------------")]
	private string id_banner_android = string.Empty;

	private string id_inter_android = "ca-app-pub-3485084167738450/1337069697";

	private string id_android_reward = "ca-app-pub-3485084167738450/7628608314";

	[Header("------------------ IOS ---------------")]
	private string id_banner_ios = string.Empty;

	private string id_inter_ios = "ca-app-pub-3485084167738450/9960694975";

	private string id_ios_reward = "ca-app-pub-3485084167738450/9577551598";

	private string adUnitId;

	private string interID;

	private string adUnitId_reward;

	private bool iner_is_Loaded;

	public static float m_Time;

	[SerializeField]
	private bool ShowBanner;

	protected bool AdmobActive = true;

	private bool status_ShowSmallBanner;

	private void Awake()
	{
		if (!Instance)
		{
			Instance = this;
			UnityEngine.Object.DontDestroyOnLoad(this);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

    private void OnEnable()
    {
        AdsManager.OnRewardedAdCompleted += AdsManager_OnRewardedAdCompleted;
        AdsManager.OnRewardedAdFailed += AdsManager_OnRewardedAdFailed;
        AdsManager.OnRewardedAdSkiped += AdsManager_OnRewardedAdSkiped;
        AdsManager.OnRewardedAdLoaded += AdsManager_OnRewardedAdLoaded;
    }

    private void AdsManager_OnRewardedAdLoaded(ERewardedAdNetwork adNetwork)
    {
        UnityEngine.Debug.Log("<<<<<<<<<<<<<<<<<<<<<<  РЕВАРТЕД ВИДЕО ЗАГРУЖЕНО >>>>>>>>>>>>>>>>>>>>>>>>>> ");
        if (SceneManager.GetActiveScene().name == "PlayModeSelect")
        {
            Unity_PurchaseButtonController.Instance.State_ButtonShowReward();
        }
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            GameMenuControl.instance.Activation_ButtonRewartd();
            GameMenuControl.instance.donations.CheckAdButton();
        }
    }

    private void AdsManager_OnRewardedAdSkiped(ERewardedAdNetwork adNetwork)
    {
        Activation_Sound_Backround(stat: true);
        if (SceneManager.GetActiveScene().name == "PlayModeSelect")
        {
            Unity_PurchaseButtonController.Instance.State_ButtonShowReward();
        }
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            GameMenuControl.instance.Activation_ButtonRewartd();
            GameMenuControl.instance.donations.CheckAdButton();
        }
    }

    private void AdsManager_OnRewardedAdFailed(ERewardedAdNetwork adNetwork, string error)
    {
    }

    private void AdsManager_OnRewardedAdCompleted(ERewardedAdNetwork adNetwork, string adName, double value)
    {
        Activation_Sound_Backround(stat: true);
        if (SceneManager.GetActiveScene().name == "PlayModeSelect")
        {
            NewDataController.instance.AddMoney(100);
            NewDataController.instance.SavePlayerMoney();
            AchievementManager.instance.AchievementProgress(AchieventType.WatchAnAdd, AchievementRegion.AllGame, 1);
            Unity_PurchaseButtonController.Instance.State_ButtonShowReward();
            Unity_PurchaseButtonController.Instance.Update_TextMoneyCount();
        }
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            if (GlobalLogic.instance.isGameEnded)
            {
                GameMenuControl.instance.AddCoinsAfterGame();
                GameMenuControl.instance.Activation_ButtonRewartd();
            }
            else if (GameMenuControl.instance.donations != null)
            {
                GameMenuControl.instance.donations.RecieveAdReward();
                GameMenuControl.instance.donations.CheckAdButton();
            }
        }
    }

    private void OnDisable()
    {
        AdsManager.OnRewardedAdCompleted -= AdsManager_OnRewardedAdCompleted;
        AdsManager.OnRewardedAdFailed -= AdsManager_OnRewardedAdFailed;
        AdsManager.OnRewardedAdSkiped -= AdsManager_OnRewardedAdSkiped;
    }

    private void Start_Go()
	{
	//	if (// PJ Unity_PurchaiseIAP.Instance.Status_Admob_Purchase)
		{
			Invoke("RequestInterstitial_And_Load", 2f);
			m_Time = Time.time;
			UnityEngine.Debug.Log("< -------------------- РЕКЛАМА  НЕ  КУПЛЕННА ---------------------->");
		}
	//	else
		{
			AdmobActive = false;
			UnityEngine.Debug.Log("< -------------------- РЕКЛАМА КУПЛЕННА ---------------------->");
		}
	}

	public void DisableAdmob()
	{
		AdmobActive = false;
		Hide_Small_Baner();
	}

	public void ShowInter_time()
	{
		if (AdmobActive && AdsManager.Instance.IsInterstitialAdsReady() )//&& // PJ Unity_PurchaiseIAP.Instance.Status_Admob_Purchase)
		{
			if (m_Time + (float)TimerShowInter < Time.time)
			{
				m_Time = Time.time;
				Activation_Sound_Backround(stat: false);
				AdsManager.Instance.ShowInterstitialAds();
				if (Status_Scene.Inst.ADS_first_enter)
				{
					Status_Scene.Inst.ADS_first_enter = false;
				}
			}
			else
			{
				BYV_ScenesLoader.Instance.Forced_LoadScene(Status_Scene.Inst.scene_forLoad);
			}
		}
		else
		{
			BYV_ScenesLoader.Instance.Forced_LoadScene(Status_Scene.Inst.scene_forLoad);
		}
	}

	public void Show_Inter_default()
	{
		if (AdsManager.Instance.IsInterstitialAdsReady())// && // PJ Unity_PurchaiseIAP.Instance.Status_Admob_Purchase)
		{
			Activation_Sound_Backround(stat: false);
			AdsManager.Instance.ShowInterstitialAds();
			if (Status_Scene.Inst.ADS_first_enter)
			{
				Status_Scene.Inst.ADS_first_enter = false;
			}
		}
		else
		{
			BYV_ScenesLoader.Instance.Forced_LoadScene(Status_Scene.Inst.scene_forLoad);
		}
	}

	public void TEST_Show_Inter_default()
	{
		if (AdmobActive && AdsManager.Instance.IsInterstitialAdsReady()) //&& // PJ Unity_PurchaiseIAP.Instance.Status_Admob_Purchase)
		{
			Activation_Sound_Backround(stat: false);
			AdsManager.Instance.ShowInterstitialAds();
			m_Time = Time.time;
			if (Status_Scene.Inst.ADS_first_enter)
			{
				Status_Scene.Inst.ADS_first_enter = false;
			}
		}
	}

	public void TEST_Inter_Is_Loaded()
	{
	}

	public void Request_Laod_new_Inter()
	{
		Time.timeScale = 1f;
		UnityEngine.Debug.Log("---> ЗАПРОС НА РЕКВЕСТ ИНТЕРА !! 3 <---");
		Activation_Sound_Backround(stat: true);
	}

	public void Show_Small_Banner()
	{
        AdsManager.Instance.ShowBannerAds();
    }

    public void Hide_Small_Baner()
	{
		AdsManager.Instance.HideBannerAds();
	}

	private void Activation_Sound_Backround(bool stat)
	{
	}

	public bool RewardVideoIsLoaded()
	{
		return AdsManager.Instance.IsRewardAdsReady();
	}

	public void ShowRewardVideo()
	{
		if (RewardVideoIsLoaded())
		{
			if (Status_Scene.Inst.ADS_first_enter)
			{
				Status_Scene.Inst.ADS_first_enter = false;
			}
			m_Time = Time.time;
			Activation_Sound_Backround(stat: false);
			AdsManager.Instance.ShowRewardAds();
		}
	}
}
