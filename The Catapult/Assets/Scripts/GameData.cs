using Logic;
using System;

[Serializable]
public class GameData
{
	public int noAdsBuyState;

	public float playerMoney;

	public int playerAimingEnabled;

	public int playerControlHand;

	public int playerNotifications;

	public CatapultsGameData catapults;

	public HelmetsGameData helmets;

	public ShieldsGameData shields;

	public ProjectilesGameData projectiles;

	public SaveAchievementData[] achievements;

	public NewCatapultsGameData newCatapults;

	public GameData(int achievementSize)
	{
		catapults = new CatapultsGameData();
		helmets = new HelmetsGameData();
		shields = new ShieldsGameData();
		projectiles = new ProjectilesGameData();
		achievements = new SaveAchievementData[achievementSize];
		for (int i = 0; i < achievements.Length; i++)
		{
			achievements[i] = new SaveAchievementData();
		}
		newCatapults = new NewCatapultsGameData();
	}

	public void SetNoAdsState(int value)
	{
		noAdsBuyState = value;
	}

	public void SetMainData(float money, int aiming, int hand, int notify)
	{
		playerMoney = money;
		playerAimingEnabled = aiming;
		playerControlHand = hand;
		playerNotifications = notify;
	}

	public void SetHelmetsProgress(int[] allHelmets, int selected)
	{
		helmets.helmetsProgress = allHelmets;
		helmets.selectedHelmet = selected;
	}

	public void SetShieldsProgress(int[] allShields, int selected)
	{
		shields.shieldsProgress = allShields;
		shields.selectedShield = selected;
	}

	public void SetProjectilesProgress(int[] allProjectiles, int[] selected)
	{
		projectiles.projectilesCount = allProjectiles;
		projectiles.selectedProjectiles = selected;
	}

	public void SetAchievementGroupProgress(AchieventData[] data)
	{
		for (int i = 0; i < data.Length; i++)
		{
			if (i <= achievements.Length)
			{
				achievements[i].achievementProgress = data[i].currentProgress;
				achievements[i].isAchieved = (data[i].isAchieved ? 1 : 0);
				achievements[i].repeatCount = data[i].currentRepeatCount;
			}
		}
	}

	public void SetCatapultProgress(int selected, int[] canBuy, int[] isBuy, int[] upgrade)
	{
		newCatapults.selectedCatapult = selected;
		newCatapults.canBuyCatapult = canBuy;
		newCatapults.isCatapultBought = isBuy;
		newCatapults.catapultUpgrade = upgrade;
	}
}
