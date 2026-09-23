using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using View;

namespace Logic
{
	public class GameMenuControl : MonoBehaviour
	{
		public static GameMenuControl instance;

		public LoadingScreen loadingScreen;

		public LevelDonationPanel donations;

		public GameObject gamePanel;

		public GameObject pausePanel;

		public GameObject gameOverPanel;

		public Text gameScore;

		public Text[] gameCoins;

		public Text bestScore;

		public Text finalScore;

		public Text[] oneGameCoins;

		public Button rewardButton;

		public Sprite[] rewardButtonSprite;

		public Transform[] rewardButtonPositions;

		public Transform[] movablePanels;

		public Transform[] movablePanelsPositions;

		public Transform moneyPopupPlace;

		private bool rewardRecieved;

		public Button continueAfterDeathButton;

		private int continueGameCost;

		private bool continueCostSetted;

		public GameObject moneyPopup;

		private Queue<float> popupList;

		private float popupTime;

		public GameObject[] handTransformNeeded;

		public RectTransform[] needToMirror;

		private int resurected;

		private int maxResurected = 1;

		public Button alwaysSteelChecker;

		public Text helpDisablingText;

		private bool textShowed;

		private float textShowedTime;

		public bool isAlwaysSteel
		{
			get;
			set;
		}

		private void Awake()
		{
			InitController.instance.guiPressed = false;
			textShowed = false;
			isAlwaysSteel = false;
			instance = this;
			rewardRecieved = false;
			resurected = 0;
			Time.timeScale = 1f;
			continueGameCost = 0;
			continueCostSetted = false;
			popupTime = 0f;
			popupList = new Queue<float>();
		}

		private void Start()
		{
			helpDisablingText.gameObject.SetActive(value: false);
			RefreshCoins();
			CheckControlHand();
			gamePanel.SetActive(value: true);
			pausePanel.SetActive(value: false);
			gameOverPanel.SetActive(value: false);
			donations.UnBlockPanel();
		}

		public void PauseGame()
		{
			SoundMgr.instance.PauseGame();
			InitController.instance.guiPressed = true;
			gamePanel.SetActive(value: false);
			gameOverPanel.SetActive(value: false);
			pausePanel.SetActive(value: true);
			GlobalLogic.instance.isPaused = true;
			Time.timeScale = 0f;
		}

		public void UpdateScore()
		{
			gameScore.text = InitController.instance.GetPoint().ToString();
		}

		public void RefreshCoins()
		{
			for (int i = 0; i < gameCoins.Length; i++)
			{
				gameCoins[i].text = NewDataController.instance.GetPlayerMoney().ToString();
			}
		}

		public void AddToPopup(float value)
		{
			if (popupList.Count == 0)
			{
				popupTime = 0f;
			}
			popupList.Enqueue(value);
		}

		public void PopupMoney(float value)
		{
			FloatingText component = UnityEngine.Object.Instantiate(moneyPopup, moneyPopupPlace.position, Quaternion.identity, gamePanel.transform).GetComponent<FloatingText>();
			component.transform.position = moneyPopupPlace.position;
			component.SetText("+" + value.ToString());
			if (NewDataController.instance.GetPlayerControl() == ControlType.RightHand)
			{
				component.transform.localScale = new Vector3(-1f, 1f, 1f);
			}
			popupTime = 0.25f;
		}

