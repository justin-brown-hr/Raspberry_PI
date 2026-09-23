using Assets.All_Scripts;
using UnityEngine;
using UnityEngine.UI;
using View;

namespace Logic
{
	public class MenuControl : MonoBehaviour
	{
		public GUIControl_MainMenu _sceneView;

		public GameObject soundButton;

		public GameObject musicButton;

		public GameObject tipButton;

		public GameObject handButton;

		public GameObject[] menuObjects;

		public string[] menuCatapults;

		public GameObject birdPrefab;

		public Sprite[] soundStates;

		public Sprite[] musicStates;

		public Sprite[] aimingStates;

		public Sprite[] handSprites;

		public GameObject notificationButton;

		public GameObject notificationChainBlock;

		public Sprite[] notificationStates;

		private bool birdSpawnAllowed;

		private float birdSpawnTime;

		public GameObject currentCatapult;

		private void Start()
		{
			_sceneView.InitDefault();
			InitController.instance.guiPressed = false;
			SoundsHandle();
			MusicHandle();
			if (InitController.instance != null)
			{
				InitController.instance.guiPressed = false;
			}
			NewDataController.instance.SetGameMode(GameMode.None);
			int currentCatapultIndex = NewDataController.instance.GetCurrentCatapultIndex();
			GameObject original = Resources.Load<GameObject>("Prefabs/Menu/" + menuCatapults[currentCatapultIndex]);
			currentCatapult = UnityEngine.Object.Instantiate(original, Camera.main.ViewportToWorldPoint(new Vector3(0.25f, -0.765f, 10f)), Quaternion.identity);
			currentCatapult.GetComponentInChildren<CatapultLogic>().InitCatapultInPool();
			currentCatapult.GetComponentInChildren<CatapultLogic>().InitCatapultIngame();
			CheckScreenSize();
			birdSpawnTime = UnityEngine.Random.Range(25f, 35f);
			CheckAimButton();
			CheckHandButton();
			CheckNotifications();
		}

		public void SetNotificationPreProc(bool value)
		{
			if (!value)
			{
				notificationButton.SetActive(value: false);
				notificationChainBlock.SetActive(value: false);
			}
			else
			{
				notificationChainBlock.SetActive(value: true);
				notificationButton.SetActive(value: true);
			}
		}

		public void SavegameLoaded()
		{
			UnityEngine.Object.Destroy(currentCatapult);
			currentCatapult = null;
			GameObject original = Resources.Load<GameObject>("Prefabs/Menu/" + menuCatapults[NewDataController.instance.GetCurrentCatapultIndex()]);
			currentCatapult = UnityEngine.Object.Instantiate(original, Camera.main.ViewportToWorldPoint(new Vector3(0.25f, -0.765f, 10f)), Quaternion.identity);
			currentCatapult.GetComponentInChildren<CatapultLogic>().InitCatapultInPool();
			currentCatapult.GetComponentInChildren<CatapultLogic>().InitCatapultIngame();
			CheckAimButton();
			CheckHandButton();
			CheckNotifications();
		}

		public void CheckAimButton()
		{
			if (NewDataController.instance.GetPlayerAiming() == AimingTipType.None)
			{
				tipButton.GetComponent<Image>().sprite = aimingStates[0];
			}
			else if (NewDataController.instance.GetPlayerAiming() == AimingTipType.Dot)
			{
				tipButton.GetComponent<Image>().sprite = aimingStates[1];
			}
		}

		public void CheckHandButton()
		{
			if (NewDataController.instance.GetPlayerControl() == ControlType.LeftHand)
			{
				handButton.GetComponent<Image>().sprite = handSprites[0];
			}
			else if (NewDataController.instance.GetPlayerControl() == ControlType.RightHand)
			{
				handButton.GetComponent<Image>().sprite = handSprites[1];
			}
		}

		private void CheckScreenSize()
		{
			float num = Camera.main.pixelWidth;
			float num2 = Camera.main.pixelHeight;
			if (!(num / num2 < 1.4f) && (double)(num / num2) > 1.9)
			{
				for (int i = 0; i < menuObjects.Length; i++)
				{
					Transform transform = menuObjects[i].transform;
					Vector3 localScale = menuObjects[i].transform.localScale;
					float x = localScale.x * 0.8f;
					Vector3 localScale2 = menuObjects[i].transform.localScale;
					float y = localScale2.y * 0.8f;
					Vector3 localScale3 = menuObjects[i].transform.localScale;
					transform.localScale = new Vector3(x, y, localScale3.z * 0.8f);
				}
			}
		}

