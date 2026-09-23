using Logic;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
	public class GUIControl_MainMenu : MonoBehaviour
	{
		public static GUIControl_MainMenu Instance;

		public MenuControl _sceneControl;

		public AchievementWindow achievementWindow;

		public SavegameScreenshot screenshot;

		public GameObject mainControl;

		public GameObject infoControl;

		public GameObject settingsControl;

		public GameObject exitControl;

		public LoadingScreen loadingScreen;

		public GameObject[] rateStates;

		public GameObject[] exitStates;

		public GameObject notifyPanel;

		public GameObject singoutPanel;

		public GameObject savegamePanel;

		public GameObject loadgamePanel;

		public GameObject cat2PromoPanel;

		public GameObject leaderboardBlock;

		public GameObject achievementBlock;

		public GameObject connectToPGamesBlock;

		public GameObject connectToPGamesChainBlock;

		public Image connectToPGamesButton;

		public Sprite[] pGamesConnectStatus;

		private void Awake()
		{
			Instance = this;
		}

		public void InitDefault()
		{
			mainControl.SetActive(value: true);
			infoControl.SetActive(value: false);
			settingsControl.SetActive(value: false);
			exitControl.SetActive(value: false);
			cat2PromoPanel.SetActive(value: false);
			CheckRatings();
			CheckPGamesConnection();
			CheckPreProc();
			achievementWindow.LoadAchievementData();
			if (!PromoCheckingCat2.instance.CheckAppInstalled() && PromoCheckingCat2.instance.ShowingAllowed)
			{
				cat2PromoPanel.SetActive(value: true);
				PromoCheckingCat2.instance.PromoOpened();
			}
		}

		private void CheckPreProc()
		{
			_sceneControl.SetNotificationPreProc(value: true);
			connectToPGamesChainBlock.SetActive(value: true);
			connectToPGamesButton.gameObject.SetActive(value: true);
		}

		public void Cat2PromoClose()
		{
			My_GoogleAnalytics.Instance.ClickCloseCatPromo();
			cat2PromoPanel.SetActive(value: false);
		}

		public void Cat2PromoInstall()
		{
			My_GoogleAnalytics.Instance.ClickDownloadCatPromo();
			Application.OpenURL("market://details?id=com.byv.TheCatapult2");
			cat2PromoPanel.SetActive(value: false);
		}

		private void CheckRatings()
		{
			if (InitController.instance.IsAppRated)
			{
				rateStates[0].SetActive(value: false);
			}
			else
			{
				rateStates[0].SetActive(value: true);
			}
			if (InitController.instance.IsAppRated || InitController.instance.LaunchCount < 3)
			{
				exitStates[0].SetActive(value: true);
				exitStates[1].SetActive(value: false);
			}
			else
			{
				exitStates[1].SetActive(value: true);
				exitStates[0].SetActive(value: false);
			}
		}

		public void PlayButton_Click()
		{
			OnClick();
			loadingScreen.LoadScene("PlayModeSelect", showAds: true);
		}

		public void ChangeHand_Click()
		{
			OnClick();
			if (NewDataController.instance.GetPlayerControl() == ControlType.LeftHand)
			{
				NewDataController.instance.SetPlayerControl(ControlType.RightHand);
				NewDataController.instance.SavePlayerControl();
			}
			else
			{
				NewDataController.instance.SetPlayerControl(ControlType.LeftHand);
				NewDataController.instance.SavePlayerControl();
			}
			_sceneControl.CheckHandButton();
		}

		public void AimingTipClick()
		{
			OnClick();
			if (NewDataController.instance.GetPlayerAiming() == AimingTipType.None)
			{
				NewDataController.instance.SetPlayerAiming(AimingTipType.Dot);
				NewDataController.instance.SavePlayerAiming();
			}
			else if (NewDataController.instance.GetPlayerAiming() == AimingTipType.Dot)
			{
				NewDataController.instance.SetPlayerAiming(AimingTipType.None);
				NewDataController.instance.SavePlayerAiming();
			}
			_sceneControl.CheckAimButton();
			My_GoogleAnalytics.Instance.Down_ButtonAimingTip();
		}

		public void SettingsButton_Click()
		{
			OnClick();
			settingsControl.SetActive(value: true);
			mainControl.SetActive(value: false);
		}

		public void SettingsBack_Click()
		{
			OnClick();
			settingsControl.SetActive(value: false);
			mainControl.SetActive(value: true);
		}

		public void InfoButton_Ckick()
		{
			OnClick();
			infoControl.SetActive(value: true);
			settingsControl.SetActive(value: false);
		}

		public void InfoBack_Click()
		{
			OnClick();
			infoControl.SetActive(value: false);
			settingsControl.SetActive(value: true);
		}

		public void ExitButton_Click()
		{
			OnClick();
			if (InitController.instance.IsAppRated || InitController.instance.LaunchCount < 3)
			{
				exitStates[0].SetActive(value: true);
				exitStates[1].SetActive(value: false);
			}
			else
			{
				exitStates[1].SetActive(value: true);
				exitStates[0].SetActive(value: false);
			}
			mainControl.SetActive(value: false);
			exitControl.SetActive(value: true);
		}

		public void ReturnFromExit()
		{
			OnClick();
			mainControl.SetActive(value: true);
			exitControl.SetActive(value: false);
		}

		public void SoundButtonPressed()
		{
			OnClick();
			_sceneControl.SoundsHandle(sceneLoaded: true);
		}

		public void MusicButtonPressed()
		{
			OnClick();
			_sceneControl.MusicHandle(sceneLoaded: true);
		}

		public void GameExit()
		{
			OnClick();
			_sceneControl.GameExit();
		}

		public void RateButtonYes()
		{
			if (InitController.instance.LaunchCount >= 3 && !InitController.instance.IsAppRated)
			{
				InitController.instance.AppRated();
				_sceneControl.GameExit(stat: true);
			}
		}

		public void RateButtonNo()
		{
			_sceneControl.GameExit();
		}

		public void OpenSavegamePanel()
		{
			if (Unity_SavedGame.Instance.GetStatus_PlayService())
			{
				savegamePanel.SetActive(value: true);
			}
		}

		public void OpenLoadgamePanel()
		{
			if (Unity_SavedGame.Instance.GetStatus_PlayService())
			{
				loadgamePanel.SetActive(value: true);
			}
		}

		public void SaveGame()
		{
			if (Unity_SavedGame.Instance.GetStatus_PlayService())
			{
				screenshot.TakeScreensot();
			}
		}

		public void LoadGame()
		{
			if (Unity_SavedGame.Instance.GetStatus_PlayService())
			{
				Unity_SavedGame.Instance.ShowSelectUI();
			}
		}

		public void LoadGameAnswer(bool answer)
		{
			if (Unity_SavedGame.Instance.GetStatus_PlayService())
			{
				Unity_SavedGame.Instance.SavedGame_Selected(answer);
			}
		}

		public void DisableNotify()
		{
			_sceneControl.ChangeNotifications(state: false);
		}

		public void EnableNotify()
		{
			if (NewDataController.instance.GetPlayerNotifications() == 1)
			{
				notifyPanel.SetActive(value: true);
			}
			else
			{
				_sceneControl.ChangeNotifications(state: true);
			}
		}

		public void ShowLeaderboard()
		{
			Leaderboard.Instance.ShowLeaderboard();
		}

		public void MouseDown()
		{
			for (int i = 0; i < UnityEngine.Input.touchCount; i++)
			{
				if (UnityEngine.Input.GetTouch(i).phase == TouchPhase.Began)
				{
					InitController.instance.guiPressedFingerId = UnityEngine.Input.GetTouch(i).fingerId;
					break;
				}
			}
			InitController.instance.guiPressed = true;
		}

		public void MouseUp()
		{
			InitController.instance.guiPressedFingerId = -1f;
			InitController.instance.guiPressed = false;
		}

		public void TwitterButton()
		{
			OnClick();
			Application.OpenURL("https://twitter.com/HappyDragonApps");
		}

		public void FacebookButton()
		{
			OnClick();
			Application.OpenURL("https://www.facebook.com/BYVGames");
		}

		public void GoogleButton()
		{
			OnClick();
			Application.OpenURL("https://plus.google.com/+BYVGames");
		}

		public void VkButton()
		{
			OnClick();
			Application.OpenURL("https://vk.com/byvgames");
		}

		public void RatingButtonPressed()
		{
			Application.OpenURL("market://details?id=com.byv.TheCatapult");
			InitController.instance.AppRated();
		}

		public void OnClick()
		{
			SoundMgr.instance.ButtonPress();
		}

		public void CheckPGamesConnection()
		{
			if (Unity_SavedGame.Instance.GetStatus_PlayService())
			{
				connectToPGamesBlock.SetActive(value: false);
				achievementBlock.SetActive(value: true);
				leaderboardBlock.SetActive(value: true);
				connectToPGamesButton.sprite = pGamesConnectStatus[0];
			}
			else
			{
				connectToPGamesBlock.SetActive(value: true);
				leaderboardBlock.SetActive(value: false);
				achievementBlock.SetActive(value: false);
				connectToPGamesButton.sprite = pGamesConnectStatus[1];
			}
		}

		public void PGamesButton()
		{
			if (Unity_SavedGame.Instance.GetStatus_PlayService())
			{
				singoutPanel.SetActive(value: true);
			}
			else
			{
				Unity_SavedGame.Instance.PlayService_SignIN();
			}
		}

		public void OpenAchievements()
		{
			mainControl.SetActive(value: false);
			achievementWindow.gameObject.SetActive(value: true);
			InitController.instance.guiPressed = true;
		}

		public void CloseAchievements()
		{
			InitController.instance.guiPressed = false;
			mainControl.SetActive(value: true);
			achievementWindow.gameObject.SetActive(value: false);
		}

		public void DisconnectPGames()
		{
			Unity_SavedGame.Instance.PlayService_SignOUT();
			CheckPGamesConnection();
		}

		public void OpenExternalAchievementUI()
		{
			Social.ShowAchievementsUI();
		}

		public void AddKills()
		{
			AchievementManager.instance.AchievementProgress(AchieventType.KillEnemy, AchievementRegion.AllGame, 99);
		}

		public void ClearAchievements()
		{
			KTGameCenter.SharedCenter().ResetAchievements();
			UnityEngine.Debug.Log("========================================> All achievements cleared");
		}

		private void Update()
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
			{
				if (cat2PromoPanel.activeSelf)
				{
					cat2PromoPanel.SetActive(value: false);
				}
				else if (notifyPanel.activeSelf)
				{
					notifyPanel.SetActive(value: false);
				}
				else if (singoutPanel.activeSelf)
				{
					singoutPanel.SetActive(value: false);
				}
				else if (savegamePanel.activeSelf)
				{
					savegamePanel.SetActive(value: false);
				}
				else if (loadgamePanel.activeSelf)
				{
					loadgamePanel.SetActive(value: false);
				}
				else if (mainControl.activeSelf)
				{
					ExitButton_Click();
				}
				else if (settingsControl.activeSelf)
				{
					SettingsBack_Click();
				}
				else if (infoControl.activeSelf)
				{
					InfoBack_Click();
				}
				else if (exitControl.activeSelf)
				{
					ReturnFromExit();
				}
			}
		}
	}
}
