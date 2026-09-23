using UnityEngine;
using UnityEngine.UI;
using View;

namespace Logic
{
	public class PvPGameControl : MonoBehaviour
	{
		public static PvPGameControl instance;

		public LoadingScreen loadingScreen;

		public GameObject gameUI;

		public GameObject pausePanel;

		public GameObject gameOverPanel;

		public GameObject[] winnerTexts;

		public GameObject drawText;

		public Text[] playerScores;

		public GameObject[] playersProjectile;

		public Image[] playersReload;

		private bool[] rechargeStatus;

		private float[] rechargeTicks;

		private void Awake()
		{
			instance = this;
			rechargeStatus = new bool[2];
			rechargeTicks = new float[2];
		}

		private void Start()
		{
			InitController.instance.guiPressed = false;
			gameUI.SetActive(value: true);
			pausePanel.SetActive(value: false);
			gameOverPanel.SetActive(value: false);
			SetScore(GameSides.Player1, 20f);
			SetScore(GameSides.Player2, 20f);
			SetAvailableCount(GameSides.Player1, 0f);
			SetAvailableCount(GameSides.Player2, 0f);
		}

		public void ToMainMenu()
		{
			Time.timeScale = 1f;
			loadingScreen.LoadScene("PlayModeSelect", showAds: true);
		}

		public void PauseClick()
		{
			Time.timeScale = 0f;
			pausePanel.SetActive(value: true);
			gameUI.SetActive(value: false);
			gameOverPanel.SetActive(value: false);
		}

		public void Return()
		{
			PvPLogic.instance.PauseExit();
			Time.timeScale = 1f;
			pausePanel.SetActive(value: false);
			gameUI.SetActive(value: true);
			gameOverPanel.SetActive(value: false);
		}

		public void GameOver(int winner)
		{
			pausePanel.SetActive(value: false);
			gameUI.SetActive(value: false);
			gameOverPanel.SetActive(value: true);
			switch (winner)
			{
			case 0:
				winnerTexts[0].SetActive(value: false);
				winnerTexts[1].SetActive(value: false);
				drawText.SetActive(value: true);
				break;
			case 1:
				winnerTexts[0].SetActive(value: true);
				winnerTexts[1].SetActive(value: false);
				drawText.SetActive(value: false);
				break;
			default:
				winnerTexts[1].SetActive(value: true);
				winnerTexts[0].SetActive(value: false);
				drawText.SetActive(value: false);
				break;
			}
		}

		private void OnApplicationFocus(bool focus)
		{
			if (!focus && Time.timeScale == 1f && !PvPLogic.instance.isEndGame)
			{
				PauseClick();
			}
		}

		public void SetScore(GameSides side, float value)
		{
			if (side == GameSides.Player1)
			{
				playerScores[0].text = value.ToString();
			}
			else
			{
				playerScores[1].text = value.ToString();
			}
		}

		public void Recharge(float time, GameSides side)
		{
			playersReload[(int)side].fillAmount = 0f;
			rechargeTicks[(int)side] = 1f / (time / Time.deltaTime);
			rechargeStatus[(int)side] = true;
		}

		public void SetProjectileType(GameSides side, Sprite sprite)
		{
			playersProjectile[(int)side].GetComponent<Image>().sprite = sprite;
		}

		public void SetAvailableCount(GameSides side, float count)
		{
			if (count > 0f)
			{
				playersProjectile[(int)side].GetComponentInChildren<Text>().text = count.ToString();
			}
			else
			{
				playersProjectile[(int)side].GetComponentInChildren<Text>().text = string.Empty;
			}
		}

		public void ReloadGame()
		{
			Time.timeScale = 1f;
			loadingScreen.LoadScene("PvPScene", showAds: true);
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

		public void RatingButtonPressed()
		{
			Application.OpenURL("market://details?id=com.byv.TheCatapult");
			InitController.instance.AppRated();
		}

		public void OnClickSound()
		{
			SoundMgr.instance.ButtonPress();
		}

		private void Update()
		{
			if (Time.timeScale != 0f)
			{
				if (rechargeStatus[0])
				{
					if (playersReload[0].fillAmount < 1f)
					{
						playersReload[0].fillAmount += rechargeTicks[0];
					}
					else
					{
						rechargeStatus[0] = false;
					}
				}
				if (rechargeStatus[1])
				{
					if (playersReload[1].fillAmount < 1f)
					{
						playersReload[1].fillAmount += rechargeTicks[1];
					}
					else
					{
						rechargeStatus[1] = false;
					}
				}
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
			{
				if (gameUI.activeSelf)
				{
					PauseClick();
				}
				else if (pausePanel.activeSelf)
				{
					Return();
				}
				else if (gameOverPanel.activeSelf)
				{
					ToMainMenu();
				}
			}
		}
	}
}
