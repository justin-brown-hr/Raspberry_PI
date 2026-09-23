using Assets.All_Scripts;
using Logic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Unity_PurchaseButtonController : MonoBehaviour
{
	[Header("---------------- MainScene --------------")]
	public Image im_ButtonNoADS;

	public Sprite sp_MoreGames;

	[Header("---------------- PlayModeSelect --------------")]
	public Button Button_RewardON;

	public GameObject Button_RewardOFF;

	public Text Text_MoneyCount;

	private bool status_ButtonNoADS = true;

	private bool bool_ButtonPlayServiceON = true;

	public static Unity_PurchaseButtonController Instance
	{
		get;
		set;
	}

	private void Awake()
	{
		if (!Instance)
		{
			Instance = this;
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		if (SceneManager.GetActiveScene().name == "MainScene")// && !// PJ Unity_PurchaiseIAP.Instance.Status_Admob_Purchase)
		{
			im_ButtonNoADS.sprite = sp_MoreGames;
			status_ButtonNoADS = false;
		}
		State_ButtonShowReward();
	}

	public void Buttton_ShowRewardVideo()
	{
		Start_Admob.Instance.ShowRewardVideo();
		My_GoogleAnalytics.Instance.Click_Show_Rewaerd_Video();
	}

	public void Buttton_Buy1000coins()
	{
		// PJ Unity_PurchaiseIAP.Instance.Buy_1000coins();
	}

	public void Buttton_Buy5000coins()
	{
		// PJ Unity_PurchaiseIAP.Instance.Buy_5000coins();
	}

	public void Buttton_Buy15000coins()
	{
		// PJ Unity_PurchaiseIAP.Instance.Buy_15000coins();
	}

	public void Buttton_Buy100000coins()
	{
		// PJ Unity_PurchaiseIAP.Instance.Buy_100000coins();
	}

	public void Button_Buy_no_ADS()
	{
		if (status_ButtonNoADS)
		{
			// PJ Unity_PurchaiseIAP.Instance.Buy_no_ads();
		}
		else
		{
			More_Games();
		}
	}

	public void Button_MoreGames()
	{
		More_Games();
	}

	private void More_Games()
	{
		Application.OpenURL("market://search?q=pub:BYV");
	}

	public void Set_VisibleButtons(int who, bool stat)
	{
		if (who != 1 && who != 2 && who == 3)
		{
			if (SceneManager.GetActiveScene().name == "MainScene" && !stat)
			{
				im_ButtonNoADS.sprite = sp_MoreGames;
				status_ButtonNoADS = false;
			}
			PlayerPrefs.SetInt(Status_Scene.Inst.status_NoADS, 1);
			// PJ Unity_PurchaiseIAP.Instance.Inspection_NoADSButton();
			Start_Admob.Instance.DisableAdmob();
			My_GoogleAnalytics.Instance.Buy_ADS();
		}
	}

	public void State_ButtonShowReward()
	{
		if (SceneManager.GetActiveScene().name == "PlayModeSelect")
		{
			if (Start_Admob.Instance.RewardVideoIsLoaded())
			{
				UnityEngine.Debug.Log("------------ Button Show Ravard Video Eneblad ---------");
				Button_RewardON.enabled = true;
				Button_RewardOFF.SetActive(value: false);
			}
			else
			{
				UnityEngine.Debug.Log("------------ Button Show Ravard Video Disabled  ---------");
				Button_RewardON.enabled = false;
				Button_RewardOFF.SetActive(value: true);
			}
		}
	}

	public void Update_TextMoneyCount()
	{
		if (SceneManager.GetActiveScene().name == "PlayModeSelect")
		{
			Text_MoneyCount.text = string.Empty + NewDataController.instance.GetPlayerMoney();
		}
	}

	public void Button_RestorePurchases()
	{
	}

	public void Add_Co()
	{
		NewDataController.instance.AddMoney(100000);
		NewDataController.instance.SavePlayerMoney();
	}

	public void Click_Catapult2()
	{
		Application.OpenURL("market://details?id=com.byv.TheCatapult2");
	}
}
