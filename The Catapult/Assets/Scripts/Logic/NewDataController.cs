using Assets.All_Scripts;
using DG.Tweening;
using Model;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Logic
{
	public class NewDataController : MonoBehaviour
	{
		public static NewDataController instance;

		public bool debug_mode;

		public bool deleteAllPlayerPrefs;

		private string moneyPost5 = "Money";

		private string aimingPost5 = "AimingHelp";

		private string controlPost5 = "ControlHand";

		private string notificationsPost5 = "PushNotificatons";

		private string catapultPre5 = "catapult_";

		private string catapultPost5 = "catapult/";

		private string selectedCatapultPost5 = "CurrentCatapult";

		private string helmetPost5 = "helmets_";

		private string selectedHelmetPost5 = "CurrentHelmet";

		private string shieldPost5 = "shield_";

		private string SelectedShieldPost5 = "CurrentShield";

		private bool pre5Present;

		private bool post5Present;

		private bool newPresent;

		private Post5SaveData post5 = new Post5SaveData();

		private Pre5SaveData pre5 = new Pre5SaveData();

		private string playerMoneySaveString = "playerMoney";

		private string playerAimingSaveString = "playerAiming";

		private string playerControlHandSaveString = "playerControl";

		private string playerNotificationsSaveString = "playerNotify";

		private string canBuyCatapultSaveString = "cat_c_";

		private string isCatapultBoughtSaveString = "cat_b_";

		private string catapultUpgradeSaveString = "cat_u_";

		private string currentCatapultSaveString = "equip_cat";

		private string isHelmetBoughtSaveString = "helm_b_";

		private string currentHelmetSaveString = "equip_helm";

		private string isShieldBoughtSaveString = "shield_b";

		private string currentShieldSaveString = "equip_shield";

		private int playerMoney;

		private AimingTipType playerAiming;

		private ControlType playerControl;

		private int playerNotifications;

		private GameMode _gameMode;

		private int currentEquipedCatapult;

		private int currentEquipedHelmet;

		private int currentEquipedShield;

		private int[] canBuyCatapult;

		private int[] isCatapultBought;

		private int[] catapultUpgrade;

		private int[] isHelmetBought;

		private int[] isShieldBought;

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
				UnityEngine.Object.DontDestroyOnLoad(this);
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		private void Start()
		{
			PrepareGameArrays();
			CheckOldSaveSystems();
			if ((!post5Present && !pre5Present) || newPresent)
			{
				LoadGameDataFromPlayerPrefs();
			}
			SetGameMode(GameMode.None);
		}

		private void PrepareGameArrays()
		{
			canBuyCatapult = new int[3];
			isCatapultBought = new int[3];
			catapultUpgrade = new int[3];
			isHelmetBought = new int[UpgradeShopData.instance.GetHelmetCount()];
			isShieldBought = new int[UpgradeShopData.instance.GetShieldCount()];
			currentEquipedCatapult = 999;
			currentEquipedHelmet = 999;
			currentEquipedShield = 999;
		}

		private void LoadGameDataFromPlayerPrefs()
		{
			LoadPlayerMoney();
			LoadPlayerAimingFromPlayerPrefs();
			LoadPlayerControlHandFromPlayerPrefs();
			LoadPlayerNotificationFromPlayerPrefs();
			LoadCatapultProgressFromPlayerPrefs();
			LoadSelectedCatapultFromPlayerPrefs();
			LoadHelmetsProgressFromPlayerPrefs();
			LoadSelectedHelmetFromPlayerPrefs();
			LoadShieldProgressFromPlayerPrefs();
			LoadSelectedShieldFromPlayerPrefs();
		}

		public void SaveGameDataToPlayerPrefs()
		{
			SavePlayerMoney();
			SavePlayerAiming();
			SavePlayerControl();
			SavePlayerNotifications();
			for (int i = 0; i < canBuyCatapult.Length; i++)
			{
				SaveCanBuyCatapult(i);
				SaveCatapulIsBought(i);
				SaveCatapultUpgrade(i);
			}
			SaveCurrentCatapult();
			for (int j = 0; j < isHelmetBought.Length; j++)
			{
				SaveHelmetIsBought(j);
			}
			SaveEquipedHelmet();
			for (int k = 0; k < isShieldBought.Length; k++)
			{
				SaveShieldIsBought(k);
			}
			SaveEqupedShield();
		}

		private void LoadPlayerMoney()
		{
			try
			{
				if (!PlayerPrefs.HasKey(playerMoneySaveString))
				{
					SetPlayerMoney(0);
					SavePlayerMoney();
				}
				else
				{
					SetPlayerMoney(PlayerPrefs.GetInt(playerMoneySaveString));
				}
			}
			catch
			{
				SetPlayerMoney(0);
			}
		}

		private void SetPlayerMoney(int value)
		{
			if (value >= 0)
			{
				playerMoney = value;
			}
			else
			{
				SendDebugMessage("Readed wrong money value " + value, isCriticalError: true);
			}
		}

		public void SavePlayerMoney()
		{
			PlayerPrefs.SetInt(playerMoneySaveString, playerMoney);
		}

		public int GetPlayerMoney()
		{
			return playerMoney;
		}

		public void AddMoney(int value)
		{
			if (value < 0)
			{
				SendDebugMessage("Added negative money value - " + value);
			}
			else
			{
				playerMoney += value;
			}
		}

		public bool IsEnoughMoney(int checkoutValue)
		{
			if (checkoutValue < 0)
			{
				return false;
			}
			if (playerMoney >= checkoutValue)
			{
				return true;
			}
			return false;
		}

		public void CheckoutMoney(int checkoutValue)
		{
			if (checkoutValue < 0)
			{
				UnityEngine.Debug.LogError("Cannot checkout negative");
			}
			else
			{
				playerMoney -= checkoutValue;
			}
		}

		private void LoadPlayerAimingFromPlayerPrefs()
		{
			try
			{
				if (!PlayerPrefs.HasKey(playerAimingSaveString))
				{
					SetPlayerAiming(0);
					SavePlayerAiming();
				}
				else
				{
					SetPlayerAiming(PlayerPrefs.GetInt(playerAimingSaveString));
				}
			}
			catch
			{
				SetPlayerAiming(0);
			}
		}

		private void SetPlayerAiming(int index)
		{
			playerAiming = (AimingTipType)index;
		}

		public void SetPlayerAiming(AimingTipType type)
		{
			playerAiming = type;
		}

		public void SavePlayerAiming()
		{
			PlayerPrefs.SetInt(playerAimingSaveString, (int)playerAiming);
		}

		public AimingTipType GetPlayerAiming()
		{
			return playerAiming;
		}

		private void LoadPlayerControlHandFromPlayerPrefs()
		{
			try
			{
				if (!PlayerPrefs.HasKey(playerControlHandSaveString))
				{
					SetPlayerControl(0);
					SavePlayerControl();
				}
				else
				{
					SetPlayerControl(PlayerPrefs.GetInt(playerControlHandSaveString));
				}
			}
			catch
			{
				SetPlayerControl(0);
			}
		}

		private void SetPlayerControl(int index)
		{
			playerControl = (ControlType)index;
		}

		public void SetPlayerControl(ControlType type)
		{
			playerControl = type;
		}

		public void SavePlayerControl()
		{
			PlayerPrefs.SetInt(playerControlHandSaveString, (int)playerControl);
		}

		public ControlType GetPlayerControl()
		{
			return playerControl;
		}

		private void LoadPlayerNotificationFromPlayerPrefs()
		{
			try
			{
				if (!PlayerPrefs.HasKey(playerNotificationsSaveString))
				{
					SetPlayerNotifications(1);
					SavePlayerNotifications();
					StartNotificationsWork();
				}
				else
				{
					SetPlayerNotifications(PlayerPrefs.GetInt(playerNotificationsSaveString));
					StartNotificationsWork();
				}
			}
			catch
			{
				SetPlayerNotifications(0);
			}
		}

		private void StartNotificationsWork()
		{
			if (GetPlayerNotifications() == 1)
			{
				DOVirtual.DelayedCall(2f, delegate
				{
					BYV_Notification.instance.StartNotificationWork();
				});
			}
		}

		private void StopNotificationsWork()
		{
			if (GetPlayerNotifications() == 0)
			{
				BYV_Notification.instance.StopNotificataionWork();
			}
		}

		private void SetPlayerNotifications(int value)
		{
			if (value == 0 || value == 1)
			{
				playerNotifications = value;
			}
		}

		public void SavePlayerNotifications()
		{
			PlayerPrefs.SetInt(playerNotificationsSaveString, playerNotifications);
		}

		public int GetPlayerNotifications()
		{
			return playerNotifications;
		}

		public void EnableNotifications()
		{
			playerNotifications = 1;
			StartNotificationsWork();
		}

		public void DisableNotifications()
		{
			playerNotifications = 0;
			StopNotificationsWork();
		}

		private void LoadCatapultProgressFromPlayerPrefs()
		{
			for (int i = 0; i < canBuyCatapult.Length; i++)
			{
				try
				{
					if (!PlayerPrefs.HasKey(canBuyCatapultSaveString + i))
					{
						if (i == 0)
						{
							SetCanBuyCatapult(i, 1);
						}
						else
						{
							SetCanBuyCatapult(i, 0);
						}
						SaveCanBuyCatapult(i);
					}
					else
					{
						SetCanBuyCatapult(i, PlayerPrefs.GetInt(canBuyCatapultSaveString + i));
					}
				}
				catch
				{
					if (i == 0)
					{
						SetCanBuyCatapult(i, 1);
					}
					else
					{
						SetCanBuyCatapult(i, 0);
					}
				}
				try
				{
					if (!PlayerPrefs.HasKey(isCatapultBoughtSaveString + i))
					{
						if (i == 0)
						{
							SetCatapultIsBought(i, 1);
						}
						else
						{
							SetCatapultIsBought(i, 0);
						}
						SaveCatapulIsBought(i);
					}
					else
					{
						SetCatapultIsBought(i, PlayerPrefs.GetInt(isCatapultBoughtSaveString + i));
					}
				}
				catch
				{
					if (i == 0)
					{
						SetCatapultIsBought(i, 1);
					}
					else
					{
						SetCatapultIsBought(i, 0);
					}
				}
				try
				{
					if (!PlayerPrefs.HasKey(catapultUpgradeSaveString + i))
					{
						SetCatapultUpgrade(i, 0);
						SaveCatapultUpgrade(i);
					}
					else
					{
						SetCatapultUpgrade(i, PlayerPrefs.GetInt(catapultUpgradeSaveString + i));
					}
				}
				catch
				{
					SetCatapultUpgrade(i, 0);
				}
			}
		}

		private bool CheckCatapultIndex(int index)
		{
			if (index < 0 || index >= canBuyCatapult.Length)
			{
				SendDebugMessage("Wrong catapult index " + index);
				return false;
			}
			return true;
		}

		private int CheckAndFixUpgradeIndex(int index, int value)
		{
			if (value < 0)
			{
				return 0;
			}
			if (value > UpgradeShopData.instance.GetUpgradeLength(index) - 1)
			{
				return UpgradeShopData.instance.GetUpgradeLength(index) - 1;
			}
			return value;
		}

		private void SetCanBuyCatapult(int index, int value)
		{
			if (CheckCatapultIndex(index))
			{
				if (value != 0 && value != 1)
				{
					SendDebugMessage("Wrong can buy state for " + index + "=>" + value);
				}
				else
				{
					canBuyCatapult[index] = value;
				}
			}
		}

		private void SaveCanBuyCatapult(int index)
		{
			if (CheckCatapultIndex(index))
			{
				PlayerPrefs.SetInt(canBuyCatapultSaveString + index, canBuyCatapult[index]);
			}
		}

		public int CanBuyCatapult(int index)
		{
			if (!CheckCatapultIndex(index))
			{
				return 0;
			}
			return canBuyCatapult[index];
		}

		public int[] GetAllCanBuyCatapult()
		{
			int[] array = new int[canBuyCatapult.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = canBuyCatapult[i];
			}
			return array;
		}

		private void UnlockNextCatapult(int current)
		{
			if (CheckCatapultIndex(current + 1))
			{
				SetCanBuyCatapult(current + 1, 1);
				SaveCanBuyCatapult(current + 1);
			}
		}

		private void SetCatapultIsBought(int index, int value)
		{
			if (CheckCatapultIndex(index))
			{
				if (value != 0 && value != 1)
				{
					SendDebugMessage("Wrong can buy state for " + index + "=>" + value);
				}
				else
				{
					isCatapultBought[index] = value;
				}
			}
		}

		public void SaveCatapulIsBought(int index)
		{
			if (CheckCatapultIndex(index))
			{
				PlayerPrefs.SetInt(isCatapultBoughtSaveString + index, isCatapultBought[index]);
			}
		}

		public int IsCatapultBought(int index)
		{
			if (!CheckCatapultIndex(index))
			{
				return 0;
			}
			return isCatapultBought[index];
		}

		public void BuyCatapult(int index)
		{
			if (CanBuyCatapult(index) == 1 && IsCatapultBought(index) == 0)
			{
				isCatapultBought[index] = 1;
			}
		}

		public int[] GetAllCatapultIsBought()
		{
			int[] array = new int[isCatapultBought.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = isCatapultBought[i];
			}
			return array;
		}

		private void SetCatapultUpgrade(int index, int value)
		{
			if (CheckCatapultIndex(index))
			{
				if (value < 0 || value >= UpgradeShopData.instance.GetUpgradeLength(index))
				{
					SendDebugMessage("There is no upgrade index - " + value);
				}
				else
				{
					catapultUpgrade[index] = value;
				}
			}
		}

		private void SaveCatapultUpgrade(int index)
		{
			if (CheckCatapultIndex(index))
			{
				PlayerPrefs.SetInt(catapultUpgradeSaveString + index, catapultUpgrade[index]);
			}
		}

		public int GetCatapultUpgrade(int index)
		{
			if (!CheckCatapultIndex(index))
			{
				return -1;
			}
			return catapultUpgrade[index];
		}

		public int GetCurrentCatapultUpgrade()
		{
			int currentCatapultIndex = GetCurrentCatapultIndex();
			return catapultUpgrade[currentCatapultIndex];
		}

		public int[] GetAllCatapultUpgrades()
		{
			int[] array = new int[catapultUpgrade.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = catapultUpgrade[i];
			}
			return array;
		}

		public bool CanUpgradeCurrentCatapult()
		{
			if (GetCurrentCatapultUpgrade() < UpgradeShopData.instance.GetUpgradeLength(GetCurrentCatapultIndex()) - 1)
			{
				return true;
			}
			return false;
		}

		private void UpgradeToMaximum(int index)
		{
			if (CheckCatapultIndex(index))
			{
				SetCatapultUpgrade(index, UpgradeShopData.instance.GetUpgradeLength(index) - 1);
			}
		}

		private bool IsUpgradeReachedMaximim()
		{
			if (GetCurrentCatapultUpgrade() == UpgradeShopData.instance.GetUpgradeLength(GetCurrentCatapultIndex()) - 1)
			{
				return true;
			}
			return false;
		}

		public void UpgradeCurrentCatapult()
		{
			if (CanUpgradeCurrentCatapult())
			{
				catapultUpgrade[GetCurrentCatapultIndex()]++;
				if (IsUpgradeReachedMaximim())
				{
					UnlockNextCatapult(GetCurrentCatapultIndex());
				}
			}
		}

		private bool IsCatapultAtMaximumLevel(int index)
		{
			if (!CheckCatapultIndex(index))
			{
				return false;
			}
			if (GetCatapultUpgrade(index) == UpgradeShopData.instance.GetUpgradeLength(index) - 1)
			{
				return true;
			}
			return false;
		}

		public void SaveCurrentCatapultUpgrade()
		{
			SaveCatapultUpgrade(GetCurrentCatapultIndex());
		}

		private void LoadSelectedCatapultFromPlayerPrefs()
		{
			try
			{
				if (!PlayerPrefs.HasKey(currentCatapultSaveString))
				{
					SetCurrentCatapult(0);
					SaveCurrentCatapult();
				}
				else
				{
					SetCurrentCatapult(PlayerPrefs.GetInt(currentCatapultSaveString));
				}
			}
			catch
			{
				SetCurrentCatapult(0);
			}
		}

		private void SetCurrentCatapult(int index)
		{
			if (index >= 0 && index < canBuyCatapult.Length)
			{
				currentEquipedCatapult = index;
			}
			else
			{
				SendDebugMessage("There is no such catapult - " + index);
			}
		}

		public void SaveCurrentCatapult()
		{
			PlayerPrefs.SetInt(currentCatapultSaveString, currentEquipedCatapult);
		}

		public int GetCurrentCatapultIndex()
		{
			return currentEquipedCatapult;
		}

		public void EquipCatapult(int index)
		{
			if ((index >= 0 && index < canBuyCatapult.Length) || index == 999)
			{
				currentEquipedCatapult = index;
			}
			else
			{
				SendDebugMessage("There is no such catapult - " + index);
			}
		}

		private void LoadHelmetsProgressFromPlayerPrefs()
		{
			for (int i = 0; i < isHelmetBought.Length; i++)
			{
				try
				{
					if (!PlayerPrefs.HasKey(isHelmetBoughtSaveString + i))
					{
						SetHelmetIsBought(i, 0);
						SaveHelmetIsBought(i);
					}
					else
					{
						SetHelmetIsBought(i, PlayerPrefs.GetInt(isHelmetBoughtSaveString + i));
					}
				}
				catch
				{
					SetHelmetIsBought(i, 0);
				}
			}
		}

		private bool CheckHelmetIndex(int index)
		{
			if (index < 0 || index >= isHelmetBought.Length)
			{
				return false;
			}
			return true;
		}

		private void SetHelmetIsBought(int index, int value)
		{
			if (CheckHelmetIndex(index) && (value == 0 || value == 1))
			{
				isHelmetBought[index] = value;
			}
		}

		public void SaveHelmetIsBought(int index)
		{
			if (CheckHelmetIndex(index))
			{
				PlayerPrefs.SetInt(isHelmetBoughtSaveString + index, isHelmetBought[index]);
			}
		}

		public bool IsHelmetBought(int index)
		{
			if (!CheckHelmetIndex(index))
			{
				return false;
			}
			return isHelmetBought[index] == 1;
		}

		public void BuyHelmet(int index)
		{
			if (CheckHelmetIndex(index))
			{
				isHelmetBought[index] = 1;
			}
		}

		public int[] GetAllHelmetsIsBought()
		{
			int[] array = new int[isHelmetBought.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = isHelmetBought[i];
			}
			return array;
		}

		private void LoadSelectedHelmetFromPlayerPrefs()
		{
			try
			{
				if (!PlayerPrefs.HasKey(currentHelmetSaveString))
				{
					SetEquipedHelmet(999);
					SaveEquipedHelmet();
				}
				else
				{
					SetEquipedHelmet(PlayerPrefs.GetInt(currentHelmetSaveString));
				}
			}
			catch
			{
				SetEquipedHelmet(999);
			}
		}

		private void SetEquipedHelmet(int index)
		{
			if (CheckHelmetIndex(index) || index == 999)
			{
				currentEquipedHelmet = index;
			}
			else
			{
				SendDebugMessage("There is no such helmet - " + index);
			}
		}

		public void SaveEquipedHelmet()
		{
			PlayerPrefs.SetInt(currentHelmetSaveString, currentEquipedHelmet);
		}

		public int GetEquipedHelmet()
		{
			return currentEquipedHelmet;
		}

		public void EquipHelmet(int index)
		{
			if (CheckHelmetIndex(index) || index == 999)
			{
				currentEquipedHelmet = index;
			}
		}

		private void LoadShieldProgressFromPlayerPrefs()
		{
			for (int i = 0; i < isShieldBought.Length; i++)
			{
				try
				{
					if (!PlayerPrefs.HasKey(isShieldBoughtSaveString + i))
					{
						SetShieldIsBought(i, 0);
						SaveShieldIsBought(i);
					}
					else
					{
						SetShieldIsBought(i, PlayerPrefs.GetInt(isShieldBoughtSaveString + i));
					}
				}
				catch
				{
					SetShieldIsBought(i, 0);
				}
			}
		}

		private bool CheckShieldIndex(int index)
		{
			if (index < 0 || index >= isShieldBought.Length)
			{
				return false;
			}
			return true;
		}

		private void SetShieldIsBought(int index, int value)
		{
			if (CheckShieldIndex(index) && (value == 0 || value == 1))
			{
				isShieldBought[index] = value;
			}
		}

		public void SaveShieldIsBought(int index)
		{
			if (CheckShieldIndex(index))
			{
				PlayerPrefs.SetInt(isShieldBoughtSaveString + index, isShieldBought[index]);
			}
		}

		public bool GetShieldIsBought(int index)
		{
			if (!CheckShieldIndex(index))
			{
				return false;
			}
			return isShieldBought[index] == 1;
		}

		public void BuyShield(int index)
		{
			if (CheckShieldIndex(index))
			{
				isShieldBought[index] = 1;
			}
		}

		public int[] GetAllShieldsIsBought()
		{
			int[] array = new int[isShieldBought.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = isShieldBought[i];
			}
			return array;
		}

		private void LoadSelectedShieldFromPlayerPrefs()
		{
			try
			{
				if (!PlayerPrefs.HasKey(currentShieldSaveString))
				{
					SetEquipedShield(999);
					SaveEqupedShield();
				}
				else
				{
					SetEquipedShield(PlayerPrefs.GetInt(currentShieldSaveString));
				}
			}
			catch
			{
				SetEquipedShield(999);
			}
		}

		private void SetEquipedShield(int index)
		{
			if (CheckShieldIndex(index) || index == 999)
			{
				currentEquipedShield = index;
			}
		}

		public void SaveEqupedShield()
		{
			PlayerPrefs.SetInt(currentShieldSaveString, currentEquipedShield);
		}

		public int GetEquipedShield()
		{
			return currentEquipedShield;
		}

		public void EquipShield(int index)
		{
			if (CheckShieldIndex(index))
			{
				currentEquipedShield = index;
			}
		}

		public void ClearShieldSlot()
		{
			currentEquipedShield = 999;
		}

		public void SetGameMode(GameMode mode)
		{
			_gameMode = mode;
		}

		public GameMode GetGameMode()
		{
			return _gameMode;
		}

		private void SendDebugMessage(string message, bool isCriticalError = false)
		{
			if (debug_mode || isCriticalError)
			{
				string str = "InitScene";
				if (Status_Scene.Inst != null)
				{
					str = SceneManager.GetActiveScene().name;
				}
				if (!isCriticalError)
				{
					UnityEngine.Debug.Log("(MESSAGE)(Scene:" + str + ")" + message);
					return;
				}
				string message2 = "(ERROR)(Scene:" + str + ")" + message;
				UnityEngine.Debug.LogError(message2);
				My_GoogleAnalytics.Instance.Send_DebugLogInAnalytic(message2);
			}
		}

		private void CheckOldSaveSystems()
		{
			newPresent = false;
			post5Present = false;
			pre5Present = false;
			if (PlayerPrefs.HasKey(catapultUpgradeSaveString + 0))
			{
				newPresent = true;
			}
			else if (PlayerPrefs.HasKey(catapultPost5 + 0))
			{
				post5Present = true;
				LoadFromOldSaves();
			}
			else if (PlayerPrefs.HasKey(catapultPre5 + 0) || PlayerPrefs.HasKey("CatapultLevel"))
			{
				pre5Present = true;
				LoadFromOldSaves();
			}
			else
			{
				newPresent = true;
			}
		}

		private void LoadFromOldSaves()
		{
			if (!post5Present && pre5Present)
			{
				LoadPre5();
				Pre5Post5Transmit();
				Post5CurrentTransmit();
			}
			else if (post5Present)
			{
				LoadPost5();
				Post5CurrentTransmit();
			}
		}

		private void LoadPre5()
		{
			try
			{
				if (PlayerPrefs.HasKey("CatapultLevel"))
				{
					pre5.catapultLevel = PlayerPrefs.GetInt("CatapultLevel");
				}
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogError("(BYV)" + ex.ToString());
			}
			try
			{
				if (PlayerPrefs.HasKey("Evolve"))
				{
					pre5.catapultType = PlayerPrefs.GetInt("Evolve");
				}
			}
			catch (Exception ex2)
			{
				UnityEngine.Debug.LogError("(BYV)" + ex2.ToString());
			}
			for (int i = 0; i < pre5.catapultProgress.Length; i++)
			{
				try
				{
					if (PlayerPrefs.HasKey(catapultPre5 + i))
					{
						pre5.catapultProgress[i] = PlayerPrefs.GetInt(catapultPre5 + i);
					}
				}
				catch (Exception ex3)
				{
					UnityEngine.Debug.LogError("(BYV)" + ex3.ToString());
				}
			}
			try
			{
				if (!PlayerPrefs.HasKey(catapultPre5 + 0))
				{
					pre5.catapultLevel++;
					pre5.catapultProgress[0] = pre5.catapultLevel;
				}
			}
			catch (Exception ex4)
			{
				UnityEngine.Debug.LogError("(BYV)" + ex4.ToString());
			}
			try
			{
				if (PlayerPrefs.HasKey(moneyPost5))
				{
					pre5.money = PlayerPrefs.GetFloat(moneyPost5);
					pre5.moneySet = true;
				}
			}
			catch (Exception ex5)
			{
				UnityEngine.Debug.LogError("(BYV)" + ex5.ToString());
			}
			try
			{
				if (PlayerPrefs.HasKey(aimingPost5))
				{
					pre5.aiming = PlayerPrefs.GetInt(aimingPost5);
					pre5.aimingSet = true;
				}
			}
			catch (Exception ex6)
			{
				UnityEngine.Debug.LogError("(BYV)" + ex6.ToString());
			}
			try
			{
				if (PlayerPrefs.HasKey(controlPost5))
				{
					pre5.control = PlayerPrefs.GetInt(controlPost5);
					pre5.ControlSet = true;
				}
			}
			catch (Exception ex7)
			{
				UnityEngine.Debug.LogError("(BYV)" + ex7.ToString());
			}
			try
			{
				if (PlayerPrefs.HasKey(selectedHelmetPost5))
				{
					pre5.currentHelmet = PlayerPrefs.GetInt(selectedHelmetPost5);
					pre5.helmetSetted = true;
				}
			}
			catch (Exception ex8)
			{
				UnityEngine.Debug.LogError("(BYV)" + ex8.ToString());
			}
			if (pre5.helmetSetted)
			{
				for (int j = 0; j < pre5.helmetStates.Length; j++)
				{
					try
					{
						if (PlayerPrefs.HasKey(helmetPost5 + j))
						{
							pre5.helmetStates[j] = PlayerPrefs.GetInt(helmetPost5 + j);
							pre5.helmetStatesSetted[j] = true;
						}
					}
					catch (Exception ex9)
					{
						UnityEngine.Debug.LogError("(BYV)" + ex9.ToString());
					}
				}
			}
			try
			{
				if (PlayerPrefs.HasKey(SelectedShieldPost5))
				{
					pre5.currentShield = PlayerPrefs.GetInt(SelectedShieldPost5);
					pre5.shieldSetted = true;
				}
			}
			catch (Exception ex10)
			{
				UnityEngine.Debug.LogError("(BYV)" + ex10.ToString());
			}
			if (pre5.shieldSetted)
			{
				for (int k = 0; k < pre5.shieldStates.Length; k++)
				{
					try
					{
						if (PlayerPrefs.HasKey(shieldPost5 + k))
						{
							pre5.shieldStates[k] = PlayerPrefs.GetInt(shieldPost5 + k);
							pre5.shieldStatesSetted[k] = true;
						}
					}
					catch (Exception ex11)
					{
						UnityEngine.Debug.LogError("(BYV)" + ex11.ToString());
					}
				}
			}
		}

		private void Pre5Post5Transmit()
		{
			if (pre5.catapultType >= 0 && pre5.catapultType < 3)
			{
				post5.currentCatapult = pre5.catapultType;
			}
			for (int num = post5.catapultProgress.Length - 1; num >= 0; num--)
			{
				if (num < pre5.catapultProgress.Length && pre5.catapultProgress[num] > 0)
				{
					if (pre5.catapultProgress[num] <= 11)
					{
						post5.catapultProgress[num] = pre5.catapultProgress[num];
					}
					else
					{
						post5.catapultProgress[num] = 11;
					}
					for (int num2 = num - 1; num2 >= 0; num2--)
					{
						post5.catapultProgress[num2] = 11;
					}
					break;
				}
			}
			post5.money = pre5.money;
			post5.moneySet = pre5.moneySet;
			post5.aiming = pre5.aiming;
			post5.aimingSet = pre5.aimingSet;
			post5.control = pre5.control;
			post5.ControlSet = pre5.ControlSet;
			post5.currentHelmet = pre5.currentHelmet;
			post5.helmetSetted = pre5.helmetSetted;
			for (int i = 0; i < post5.helmetStates.Length; i++)
			{
				post5.helmetStates[i] = pre5.helmetStates[i];
				post5.helmetStatesSetted[i] = pre5.helmetStatesSetted[i];
			}
			post5.currentShield = pre5.currentShield;
			post5.shieldSetted = pre5.shieldSetted;
			for (int j = 0; j < post5.shieldStates.Length; j++)
			{
				post5.shieldStates[j] = pre5.shieldStates[j];
				post5.shieldStatesSetted[j] = pre5.shieldStatesSetted[j];
			}
		}

		private void LoadPost5()
		{
			try
			{
				if (PlayerPrefs.HasKey(selectedCatapultPost5))
				{
					post5.currentCatapult = PlayerPrefs.GetInt(selectedCatapultPost5);
				}
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogError("(BYV)" + ex.ToString());
			}
			for (int i = 0; i < post5.catapultProgress.Length; i++)
			{
				try
				{
					if (PlayerPrefs.HasKey(catapultPost5 + i))
					{
						post5.catapultProgress[i] = PlayerPrefs.GetInt(catapultPost5 + i);
					}
				}
				catch (Exception ex2)
				{
					UnityEngine.Debug.LogError("(BYV)" + ex2.ToString());
				}
			}
			try
			{
				if (PlayerPrefs.HasKey(moneyPost5))
				{
					post5.money = PlayerPrefs.GetFloat(moneyPost5);
					post5.moneySet = true;
				}
			}
			catch (Exception ex3)
			{
				UnityEngine.Debug.LogError("(BYV)" + ex3.ToString());
			}
			try
			{
				if (PlayerPrefs.HasKey(aimingPost5))
				{
					post5.aiming = PlayerPrefs.GetInt(aimingPost5);
					post5.aimingSet = true;
				}
			}
			catch (Exception ex4)
			{
				UnityEngine.Debug.LogError("(BYV)" + ex4.ToString());
			}
			try
			{
				if (PlayerPrefs.HasKey(controlPost5))
				{
					post5.control = PlayerPrefs.GetInt(controlPost5);
					post5.ControlSet = true;
				}
			}
			catch (Exception ex5)
			{
				UnityEngine.Debug.LogError("(BYV)" + ex5.ToString());
			}
			try
			{
				if (PlayerPrefs.HasKey(notificationsPost5))
				{
					post5.notify = PlayerPrefs.GetInt(notificationsPost5);
					post5.NotifySet = true;
				}
			}
			catch (Exception ex6)
			{
				UnityEngine.Debug.LogError("(BYV)" + ex6.ToString());
			}
			try
			{
				if (PlayerPrefs.HasKey(selectedHelmetPost5))
				{
					post5.currentHelmet = PlayerPrefs.GetInt(selectedHelmetPost5);
					post5.helmetSetted = true;
				}
			}
			catch (Exception ex7)
			{
				UnityEngine.Debug.LogError("(BYV)" + ex7.ToString());
			}
			for (int j = 0; j < post5.helmetStates.Length; j++)
			{
				try
				{
					if (PlayerPrefs.HasKey(helmetPost5 + j))
					{
						post5.helmetStates[j] = PlayerPrefs.GetInt(helmetPost5 + j);
						post5.helmetStatesSetted[j] = true;
					}
				}
				catch (Exception ex8)
				{
					UnityEngine.Debug.LogError("(BYV)" + ex8.ToString());
				}
			}
			try
			{
				if (PlayerPrefs.HasKey(SelectedShieldPost5))
				{
					post5.currentShield = PlayerPrefs.GetInt(SelectedShieldPost5);
					post5.shieldSetted = true;
				}
			}
			catch (Exception ex9)
			{
				UnityEngine.Debug.LogError("(BYV)" + ex9.ToString());
			}
			for (int k = 0; k < post5.shieldStates.Length; k++)
			{
				try
				{
					if (PlayerPrefs.HasKey(shieldPost5 + k))
					{
						post5.shieldStates[k] = PlayerPrefs.GetInt(shieldPost5 + k);
						post5.shieldStatesSetted[k] = true;
					}
				}
				catch (Exception ex10)
				{
					UnityEngine.Debug.LogError("(BYV)" + ex10.ToString());
				}
			}
		}

		private void Post5CurrentTransmit()
		{
			for (int num = post5.catapultProgress.Length - 1; num >= 0; num--)
			{
				if (CheckCatapultIndex(num) && post5.catapultProgress[num] > 0)
				{
					SetCanBuyCatapult(num, 1);
					SaveCanBuyCatapult(num);
					SetCatapultIsBought(num, 1);
					SaveCatapulIsBought(num);
					SetCatapultUpgrade(num, CheckAndFixUpgradeIndex(num, post5.catapultProgress[num] - 1));
					SaveCatapultUpgrade(num);
					for (int num2 = num - 1; num2 >= 0; num2--)
					{
						SetCanBuyCatapult(num2, 1);
						SetCatapultIsBought(num2, 1);
						UpgradeToMaximum(num2);
						SaveCanBuyCatapult(num2);
						SaveCatapulIsBought(num2);
						SaveCatapultUpgrade(num2);
					}
					if (IsCatapultAtMaximumLevel(num) && num + 1 < canBuyCatapult.Length)
					{
						SetCanBuyCatapult(num + 1, 1);
						SaveCanBuyCatapult(num + 1);
					}
					break;
				}
			}
			if (IsCatapultBought(0) == 0 || CanBuyCatapult(0) == 0)
			{
				SetCanBuyCatapult(0, 1);
				SaveCanBuyCatapult(0);
				SetCatapultIsBought(0, 1);
				SaveCatapulIsBought(0);
			}
			int num3 = post5.currentCatapult;
			if (num3 < 0)
			{
				num3 = 0;
			}
			else if (num3 > 2)
			{
				num3 = 2;
			}
			if (IsCatapultBought(num3) == 1)
			{
				SetCurrentCatapult(num3);
				SaveCurrentCatapult();
			}
			else
			{
				SetCurrentCatapult(0);
				SaveCurrentCatapult();
			}
			for (int i = 0; i < post5.helmetStates.Length; i++)
			{
				if (!CheckHelmetIndex(i))
				{
					continue;
				}
				if (post5.helmetStatesSetted[i])
				{
					int num4 = post5.helmetStates[i];
					if (num4 < 0)
					{
						num4 = 0;
					}
					else if (num4 > 1)
					{
						num4 = 1;
					}
					SetHelmetIsBought(i, num4);
					SaveHelmetIsBought(i);
				}
				else
				{
					SetHelmetIsBought(i, 0);
					SaveHelmetIsBought(i);
				}
			}
			if (post5.helmetSetted)
			{
				int currentHelmet = post5.currentHelmet;
				if (currentHelmet == -1)
				{
					SetEquipedHelmet(999);
					SaveEquipedHelmet();
				}
				else if (!CheckHelmetIndex(currentHelmet))
				{
					SetEquipedHelmet(999);
					SaveEquipedHelmet();
				}
				else if (IsHelmetBought(currentHelmet))
				{
					SetEquipedHelmet(currentHelmet);
					SaveEquipedHelmet();
				}
				else
				{
					SetEquipedHelmet(999);
					SaveEquipedHelmet();
				}
			}
			else
			{
				SetEquipedHelmet(999);
				SaveEquipedHelmet();
			}
			for (int j = 0; j < post5.shieldStates.Length; j++)
			{
				if (!CheckShieldIndex(j))
				{
					continue;
				}
				if (post5.shieldStatesSetted[j])
				{
					int num5 = post5.shieldStates[j];
					if (num5 < 0)
					{
						num5 = 0;
					}
					else if (num5 > 1)
					{
						num5 = 1;
					}
					SetShieldIsBought(j, num5);
					SaveShieldIsBought(j);
				}
				else
				{
					SetShieldIsBought(j, 0);
					SaveShieldIsBought(j);
				}
			}
			if (post5.shieldSetted)
			{
				int currentShield = post5.currentShield;
				if (currentShield == -1)
				{
					SetEquipedShield(999);
					SaveEqupedShield();
				}
				else if (!CheckShieldIndex(currentShield))
				{
					SetEquipedShield(999);
					SaveEqupedShield();
				}
				else if (GetShieldIsBought(currentShield))
				{
					SetEquipedShield(currentShield);
					SaveEqupedShield();
				}
				else
				{
					SetEquipedShield(999);
					SaveEqupedShield();
				}
			}
			else
			{
				SetEquipedShield(999);
				SaveEqupedShield();
			}
			if (post5.moneySet)
			{
				float num6 = post5.money;
				if (num6 < 0f)
				{
					num6 = 0f;
				}
				SetPlayerMoney((int)num6);
				SavePlayerMoney();
			}
			else
			{
				SetPlayerMoney(0);
				SavePlayerMoney();
			}
			if (post5.aimingSet)
			{
				int aiming = post5.aiming;
				SetPlayerAiming(aiming);
				SavePlayerAiming();
			}
			else
			{
				SetPlayerAiming(0);
				SavePlayerAiming();
			}
			if (post5.ControlSet)
			{
				int control = post5.control;
				SetPlayerControl(control);
				SavePlayerControl();
			}
			else
			{
				SetPlayerControl(0);
				SavePlayerControl();
			}
			if (post5.NotifySet)
			{
				int notify = post5.notify;
				SetPlayerNotifications(notify);
				SavePlayerNotifications();
				StartNotificationsWork();
			}
			else
			{
				SetPlayerNotifications(1);
				SavePlayerNotifications();
				StartNotificationsWork();
			}
		}

		public void LoadMainDataFromSave(int money, int aiming, int control, int notify)
		{
			SetPlayerMoney(money);
			SavePlayerMoney();
			SetPlayerAiming(aiming);
			SavePlayerAiming();
			SetPlayerControl(control);
			SavePlayerControl();
			SetPlayerNotifications(notify);
			SavePlayerNotifications();
			StartNotificationsWork();
		}

		public void LoadHelmetDataFromSave(int selected, int[] buyProgress)
		{
			for (int i = 0; i < isHelmetBought.Length; i++)
			{
				if (i < buyProgress.Length)
				{
					SetHelmetIsBought(i, buyProgress[i]);
					SaveHelmetIsBought(i);
				}
				else
				{
					SetHelmetIsBought(i, 0);
					SaveHelmetIsBought(i);
				}
			}
			if (IsHelmetBought(selected))
			{
				SetEquipedHelmet(selected);
				SaveEquipedHelmet();
			}
			else
			{
				SetEquipedHelmet(999);
				SaveEquipedHelmet();
			}
		}

		public void LoadShieldDataFromSave(int selected, int[] buyProgress)
		{
			for (int i = 0; i < isShieldBought.Length; i++)
			{
				if (i < buyProgress.Length)
				{
					SetShieldIsBought(i, buyProgress[i]);
					SaveShieldIsBought(i);
				}
				else
				{
					SetShieldIsBought(i, 0);
					SaveShieldIsBought(i);
				}
			}
			if (GetShieldIsBought(selected))
			{
				SetEquipedShield(selected);
				SaveEqupedShield();
			}
			else
			{
				SetEquipedShield(999);
				SaveEqupedShield();
			}
		}

		public void LoadCatapultDataFromSaveFile(int selected, int[] canBuy, int[] isBought, int[] upgrade)
		{
			for (int i = 0; i < canBuyCatapult.Length; i++)
			{
				if (i < canBuy.Length)
				{
					SetCanBuyCatapult(i, canBuy[i]);
					SaveCanBuyCatapult(i);
				}
				else
				{
					SetCanBuyCatapult(i, 0);
					SaveCanBuyCatapult(i);
				}
				if (i < isBought.Length)
				{
					SetCatapultIsBought(i, isBought[i]);
					SaveCatapulIsBought(i);
				}
				else
				{
					SetCatapultIsBought(i, 0);
					SaveCatapulIsBought(i);
				}
				if (i < upgrade.Length)
				{
					SetCatapultUpgrade(i, upgrade[i]);
					SaveCatapultUpgrade(i);
				}
				else
				{
					SetCatapultUpgrade(i, 0);
					SaveCatapultUpgrade(i);
				}
			}
			if (IsCatapultBought(selected) == 1)
			{
				SetCurrentCatapult(selected);
				SaveCurrentCatapult();
			}
			else
			{
				SetCurrentCatapult(0);
				SaveCurrentCatapult();
			}
		}

		public void LoadOldMainDataFromSave(float money, int aiming, int control, int notify)
		{
			if (money < 0f)
			{
				money = 0f;
			}
			SetPlayerMoney((int)money);
			SavePlayerMoney();
			if (aiming != 0 && aiming != 1)
			{
				aiming = 0;
			}
			SetPlayerAiming(aiming);
			SavePlayerAiming();
			if (control != 0 && control != 1)
			{
				control = 0;
			}
			SetPlayerControl(control);
			SavePlayerControl();
			if (notify != 0 && notify != 1)
			{
				notify = 1;
			}
			SetPlayerNotifications(notify);
			SavePlayerNotifications();
			StartNotificationsWork();
		}

		public void LoadOldHelmetDataFromSave(int selected, int[] buyProgress)
		{
			for (int i = 0; i < isHelmetBought.Length; i++)
			{
				if (i < buyProgress.Length)
				{
					int num = buyProgress[i];
					if (num < 0)
					{
						num = 0;
					}
					else if (num > 1)
					{
						num = 1;
					}
					SetHelmetIsBought(i, num);
					SaveHelmetIsBought(i);
				}
				else
				{
					SetHelmetIsBought(i, 0);
					SaveHelmetIsBought(i);
				}
			}
			if (IsHelmetBought(selected))
			{
				SetEquipedHelmet(selected);
				SaveEquipedHelmet();
			}
			else
			{
				SetEquipedHelmet(999);
				SaveEquipedHelmet();
			}
		}

		public void LoadOldShieldDataFromSave(int selected, int[] buyProgress)
		{
			for (int i = 0; i < isShieldBought.Length; i++)
			{
				if (i < buyProgress.Length)
				{
					int num = buyProgress[i];
					if (num < 0)
					{
						num = 0;
					}
					else if (num > 1)
					{
						num = 1;
					}
					SetShieldIsBought(i, num);
					SaveShieldIsBought(i);
				}
				else
				{
					SetShieldIsBought(i, 0);
					SaveShieldIsBought(0);
				}
			}
			if (GetShieldIsBought(selected))
			{
				SetEquipedShield(selected);
				SaveEqupedShield();
			}
			else
			{
				SetEquipedShield(999);
				SaveEqupedShield();
			}
		}

		public void LoadOldCatapultDataFromSaveFile(int selected, int[] progress)
		{
			for (int num = isCatapultBought.Length - 1; num >= 0; num--)
			{
				if (num < progress.Length && progress[num] > 0)
				{
					SetCanBuyCatapult(num, 1);
					SaveCanBuyCatapult(num);
					SetCatapultIsBought(num, 1);
					SaveCatapulIsBought(num);
					SetCatapultUpgrade(num, CheckAndFixUpgradeIndex(num, progress[num] - 1));
					SaveCatapultUpgrade(num);
					for (int num2 = num - 1; num2 >= 0; num2--)
					{
						SetCanBuyCatapult(num2, 1);
						SetCatapultIsBought(num2, 1);
						UpgradeToMaximum(num2);
						SaveCanBuyCatapult(num2);
						SaveCatapulIsBought(num2);
						SaveCatapultUpgrade(num2);
					}
					if (IsCatapultAtMaximumLevel(num) && num + 1 < isCatapultBought.Length)
					{
						SetCanBuyCatapult(num + 1, 1);
						SaveCanBuyCatapult(num + 1);
					}
					break;
				}
				if (num == 0)
				{
					SetCanBuyCatapult(num, 1);
				}
				else
				{
					SetCanBuyCatapult(num, 0);
				}
				SaveCanBuyCatapult(num);
				if (num == 0)
				{
					SetCatapultIsBought(num, 1);
				}
				else
				{
					SetCatapultIsBought(num, 0);
				}
				SaveCatapulIsBought(num);
				SetCatapultUpgrade(num, 0);
				SaveCatapultUpgrade(num);
			}
			if (IsCatapultBought(selected) == 1)
			{
				SetCurrentCatapult(selected);
				SaveCurrentCatapult();
			}
			else
			{
				SetCurrentCatapult(0);
				SaveCurrentCatapult();
			}
		}
	}
}
