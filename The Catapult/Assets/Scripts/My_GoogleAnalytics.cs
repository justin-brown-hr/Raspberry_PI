using Assets.All_Scripts;
using SA.Analytics.Google;
using UnityEngine;
using UnityEngine.Analytics;

public class My_GoogleAnalytics : MonoBehaviour
{
	public static My_GoogleAnalytics Instance;

	private int local_indexAR;

	private void Awake()
	{
		if (!Instance)
		{
			Instance = this;
			Object.DontDestroyOnLoad(this);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		UnityEngine.Debug.Log("------ Значение переменной Analytic_bool ---> " + GetRemoteStatusAnalytic());
		// PJ 'AnalyticsEvent'.GameStart();
		if (GetRemoteStatusAnalytic() && Status_Scene.Inst.GetAcces_GoogleAnlGDPR())
		{
			GA_Manager.StartTracking();
		}
	}

	public bool GetRemoteStatusAnalytic()
	{
		return RemoteSettings.GetBool("Analytic_bool", defaultValue: true);
	}

	public bool GetRemoteStatus_SendIvent()
	{
		return RemoteSettings.GetBool("SendIvent_bool", defaultValue: true);
	}

	public void Click_Button_NO_ADS()
	{
		UnityEngine.Debug.Log("GA - Нажали на кнопку покупки рекламы");
		Send_LogsInAnalytics("CLICK_ADS");
	}

	public void Buy_ADS()
	{
		if (!PlayerPrefs.HasKey("buy_noads"))
		{
			PlayerPrefs.SetInt("buy_noads", 1);
			Send_LogsInAnalytics("BUY_ADS");
			UnityEngine.Debug.Log("GA - Игра куплена");
		}
	}

	public void Buy_COINS(int how)
	{
		Send_LogsInAnalytics("BUY_COINS " + how);
		UnityEngine.Debug.Log(" --- GA - Мы купили - " + how + " монет --- ");
	}

	public void Close_Multiplaer_Connection()
	{
		Send_LogsInAnalytics("CLOSE_WAIT_CONNECTION_MUL");
		UnityEngine.Debug.Log("GA - Закрыли ожидание подключения мультиплеера ");
	}

	public void Click_Show_Rewaerd_Video()
	{
		Send_LogsInAnalytics("CLICK_SHOW_REWARD_VIDEO  (-Shop-)");
	}

	public void Click_Play_Invitation()
	{
		Send_LogsInAnalytics("CLICK_PLAY_INVITATION");
	}

	public void Click_Button_Share(bool ver)
	{
		if (ver)
		{
			Send_LogsInAnalytics("CLICK_BUTTON_SHARE_level1");
		}
		else
		{
			Send_LogsInAnalytics("CLICK_BUTTON_SHARE_mainMenu");
		}
	}

	public void We_Showed_Button_Reward(bool ver)
	{
		if (ver)
		{
			Send_LogsInAnalytics("WE_SHOW_REWARD_BUTTON_coins_x2");
		}
		else
		{
			Send_LogsInAnalytics("WE_SHOW_REWARD_BUTTON_coins_plus20");
		}
	}

	public void Click_Show_Rewaerd_Video(bool ver)
	{
		if (ver)
		{
			Send_LogsInAnalytics("CLICK_SHOW_REWARD_BUTTON_coins_x2");
		}
		else
		{
			Send_LogsInAnalytics("CLICK_SHOW_REWARD_BUTTON_coins_plus20");
		}
	}

	public void Tutorial_Skip_Button()
	{
		Send_LogsInAnalytics("TUTORIAL_SKIP_BUTTON");
	}

	public void Aming_Tip()
	{
		Send_LogsInAnalytics("AMING_TIP");
	}

	public void Return_CoinsOnCloud()
	{
		Send_LogsInAnalytics("RETURN_COINS_CLOUD");
		UnityEngine.Debug.Log("------------------------- Сообщили в аналитику что монеты были возвращены ------------------------");
	}

	public void Down_ButtonAimingTip()
	{
		if (!PlayerPrefs.HasKey("aiming_tip"))
		{
			PlayerPrefs.SetInt("aiming_tip", 1);
			Send_LogsInAnalytics("Aiming ON");
			UnityEngine.Debug.Log("<------------------------------ GA - Нажали на Аиминг тип ---------------------------------->");
		}
	}

	public void Down_ButtonContinue()
	{
		if (!PlayerPrefs.HasKey("click_continue"))
		{
			PlayerPrefs.SetInt("click_continue", 1);
			Send_LogsInAnalytics("CLICK CONTINUE GAMES");
			UnityEngine.Debug.Log("<------------------------------ GA - Нажали на Продолжить ---------------------------------->");
		}
		Down_ButtonContinueAfterReward(2);
	}

	public void Down_ButtonBUYWeapons(int _shopIndex)
	{
		if (!PlayerPrefs.HasKey("click_shopIndex" + _shopIndex))
		{
			PlayerPrefs.SetInt("click_shopIndex" + _shopIndex, 1);
			return;
		}
		int @int = PlayerPrefs.GetInt("click_shopIndex" + _shopIndex, 1);
		@int++;
		PlayerPrefs.SetInt("click_shopIndex" + _shopIndex, @int);
		if (@int == 5)
		{
			Send_LogsInAnalytics(" BUY WEAPONS > 5 -- index -" + _shopIndex + string.Empty);
			UnityEngine.Debug.Log("<------------------------------ GA - ЭЛИМЕН Преобрели больше 5 раз index -" + _shopIndex + "---------------------------------->");
		}
	}

	public void Down_ButtonContinueAfterReward(int indexAR)
	{
		UnityEngine.Debug.Log("--------- ПРОДОЛЖЕНИЕ index -------> " + indexAR + " ----- locl_index----> " + local_indexAR);
		switch (indexAR)
		{
		case 3:
			local_indexAR = 0;
			break;
		case 1:
			local_indexAR = 1;
			break;
		case 2:
			if (local_indexAR == 1)
			{
				local_indexAR = 0;
				Send_LogsInAnalytics("CONTINUE AFTER REWARTED");
				UnityEngine.Debug.Log("<------------------------------ GA - Сработало продолжение после просмотра ревартед видео ---------------------------------->");
			}
			break;
		}
	}

	public void Send_DebugLogInAnalytic(string message)
	{
		Send_LogsInAnalytics("Exeption  ---> " + message);
	}

	public void Click_Promo_Module(string GameName)
	{
		Send_LogsInAnalytics(GameName);
	}

	public void Send_Score(int score)
	{
		if (score <= 10)
		{
			Send_LogsInAnalytics("Score: 10");
		}
		else if (score > 10 && score <= 45)
		{
			Send_LogsInAnalytics("Score: 45");
		}
		else if (score > 45 && score <= 85)
		{
			Send_LogsInAnalytics("Score: 85");
		}
		else if (score > 85 && score <= 120)
		{
			Send_LogsInAnalytics("Score: 120");
		}
		else if (score > 120 && score <= 160)
		{
			Send_LogsInAnalytics("Score: 160");
		}
		else if (score > 160)
		{
			Send_LogsInAnalytics("Score: > 160");
		}
	}

	private bool IsLogSend()
	{
		bool flag = false;
		return GetRemoteStatus_SendIvent();
	}

	private void Send_LogsInAnalytics(string n1)
	{
		if (IsLogSend())
		{
			UnityEngine.Debug.Log("------------------------- SEND OTHER IN ANALYTIC -------------------------");
			if (GetRemoteStatusAnalytic() && Status_Scene.Inst.GetAcces_GoogleAnlGDPR())
			{
				GA_Manager.Client.SendEventHit(n1, "--", "--", 0);
			}
		}
		// PJ 'AnalyticsEvent'.Custom(n1);
	}

	public void Button_Stop_Session()
	{
		UnityEngine.Debug.Log("GA - Закрываем сесию ");
		// PJ 'AnalyticsEvent'.GameOver();
	}

	public void ClickCloseCatPromo()
	{
		Send_LogsInAnalytics("CLICK CLOSE CATAPULT 2 PROMO");
		UnityEngine.Debug.Log("<------------------------------ GA - Нажали на Закрыть промо Катапульты ---------------------------------->");
	}

	public void ClickDownloadCatPromo()
	{
		Send_LogsInAnalytics("CLICK DOWNLOAD CATAPULT 2 PROMO");
		UnityEngine.Debug.Log("<------------------------------ GA - Нажали на Установить промо Катапульты ---------------------------------->");
	}
}
