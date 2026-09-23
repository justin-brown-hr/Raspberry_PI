using UnityEngine;
using UnityEngine.UI;
using View;

namespace Logic
{
	public class PlayModeSceneControl : MonoBehaviour
	{
		public GUIControl_PlayModeSelect _sceneView;

		public GameObject mainPanel;

		public GameObject shopPanel;

		public GameObject shop;

		public Button upgrade;

		public Text levelText;

		public Text upgradeText;

		public string[] menuCatapults;

		public Text moneyText;

		private GameObject catapult;

		private void Start()
		{
			mainPanel.SetActive(value: true);
			InitController.instance.guiPressed = false;
			RefreshMoney();
			SetUpgradeCost();
			SpawnCatapult();
			NewDataController.instance.SetGameMode(GameMode.None);
			shop.GetComponent<PlayerShop>().PrepareShop();
			if (catapult != null && catapult.GetComponentInChildren<CatapultLogic>() != null)
			{
				catapult.GetComponentInChildren<CatapultLogic>().enabled = true;
			}
			shopPanel.SetActive(value: false);
		}

		public void RefreshMoney()
		{
			moneyText.text = NewDataController.instance.GetPlayerMoney().ToString();
		}

		public void SpawnCatapult()
		{
			if (catapult != null)
			{
				UnityEngine.Object.Destroy(catapult);
				catapult = null;
			}
			GameObject original = Resources.Load<GameObject>("Prefabs/Menu/" + menuCatapults[NewDataController.instance.GetCurrentCatapultIndex()]);
			catapult = UnityEngine.Object.Instantiate(original, Camera.main.ViewportToWorldPoint(new Vector3(0.25f, -0.765f, 10f)), Quaternion.identity);
			catapult.GetComponentInChildren<CatapultLogic>().InitCatapultInPool();
			catapult.GetComponentInChildren<CatapultLogic>().InitCatapultIngame();
			if (!shopPanel.activeSelf)
			{
				try
				{
					catapult.GetComponentInChildren<CatapultLogic>().enabled = true;
				}
				catch
				{
				}
			}
			else
			{
				try
				{
					catapult.GetComponentInChildren<CatapultLogic>().enabled = false;
				}
				catch
				{
				}
			}
		}

		public void EquipHelmet()
		{
			for (int i = 0; i < catapult.GetComponentInChildren<CatapultLogic>()._stickmans.Length; i++)
			{
				catapult.GetComponentInChildren<CatapultLogic>()._stickmans[i].EquipHelmet();
			}
		}

		public void EnterShop()
		{
			mainPanel.SetActive(value: false);
			shopPanel.SetActive(value: true);
			if (catapult != null)
			{
				catapult.GetComponentInChildren<CatapultLogic>().FreeCatapult();
				catapult.GetComponentInChildren<CatapultLogic>().enabled = false;
			}
			shop.GetComponent<PlayerShop>().EnterShop();
		}

		public void ExitShop()
		{
			mainPanel.SetActive(value: true);
			shopPanel.SetActive(value: false);
			if (catapult != null)
			{
				catapult.GetComponentInChildren<CatapultLogic>().enabled = true;
			}
		}

		public void SetUpgradeCost()
		{
			levelText.text = "LEVEL " + (NewDataController.instance.GetCurrentCatapultUpgrade() + 1) + "/" + InitController.instance.GetMaxLevel(NewDataController.instance.GetCurrentCatapultIndex());
			if (InitController.instance.GetCurrentCost() != -1f)
			{
				upgradeText.text = InitController.instance.GetCurrentCost().ToString();
			}
			else
			{
				upgradeText.text = "Maximum";
			}
		}

		private void OnApplicationFocus(bool focus)
		{
			if (!focus)
			{
				Screen.sleepTimeout = -2;
			}
			else
			{
				Screen.sleepTimeout = -1;
			}
		}

		private void Update()
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
			{
				if (mainPanel.activeSelf)
				{
					_sceneView.ReturnButton_Clicked();
				}
				else if (shopPanel.activeSelf)
				{
					ExitShop();
				}
			}
		}
	}
}