		public void EndGame()
		{
			NewDataController.instance.SaveGameDataToPlayerPrefs();
			int point = InitController.instance.GetPoint();
			SoundMgr.instance.StopAllSounds();
			finalScore.text = "SCORE: " + point.ToString();
			if (point > InitController.instance.GetBestScore())
			{
				InitController.instance.SetBestScore();
			}
			bestScore.text = "BEST: " + InitController.instance.GetBestScore().ToString();
			if (Start_Admob.Instance.RewardVideoIsLoaded())
			{
				rewardButton.interactable = true;
				rewardButton.transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
			}
			else
			{
				rewardButton.interactable = false;
				rewardButton.transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
			}
			if (GlobalLogic.instance._oneGameCoins <= 20)
			{
				rewardButton.GetComponent<Image>().sprite = rewardButtonSprite[0];
				rewardButton.transform.GetChild(0).transform.position = rewardButton.transform.GetChild(1).transform.position;
			}
			else
			{
				rewardButton.GetComponent<Image>().sprite = rewardButtonSprite[1];
				rewardButton.transform.GetChild(0).transform.position = rewardButton.transform.GetChild(2).transform.position;
			}
			continueGameCost = 0;
			continueCostSetted = false;
			if (InitController.instance.GetPoint() >= 10 && resurected < maxResurected)
			{
				ContinueButtonControll(state: true);
			}
			else
			{
				ContinueButtonControll(state: false);
			}
			gamePanel.SetActive(value: false);
			pausePanel.SetActive(value: false);
			gameOverPanel.SetActive(value: true);
			My_GoogleAnalytics.Instance.Send_Score(point);
		}

		public void ToMainMenu()
		{
			Time.timeScale = 1f;
			InitController.instance.guiPressed = false;
			if (GlobalLogic.instance.isGameEnded)
			{
				AchievementManager.instance.AchievementProgress(AchieventType.JustDie, AchievementRegion.AllGame, 1);
			}
			AchievementManager.instance.CheckEndOfRound();
			loadingScreen.LoadScene("PlayModeSelect", showAds: true);
		}

		public void ReloadGame(bool newGameStarting)
		{
			gamePanel.SetActive(value: true);
			pausePanel.SetActive(value: false);
			gameOverPanel.SetActive(value: false);
			InitController.instance.ResetPoint();
			Time.timeScale = 1f;
			if (GlobalLogic.instance.isGameEnded)
			{
				AchievementManager.instance.AchievementProgress(AchieventType.JustDie, AchievementRegion.AllGame, 1);
			}
			AchievementManager.instance.CheckEndOfRound();
			loadingScreen.LoadScene("GameScene", showAds: true);
		}

		public void ResumeGame()
		{
			SoundMgr.instance.UnPauseGame();
			InitController.instance.guiPressed = false;
			gamePanel.SetActive(value: true);
			pausePanel.SetActive(value: false);
			gameOverPanel.SetActive(value: false);
			GlobalLogic.instance.isPaused = false;
			Time.timeScale = 1f;
		}

		private void OnApplicationFocus(bool focus)
		{
			if (!focus)
			{
				if (GlobalLogic.instance != null)
				{
					Screen.sleepTimeout = -2;
					if (!GlobalLogic.instance.isGameEnded)
					{
						PauseGame();
					}
				}
			}
			else
			{
				Screen.sleepTimeout = -1;
			}
		}

		private void OnApplicationPause(bool pause)
		{
		}

		public void RatingButtonPressed()
		{
			Application.OpenURL("market://details?id=com.byv.TheCatapult");
			InitController.instance.AppRated();
		}

		public void TwitterButton()
		{
			Application.OpenURL("https://twitter.com/HappyDragonApps");
		}

