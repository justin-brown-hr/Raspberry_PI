using Assets.All_Scripts;
using Logic;
using UnityEngine;
using View;

public class SavegameManager : MonoBehaviour
{
	public static SavegameManager instance;

	private JSONSerializer<GameData> serializer;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
		else
		{
			UnityEngine.Object.Destroy(instance);
		}
	}

	private void Start()
	{
		serializer = new JSONSerializer<GameData>();
	}

	public void SaveGameData(Texture2D image)
	{
		if (Unity_SavedGame.Instance.GetStatus_PlayService())
		{
			GameData data = new GameData(AchievementManager.instance.listOfAchievements.Length);
			GetAllPlayerData(ref data);
			string jsonString = serializer.GenerateString(data);
			byte[] savedArray = serializer.CodeToBinary(jsonString);
			Unity_SavedGame.Instance.Save_GameInCloud(savedArray, image);
		}
	}

	private void GetAllPlayerData(ref GameData data)
	{
		data.SetNoAdsState(PlayerPrefs.GetInt(Status_Scene.Inst.status_NoADS));
		data.SetMainData(NewDataController.instance.GetPlayerMoney(), (int)NewDataController.instance.GetPlayerAiming(), (int)NewDataController.instance.GetPlayerControl(), NewDataController.instance.GetPlayerNotifications());
		data.SetHelmetsProgress(NewDataController.instance.GetAllHelmetsIsBought(), NewDataController.instance.GetEquipedHelmet());
		data.SetShieldsProgress(NewDataController.instance.GetAllShieldsIsBought(), NewDataController.instance.GetEquipedShield());
		data.SetProjectilesProgress(ProjectileControl.instance.GetAllProjectileCounts(), ProjectileControl.instance.GetAllEquipedProjectiles());
		data.SetAchievementGroupProgress(AchievementManager.instance.listOfAchievements);
		data.SetCatapultProgress(NewDataController.instance.GetCurrentCatapultIndex(), NewDataController.instance.GetAllCanBuyCatapult(), NewDataController.instance.GetAllCatapultIsBought(), NewDataController.instance.GetAllCatapultUpgrades());
	}

	public void LoadGameData(byte[] byteArr)
	{
		if (Unity_SavedGame.Instance.GetStatus_PlayService())
		{
			GameData gameData = new GameData(AchievementManager.instance.listOfAchievements.Length);
			string json = serializer.EncodeBinary(byteArr);
			gameData = JsonUtility.FromJson<GameData>(json);
			SetAllPlayerData(gameData);
			GUIControl_MainMenu.Instance._sceneControl.SavegameLoaded();
		}
	}

	public void LoadGameData(string jsonString)
	{
		GameData gameData = new GameData(AchievementManager.instance.listOfAchievements.Length);
		gameData = JsonUtility.FromJson<GameData>(jsonString);
		SetAllPlayerData(gameData);
		GUIControl_MainMenu.Instance._sceneControl.SavegameLoaded();
	}

	private void SetAllPlayerData(GameData data)
	{
		PlayerPrefs.SetInt(Status_Scene.Inst.status_NoADS, data.noAdsBuyState);
		if (data.newCatapults.canBuyCatapult != null && data.newCatapults.canBuyCatapult.Length != 0)
		{
			NewDataController.instance.LoadMainDataFromSave((int)data.playerMoney, data.playerAimingEnabled, data.playerControlHand, data.playerNotifications);
			NewDataController.instance.LoadHelmetDataFromSave(data.helmets.selectedHelmet, data.helmets.helmetsProgress);
			NewDataController.instance.LoadShieldDataFromSave(data.shields.selectedShield, data.shields.shieldsProgress);
			ProjectileControl.instance.LoadProjectilesSavedData(data.projectiles.selectedProjectiles, data.projectiles.projectilesCount);
			NewDataController.instance.LoadCatapultDataFromSaveFile(data.newCatapults.selectedCatapult, data.newCatapults.canBuyCatapult, data.newCatapults.isCatapultBought, data.newCatapults.catapultUpgrade);
			UnityEngine.Debug.Log("=======> Start achievement loading");
			for (int i = 0; i < data.achievements.Length; i++)
			{
				AchievementManager.instance.SetDataFromSaveFile(i, data.achievements[i].achievementProgress, data.achievements[i].isAchieved, data.achievements[i].repeatCount);
			}
			AchievementManager.instance.FillNonSavedData(data.achievements.Length);
			UnityEngine.Debug.Log("=======> End achievement loading");
		}
		else
		{
			NewDataController.instance.LoadOldMainDataFromSave(data.playerMoney, data.playerAimingEnabled, data.playerControlHand, data.playerNotifications);
			NewDataController.instance.LoadHelmetDataFromSave(data.helmets.selectedHelmet, data.helmets.helmetsProgress);
			NewDataController.instance.LoadOldShieldDataFromSave(data.shields.selectedShield, data.shields.shieldsProgress);
			ProjectileControl.instance.LoadOldProjectilesSavedData(data.projectiles.selectedProjectiles, data.projectiles.projectilesCount);
			NewDataController.instance.LoadOldCatapultDataFromSaveFile(data.catapults.selectedCatapult, data.catapults.catapultsProgress);
			for (int j = 0; j < data.achievements.Length; j++)
			{
				AchievementManager.instance.SetDataFromSaveFile(j, data.achievements[j].achievementProgress, data.achievements[j].isAchieved, data.achievements[j].repeatCount);
			}
			AchievementManager.instance.FillNonSavedData(data.achievements.Length);
		}
	}
}
