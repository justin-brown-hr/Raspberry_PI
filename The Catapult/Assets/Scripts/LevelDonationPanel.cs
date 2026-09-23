using Logic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelDonationPanel : MonoBehaviour
{
	public GameObject mainPanel;

	public Button watchAd;

	public GameObject watchAdBlocker;

	private bool isBlocked;

	public void OpenPanel()
	{
		if (!GlobalLogic.instance.isGameEnded && !isBlocked)
		{
			Time.timeScale = 0f;
			SoundMgr.instance.PauseGame();
			InitController.instance.guiPressed = true;
			GlobalLogic.instance.isPaused = true;
			CheckAdButton();
			mainPanel.SetActive(value: false);
			base.gameObject.SetActive(value: true);
		}
	}

	public void BlockPanel()
	{
		isBlocked = true;
	}

	public void UnBlockPanel()
	{
		isBlocked = false;
	}

	public void ClosePanel()
	{
		SoundMgr.instance.UnPauseGame();
		InitController.instance.guiPressed = false;
		GlobalLogic.instance.isPaused = false;
		Time.timeScale = 1f;
		GameMenuControl.instance.ResumeGame();
		mainPanel.SetActive(value: true);
		base.gameObject.SetActive(value: false);
	}

	public void CheckAdButton()
	{
		if (SceneManager.GetActiveScene().name == "GameScene")
		{
			if (Start_Admob.Instance.RewardVideoIsLoaded())
			{
				UnityEngine.Debug.Log("------------ Ingame Button Show Reward Video Enabled ---------");
				watchAd.enabled = true;
				watchAdBlocker.SetActive(value: false);
			}
			else
			{
				UnityEngine.Debug.Log("------------ Ingame Button Show Reward Video Disabled  ---------");
				watchAd.enabled = false;
				watchAdBlocker.SetActive(value: true);
			}
		}
	}

	public void RecieveAdReward()
	{
		NewDataController.instance.AddMoney(100);
		NewDataController.instance.SavePlayerMoney();
		GameMenuControl.instance.RefreshCoins();
	}

	public void WatchAnAd()
	{
		Start_Admob.Instance.ShowRewardVideo();
		My_GoogleAnalytics.Instance.Click_Show_Rewaerd_Video();
	}

	public void Buy1000Coins()
	{
		// PJ Unity_PurchaiseIAP.Instance.Buy_1000coins();
	}

	public void Buy5000Coins()
	{
		// PJ Unity_PurchaiseIAP.Instance.Buy_5000coins();
	}

	public void Buy15000Coins()
	{
		// PJ Unity_PurchaiseIAP.Instance.Buy_15000coins();
	}

	public void Buy100000Coins()
	{
		// PJ Unity_PurchaiseIAP.Instance.Buy_100000coins();
	}
}