		public void FacebookButton()
		{
			Application.OpenURL("https://www.facebook.com/HappyDragonGames");
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

		public void MoreCoins()
		{
			Start_Admob.Instance.ShowRewardVideo();
		}

		public void AddCoinsAfterGame()
		{
			rewardRecieved = true;
			if (GlobalLogic.instance._oneGameCoins <= 20)
			{
				NewDataController.instance.AddMoney(20);
				GlobalLogic.instance._oneGameCoins += 20;
				SetOneGameCoins(GlobalLogic.instance._oneGameCoins, endAdded: true);
			}
			else
			{
				NewDataController.instance.AddMoney(GlobalLogic.instance._oneGameCoins);
				GlobalLogic.instance._oneGameCoins *= 2;
				SetOneGameCoins(GlobalLogic.instance._oneGameCoins, endAdded: true);
				My_GoogleAnalytics.Instance.Down_ButtonContinueAfterReward(1);
			}
			RefreshCoins();
			rewardButton.interactable = false;
			rewardButton.transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
		}

		public void Activation_ButtonRewartd()
		{
			if (Start_Admob.Instance.RewardVideoIsLoaded() && !rewardRecieved)
			{
				rewardButton.interactable = true;
				rewardButton.transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
			}
			else
			{
				rewardButton.interactable = false;
				rewardButton.transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
			}
		}

		private void ContinueButtonControll(bool state)
		{
			if (state)
			{
				if (!continueCostSetted)
				{
					if (GlobalLogic.instance._oneGameCoins >= 20)
					{
						continueGameCost = (int)Mathf.Round((float)GlobalLogic.instance._oneGameCoins * 1.5f);
					}
					else
					{
						continueGameCost = 20;
					}
					continueCostSetted = true;
				}
				continueAfterDeathButton.transform.GetComponentInChildren<Text>().text = continueGameCost.ToString();
				continueAfterDeathButton.gameObject.SetActive(value: true);
				continueAfterDeathButton.transform.GetComponentInChildren<Image>().color = new Color(1f, 1f, 1f, 1f);
				continueAfterDeathButton.transform.GetComponentInChildren<Text>().color = new Color(1f, 1f, 1f, 1f);
			}
			else
			{
				continueAfterDeathButton.gameObject.SetActive(value: false);
				rewardButton.transform.position = rewardButtonPositions[1].position;
				movablePanels[0].transform.position = movablePanelsPositions[0].transform.position;
				movablePanels[1].transform.position = movablePanelsPositions[1].transform.position;
			}
		}

		public void ContinueAfterDeath()
		{
			if (resurected < maxResurected && InitController.instance.GetPoint() >= 10 && NewDataController.instance.IsEnoughMoney(continueGameCost))
			{
				NewDataController.instance.CheckoutMoney(continueGameCost);
				GlobalLogic.instance._oneGameCoins -= continueGameCost;
				if (GlobalLogic.instance._oneGameCoins < 0)
				{
					GlobalLogic.instance._oneGameCoins = 0;
				}
				RefreshCoins();
				SoundMgr.instance.PurchaseSound();
				resurected++;
				ContinueButtonControll(state: false);
				gamePanel.SetActive(value: true);
				pausePanel.SetActive(value: false);
				gameOverPanel.SetActive(value: false);
				GlobalLogic.instance.ContinueGameAfterDeath();
				My_GoogleAnalytics.Instance.Down_ButtonContinue();
			}
		}

		public void SetOneGameCoins(float value, bool endAdded = false)
		{
			if (value >= 0f)
			{
				oneGameCoins[0].rectTransform.sizeDelta = new Vector2((float)value.ToString().Length * 50f, 200f);
				oneGameCoins[0].text = value.ToString();
			}
			else
			{
				oneGameCoins[0].rectTransform.sizeDelta = new Vector2(50f, 200f);
				oneGameCoins[0].text = "0";
			}
		}

		public void CheckControlHand()
		{
			if (NewDataController.instance.GetPlayerControl() == ControlType.RightHand)
			{
				Transform transform = Camera.main.transform;
				Transform transform2 = transform;
				Vector3 position = transform.position;
				float x = position.x;
				Vector3 position2 = transform.position;
				transform2.position = new Vector3(x, position2.y, 90f);
				transform.rotation = Quaternion.Euler(0f, 180f, 0f);
				Vector3 localPosition = handTransformNeeded[0].transform.localPosition;
				float num = localPosition.x - 10f;
				handTransformNeeded[0].GetComponent<RectTransform>().anchorMax = new Vector2(0f, 1f);
				handTransformNeeded[0].GetComponent<RectTransform>().anchorMin = new Vector2(0f, 1f);
				Transform transform3 = handTransformNeeded[0].transform;
				float x2 = -1f * num;
				Vector3 localPosition2 = handTransformNeeded[0].transform.localPosition;
				transform3.localPosition = new Vector2(x2, localPosition2.y);
				Vector3 position3 = handTransformNeeded[1].transform.position;
				num = position3.x;
				handTransformNeeded[1].GetComponent<RectTransform>().anchorMax = new Vector2(0f, 1f);
				handTransformNeeded[1].GetComponent<RectTransform>().anchorMin = new Vector2(0f, 1f);
				Transform transform4 = handTransformNeeded[1].transform;
				float x3 = -1f * num;
				Vector3 position4 = handTransformNeeded[1].transform.position;
				transform4.position = new Vector2(x3, position4.y);
				Vector3 position5 = handTransformNeeded[2].transform.position;
				num = position5.x;
				handTransformNeeded[2].GetComponent<RectTransform>().anchorMax = new Vector2(0f, 0f);
				handTransformNeeded[2].GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0f);
				Transform transform5 = handTransformNeeded[2].transform;
				float x4 = -1f * num;
				Vector3 position6 = handTransformNeeded[2].transform.position;
				transform5.position = new Vector2(x4, position6.y);
				for (int i = 3; i < 6; i++)
				{
					Vector3 position7 = handTransformNeeded[i].transform.position;
					num = position7.x;
					handTransformNeeded[i].GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
					handTransformNeeded[i].GetComponent<RectTransform>().anchorMin = new Vector2(1f, 1f);
					Transform transform6 = handTransformNeeded[i].transform;
					float x5 = -1f * num;
					Vector3 position8 = handTransformNeeded[i].transform.position;
					transform6.position = new Vector2(x5, position8.y);
				}
				for (int j = 0; j < needToMirror.Length; j++)
				{
					RectTransform obj = needToMirror[j];
					Vector3 localScale = needToMirror[j].localScale;
					float x6 = localScale.x * -1f;
					Vector3 localScale2 = needToMirror[j].localScale;
					obj.localScale = new Vector3(x6, localScale2.y);
				}
			}
		}