		public void SoundsHandle(bool sceneLoaded = false)
		{
			if (!sceneLoaded)
			{
				ButtonEnabling(SoundMgr.instance.soundEnabled ? 1 : 0, 1);
			}
			else if (SoundMgr.instance.soundEnabled)
			{
				ButtonEnabling(0, 1);
				if (sceneLoaded)
				{
					SoundMgr.instance.HandleSound();
				}
			}
			else
			{
				ButtonEnabling(1, 1);
				if (sceneLoaded)
				{
					SoundMgr.instance.HandleSound();
				}
			}
		}

		private void ButtonEnabling(int state, int button)
		{
			if (button == 1)
			{
				if (state == 1)
				{
					soundButton.GetComponent<Image>().sprite = soundStates[1];
				}
				else
				{
					soundButton.GetComponent<Image>().sprite = soundStates[0];
				}
			}
			else if (state == 1)
			{
				musicButton.GetComponent<Image>().sprite = musicStates[1];
			}
			else
			{
				musicButton.GetComponent<Image>().sprite = musicStates[0];
			}
		}

		public void MusicHandle(bool sceneLoaded = false)
		{
			if (!sceneLoaded)
			{
				ButtonEnabling(SoundMgr.instance.musicEnabled ? 1 : 0, 2);
				SoundMgr.instance.PlayMusic();
				return;
			}
			if (SoundMgr.instance.musicEnabled)
			{
				ButtonEnabling(0, 2);
				if (sceneLoaded)
				{
					SoundMgr.instance.HandleMusic();
				}
				return;
			}
			ButtonEnabling(1, 2);
			if (sceneLoaded)
			{
				SoundMgr.instance.HandleMusic();
			}
			SoundMgr.instance.PlayMusic();
		}

		public void GameExit(bool stat = false)
		{
			NewDataController.instance.SaveGameDataToPlayerPrefs();
			My_GoogleAnalytics.Instance.Button_Stop_Session();
			Screen.sleepTimeout = -2;
			if (stat)
			{
				Status_Scene.Inst.Status_RateGame = true;
				Application.OpenURL("market://details?id=com.byv.TheCatapult");
			}
			else
			{
				Application.Quit();
			}
		}

		private void OnApplicationFocus(bool focusStatus)
		{
			if (focusStatus && Status_Scene.Inst.Status_RateGame)
			{
				Application.Quit();
			}
		}

		private void SpawnBird()
		{
			if (birdSpawnAllowed)
			{
				birdSpawnAllowed = false;
				int side = 1;
				float x = 1.15f;
				if (UnityEngine.Random.Range(-1f, 1f) < 0f)
				{
					side = -1;
					x = -0.15f;
				}
				float y = UnityEngine.Random.Range(0.65f, 0.8f);
				BirdBonusLogic component = UnityEngine.Object.Instantiate(birdPrefab, Camera.main.ViewportToWorldPoint(new Vector3(x, y, 10f)), Quaternion.identity).GetComponent<BirdBonusLogic>();
				component.Init(side, isMenu: true);
				birdSpawnTime = UnityEngine.Random.Range(25f, 35f);
			}
		}

		private void Update()
		{
			if (birdSpawnTime > 0f)
			{
				birdSpawnTime -= Time.deltaTime;
				return;
			}
			birdSpawnAllowed = true;
			SpawnBird();
		}

		public void CheckNotifications()
		{
			if (NewDataController.instance.GetPlayerNotifications() == 1)
			{
				notificationButton.GetComponent<Image>().sprite = notificationStates[0];
			}
			else
			{
				notificationButton.GetComponent<Image>().sprite = notificationStates[1];
			}
		}

		public void ChangeNotifications(bool state)
		{
			if (NewDataController.instance.GetPlayerNotifications() == 1 && !state)
			{
				NewDataController.instance.DisableNotifications();
				NewDataController.instance.SavePlayerNotifications();
				CheckNotifications();
			}
			if (NewDataController.instance.GetPlayerNotifications() == 0 && state)
			{
				NewDataController.instance.EnableNotifications();
				NewDataController.instance.SavePlayerNotifications();
				CheckNotifications();
			}
		}
	}
}
