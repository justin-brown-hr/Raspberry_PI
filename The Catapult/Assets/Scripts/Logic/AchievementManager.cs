using GooglePlayGames;
using Model;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;

namespace Logic
{
	public class AchievementManager : MonoBehaviour
	{
		public static AchievementManager instance;

		private string achievementProgressSaveString = "achievementProgress_";

		private string achievementSaveString = "achievement_";

		public AchieventData[] listOfAchievements;

		private int[] roundProgress;

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			}
			else
			{
				UnityEngine.Object.Destroy(this);
			}
		}

		private void Start()
		{
			if (Unity_SavedGame.Instance.GetStatus_PlayService())
			{
				Invoke("LoadData", 2f);
			}
		}

		private void LoadData()
		{
			LoadAchievementDataFromPP(needRecheckBought: true);
			CheckVisitAchievement();
		}

		public void ServiceConnectedManually()
		{
			ResetVisitAchievements();
			CheckVisitAchievement();
		}

		public void LoadServiceAchievementsStatus()
		{
			LoadAchievementDataFromPP();
			Social.LoadAchievements(delegate(IAchievement[] achievements)
			{
				if (achievements.Length > 0)
				{
					foreach (IAchievement achievement in achievements)
					{
						for (int j = 0; j < listOfAchievements.Length; j++)
						{
							if (achievement.id == listOfAchievements[j].achievementID)
							{
								listOfAchievements[j].isAchieved = achievement.completed;
								SaveAchievement(j);
								UnityEngine.Debug.Log("Loaded achievement " + listOfAchievements[j].name + " status -> " + achievement.completed);
								if (listOfAchievements[j].isIncrement)
								{
									listOfAchievements[j].currentProgress = (int)((double)((float)listOfAchievements[j].targetProgress / 100f) * achievement.percentCompleted);
									SaveAchievementProgress(j);
									UnityEngine.Debug.Log("Loaded achievement " + listOfAchievements[j].name + " progress -> " + achievement.percentCompleted);
								}
								RecheckFirstBought(j);
								break;
							}
						}
					}
				}
			});
		}

		public void RecheckFirstBought(int achievementIndex)
		{
			if (listOfAchievements[achievementIndex].isAchieved)
			{
				return;
			}
			AchieventType type = listOfAchievements[achievementIndex].type;
			switch (type)
			{
			case AchieventType.HelmetUnlock:
			{
				ClearAchievementProgress(type, achievementIndex);
				int[] allHelmetsIsBought = NewDataController.instance.GetAllHelmetsIsBought();
				for (int l = 0; l < allHelmetsIsBought.Length; l++)
				{
					if (allHelmetsIsBought[l] == 1)
					{
						AchievementProgress(AchieventType.HelmetUnlock, AchievementRegion.AllGame, 1);
					}
				}
				break;
			}
			case AchieventType.CatapultUnlock:
			{
				ClearAchievementProgress(type, achievementIndex);
				int[] allCatapultIsBought = NewDataController.instance.GetAllCatapultIsBought();
				for (int j = 0; j < allCatapultIsBought.Length; j++)
				{
					if (allCatapultIsBought[j] == 1)
					{
						AchievementProgress(AchieventType.CatapultUnlock, AchievementRegion.AllGame, 1);
					}
				}
				break;
			}
			case AchieventType.DestroyShieldUnlock:
			{
				ClearAchievementProgress(type, achievementIndex);
				int[] allShieldsIsBought2 = NewDataController.instance.GetAllShieldsIsBought();
				for (int k = 0; k < allShieldsIsBought2.Length; k++)
				{
					if (allShieldsIsBought2[k] != 0)
					{
						switch (UpgradeShopData.instance.GetShieldType(k))
						{
						case ShieldTypes.None:
							UnityEngine.Debug.LogError("There is no shield type at index - " + k);
							break;
						case ShieldTypes.FullDestroy:
						case ShieldTypes.SpoonDestroy:
						case ShieldTypes.SpinDestroy:
							AchievementProgress(AchieventType.DestroyShieldUnlock, AchievementRegion.AllGame, 1);
							break;
						}
					}
				}
				break;
			}
			case AchieventType.ReflectShieldUnlock:
			{
				ClearAchievementProgress(type, achievementIndex);
				int[] allShieldsIsBought = NewDataController.instance.GetAllShieldsIsBought();
				for (int i = 0; i < allShieldsIsBought.Length; i++)
				{
					if (allShieldsIsBought[i] != 0)
					{
						switch (UpgradeShopData.instance.GetShieldType(i))
						{
						case ShieldTypes.None:
							UnityEngine.Debug.LogError("There is no shield type at index - " + i);
							break;
						case ShieldTypes.FullReflect:
						case ShieldTypes.SpoonReflect:
						case ShieldTypes.SpinReflect:
							AchievementProgress(AchieventType.ReflectShieldUnlock, AchievementRegion.AllGame, 1);
							break;
						}
					}
				}
				break;
			}
			}
		}

		private void ClearAchievementProgress(AchieventType type, int achievementIndex)
		{
			switch (type)
			{
			case AchieventType.HelmetUnlock:
			{
				int num3 = 0;
				while (true)
				{
					if (num3 < listOfAchievements.Length)
					{
						if (type == listOfAchievements[num3].type && achievementIndex == num3)
						{
							break;
						}
						num3++;
						continue;
					}
					return;
				}
				listOfAchievements[num3].currentProgress = 0;
				break;
			}
			case AchieventType.CatapultUnlock:
			{
				int num2 = 0;
				while (true)
				{
					if (num2 < listOfAchievements.Length)
					{
						if (type == listOfAchievements[num2].type && achievementIndex == num2)
						{
							break;
						}
						num2++;
						continue;
					}
					return;
				}
				listOfAchievements[num2].currentProgress = 0;
				break;
			}
			case AchieventType.DestroyShieldUnlock:
			{
				int num4 = 0;
				while (true)
				{
					if (num4 < listOfAchievements.Length)
					{
						if (type == listOfAchievements[num4].type && achievementIndex == num4)
						{
							break;
						}
						num4++;
						continue;
					}
					return;
				}
				listOfAchievements[num4].currentProgress = 0;
				break;
			}
			case AchieventType.ReflectShieldUnlock:
			{
				int num = 0;
				while (true)
				{
					if (num < listOfAchievements.Length)
					{
						if (type == listOfAchievements[num].type && achievementIndex == num)
						{
							break;
						}
						num++;
						continue;
					}
					return;
				}
				listOfAchievements[num].currentProgress = 0;
				break;
			}
			}
		}

		private void CheckVisitAchievement()
		{
			try
			{
				if (!PlayerPrefs.HasKey("LastDate"))
				{
					PlayerPrefs.SetString("LastDate", DateTime.Today.ToString());
					UnityEngine.Debug.Log("Last visit date saved -> " + DateTime.Today.ToString());
					AchievementProgress(AchieventType.VisiteGame, AchievementRegion.AllGame, 1);
				}
				else
				{
					DateTime d = DateTime.Parse(PlayerPrefs.GetString("LastDate"));
					DateTime today = DateTime.Today;
					if (!(today == d))
					{
						TimeSpan t = today - d;
						TimeSpan t2 = new TimeSpan(1, 0, 0, 0, 0);
						if (t > t2)
						{
							ResetVisitAchievements();
							UnityEngine.Debug.Log("Resseting visiting achievement to date -> " + DateTime.Today.ToString());
							PlayerPrefs.SetString("LastDate", DateTime.Today.ToString());
							AchievementProgress(AchieventType.VisiteGame, AchievementRegion.AllGame, 1);
						}
						else
						{
							PlayerPrefs.SetString("LastDate", DateTime.Today.ToString());
							UnityEngine.Debug.Log("Last visit date saved -> " + DateTime.Today.ToString());
							AchievementProgress(AchieventType.VisiteGame, AchievementRegion.AllGame, 1);
						}
					}
				}
			}
			catch
			{
				UnityEngine.Debug.LogError("Cannot wotk with visiting achievement");
			}
		}

		public void SetDataFromSaveFile(int index, int progress, int achieved, int repeat)
		{
			if (index < listOfAchievements.Length && !listOfAchievements[index].isAchieved && !listOfAchievements[index].isIncrement)
			{
				listOfAchievements[index].currentProgress = progress;
				listOfAchievements[index].currentRepeatCount = repeat;
				SaveAchievement(index);
				SaveAchievementProgress(index);
				RecheckFirstBought(index);
			}
		}

		public void FillNonSavedData(int savedDataCount)
		{
			for (int i = savedDataCount; i < listOfAchievements.Length; i++)
			{
				listOfAchievements[i].currentProgress = 0;
				listOfAchievements[i].currentRepeatCount = 0;
				listOfAchievements[i].isAchieved = false;
				SaveAchievement(i);
				SaveAchievementProgress(i);
				RecheckFirstBought(i);
			}
		}

		private void LoadAchievementDataFromPP(bool needRecheckBought = false)
		{
			for (int i = 0; i < listOfAchievements.Length; i++)
			{
				try
				{
					if (!PlayerPrefs.HasKey(achievementProgressSaveString + i))
					{
						ResetAchievementProgress(i);
						SaveAchievementProgress(i);
					}
					else
					{
						listOfAchievements[i].currentProgress = PlayerPrefs.GetInt(achievementProgressSaveString + i);
					}
				}
				catch
				{
					My_GoogleAnalytics.Instance.Send_DebugLogInAnalytic("Cannot load achievement progress" + i + " data");
					ResetAchievementProgress(i);
				}
				try
				{
					if (!PlayerPrefs.HasKey(achievementSaveString + i))
					{
						ResetAchievement(i);
						SaveAchievement(i);
						if (needRecheckBought)
						{
							RecheckFirstBought(i);
						}
					}
					else if (!listOfAchievements[i].isRepeatable)
					{
						listOfAchievements[i].isAchieved = (PlayerPrefs.GetInt(achievementSaveString + i) == 1);
					}
					else
					{
						listOfAchievements[i].currentRepeatCount = PlayerPrefs.GetInt(achievementSaveString + i);
						if (listOfAchievements[i].currentRepeatCount >= listOfAchievements[i].maxRepeatCount)
						{
							listOfAchievements[i].isAchieved = true;
						}
					}
				}
				catch
				{
					My_GoogleAnalytics.Instance.Send_DebugLogInAnalytic("Cannot load achievement " + i + " data");
					ResetAchievement(i);
				}
			}
		}

		private void ResetAchievementProgress(int index)
		{
			listOfAchievements[index].currentProgress = 0;
		}

		private void SaveAchievementProgress(int index)
		{
			PlayerPrefs.SetInt(achievementProgressSaveString + index, listOfAchievements[index].currentProgress);
		}

		private void ResetAchievement(int index)
		{
			listOfAchievements[index].currentRepeatCount = 0;
			listOfAchievements[index].isAchieved = false;
		}

		private void SaveAchievement(int index)
		{
			if (!listOfAchievements[index].isRepeatable)
			{
				PlayerPrefs.SetInt(achievementSaveString + index, listOfAchievements[index].isAchieved ? 1 : 0);
			}
			else if (!listOfAchievements[index].isAchieved)
			{
				PlayerPrefs.SetInt(achievementSaveString + index, listOfAchievements[index].currentRepeatCount);
			}
			else
			{
				PlayerPrefs.SetInt(achievementSaveString + index, listOfAchievements[index].maxRepeatCount);
			}
		}

		public void SaveAllAchievements()
		{
			for (int i = 0; i < listOfAchievements.Length; i++)
			{
				SaveAchievement(i);
				SaveAchievementProgress(i);
			}
		}

		public void AchievementProgress(AchieventType type, AchievementRegion region, int value)
		{
			if (!Unity_SavedGame.Instance.GetStatus_PlayService() || SceneManager.GetActiveScene().name == "PvPScene")
			{
				return;
			}
			for (int i = 0; i < listOfAchievements.Length; i++)
			{
				if (type != listOfAchievements[i].type || listOfAchievements[i].isAchieved)
				{
					continue;
				}
				if (!listOfAchievements[i].isIncrement || SceneManager.GetActiveScene().name != "GameScene")
				{
					if (region == AchievementRegion.AllGame && listOfAchievements[i].region == region)
					{
						listOfAchievements[i].currentProgress += value;
						SaveAchievementProgress(i);
						if (listOfAchievements[i].currentProgress >= listOfAchievements[i].targetProgress && !listOfAchievements[i].isRepeatable)
						{
							SingleStepAchieved(i);
						}
					}
					else if (region == AchievementRegion.OneRound)
					{
						listOfAchievements[i].currentProgress += value;
						if (listOfAchievements[i].region == AchievementRegion.AllGame)
						{
							SaveAchievementProgress(i);
						}
						if (listOfAchievements[i].currentProgress >= listOfAchievements[i].targetProgress && !listOfAchievements[i].isRepeatable)
						{
							SingleStepAchieved(i);
						}
					}
				}
				else
				{
					AddRoundData(i, value);
				}
			}
		}

		private void SingleStepAchieved(int index)
		{
			Social.LoadAchievements(delegate(IAchievement[] achievements)
			{
				if (achievements.Length > 0)
				{
					foreach (IAchievement achievement in achievements)
					{
						if (achievement.id == listOfAchievements[index].achievementID && !achievement.completed)
						{
							Unlock_Achievement(listOfAchievements[index].achievementID, isSimple: true);
						}
					}
				}
			});
		}

		public void Unlock_Achievement(string achievementID, bool isSimple)
		{
			if (isSimple)
			{
				Social.ReportProgress(achievementID, 100.0, delegate
				{
					RecieveReward(achievementID);
				});
			}
		}

		public void RecieveReward(string achievementId)
		{
			for (int i = 0; i < listOfAchievements.Length; i++)
			{
				if (listOfAchievements[i].achievementID == achievementId)
				{
					listOfAchievements[i].isAchieved = true;
					SaveAchievement(i);
					UnityEngine.Debug.Log("Reward (" + listOfAchievements[i].reward + ") recieved by -> " + listOfAchievements[i].name);
					NewDataController.instance.AddMoney(listOfAchievements[i].reward);
					if (SceneManager.GetActiveScene().name == "GameScene" && GameMenuControl.instance != null)
					{
						GameMenuControl.instance.RefreshCoins();
					}
					if (SceneManager.GetActiveScene().name == "PlayModeSelect")
					{
						Unity_PurchaseButtonController.Instance.Update_TextMoneyCount();
					}
				}
			}
		}

		public void ResetOneRoundAchievements()
		{
			for (int i = 0; i < listOfAchievements.Length; i++)
			{
				if (listOfAchievements[i].region == AchievementRegion.OneRound && !listOfAchievements[i].isAchieved)
				{
					listOfAchievements[i].currentProgress = 0;
				}
			}
		}

		public void ResetVisitAchievements()
		{
			for (int i = 0; i < listOfAchievements.Length; i++)
			{
				if (listOfAchievements[i].type == AchieventType.VisiteGame && !listOfAchievements[i].isAchieved)
				{
					listOfAchievements[i].currentProgress = 0;
				}
			}
		}

		public bool GetAchievementIsCompleted(string id)
		{
			for (int i = 0; i < listOfAchievements.Length; i++)
			{
				if (listOfAchievements[i].achievementID == id)
				{
					if (listOfAchievements[i].isAchieved)
					{
						return true;
					}
					return false;
				}
			}
			return false;
		}

		public void PrepareRoundData()
		{
			if (Unity_SavedGame.Instance.GetStatus_PlayService())
			{
				if (roundProgress == null)
				{
					roundProgress = new int[listOfAchievements.Length];
				}
				for (int i = 0; i < roundProgress.Length; i++)
				{
					roundProgress[i] = 0;
				}
			}
		}

		private void AddRoundData(int achievement, int value)
		{
			if (Unity_SavedGame.Instance.GetStatus_PlayService())
			{
				roundProgress[achievement] += value;
			}
		}

		public void CheckEndOfRound()
		{
			if (!Unity_SavedGame.Instance.GetStatus_PlayService())
			{
				return;
			}
			UnityEngine.Debug.Log("-------- Sending round data --------");
			for (int i = 0; i < listOfAchievements.Length; i++)
			{
				if (!listOfAchievements[i].isAchieved && roundProgress[i] > 0 && listOfAchievements[i].isIncrement)
				{
					if (listOfAchievements[i].region == AchievementRegion.AllGame)
					{
						SendIncrementAchievementData(listOfAchievements[i].achievementID, roundProgress[i]);
					}
					else if (roundProgress[i] >= listOfAchievements[i].targetProgress)
					{
						SendIncrementAchievementData(listOfAchievements[i].achievementID, roundProgress[i]);
					}
				}
			}
		}

		private void SendIncrementAchievementData(string achievementId, int value)
		{
			PlayGamesPlatform.Instance.IncrementAchievement(achievementId, value, delegate
			{
				SavedToService(achievementId, value);
			});
		}

		private void SavedToService(string achievementId, int value)
		{
			int num = 0;
			while (true)
			{
				if (num < listOfAchievements.Length)
				{
					if (listOfAchievements[num].achievementID == achievementId)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			if (listOfAchievements[num].region == AchievementRegion.AllGame)
			{
				listOfAchievements[num].currentProgress += value;
				SaveAchievementProgress(num);
				if (listOfAchievements[num].currentProgress >= listOfAchievements[num].targetProgress)
				{
					UnityEngine.Debug.Log("Service returned value. Possible completion -> " + listOfAchievements[num].name + "(" + listOfAchievements[num].achievementID + ")");
					CheckAchievmentCompletion(achievementId);
				}
			}
			else if (value >= listOfAchievements[num].targetProgress)
			{
				UnityEngine.Debug.Log("Service returned value. Possible completion -> " + listOfAchievements[num].name + "(" + listOfAchievements[num].achievementID + ")");
				CheckAchievmentCompletion(achievementId);
			}
		}

		private void CheckAchievmentCompletion(string achievementId)
		{
			Social.LoadAchievements(delegate(IAchievement[] achievements)
			{
				if (achievements.Length > 0)
				{
					foreach (IAchievement achievement in achievements)
					{
						if (achievement.id == achievementId && achievement.completed)
						{
							UnityEngine.Debug.Log("Service returned completed achievemnt -> " + achievementId);
							RecieveReward(achievementId);
						}
					}
				}
			});
		}
	}
}