		public void ChangeStoneToSteel()
		{
			if (isAlwaysSteel)
			{
				alwaysSteelChecker.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
				isAlwaysSteel = false;
			}
			else
			{
				alwaysSteelChecker.GetComponent<Image>().color = new Color(1f, 0f, 0f, 1f);
				isAlwaysSteel = true;
			}
		}

		public void PopupBonusKill(int stage)
		{
			string empty = string.Empty;
			switch (stage)
			{
			default:
				return;
			case 1:
				empty = "Double Kill!";
				AchievementManager.instance.AchievementProgress(AchieventType.DoubleKill, AchievementRegion.OneRound, 1);
				break;
			case 2:
				empty = "Triple Kill!";
				AchievementManager.instance.AchievementProgress(AchieventType.TrippleKill, AchievementRegion.OneRound, 1);
				break;
			}
			helpDisablingText.color = new Color(1f, 1f, 1f, 1f);
			helpDisablingText.text = empty;
			helpDisablingText.gameObject.SetActive(value: true);
			textShowedTime = 1f;
			textShowed = true;
		}

		public void OnClickSound()
		{
			SoundMgr.instance.ButtonPress();
		}

		private void Update()
		{
			if (textShowed)
			{
				if (textShowedTime > 1f)
				{
					textShowedTime -= Time.deltaTime;
				}
				else if (textShowedTime <= 0f)
				{
					helpDisablingText.gameObject.SetActive(value: false);
					textShowed = false;
				}
				else
				{
					textShowedTime -= Time.deltaTime;
					helpDisablingText.color -= new Color(0f, 0f, 0f, Time.deltaTime);
				}
			}
			if (popupList.Count > 0)
			{
				if (popupTime > 0f)
				{
					popupTime -= Time.deltaTime;
				}
				else
				{
					PopupMoney(popupList.Dequeue());
				}
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
			{
				if (gamePanel.activeSelf)
				{
					PauseGame();
				}
				else if (pausePanel.activeSelf)
				{
					ResumeGame();
				}
				else if (gameOverPanel.activeSelf)
				{
					ToMainMenu();
				}
			}
		}
	}
}
