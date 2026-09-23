using DG.Tweening;
using Model;
using UnityEngine;
using UnityEngine.UI;
using View;

namespace Logic
{
	internal class PlayerShop : MonoBehaviour
	{
		public PlayModeSceneControl _mainControl;

		public WeaponSlotControl firstWeaponSlot;

		public WeaponSlotControl secondWeaponSlot;

		public ShieldSlotControl shieldSlot;

		public Button buyCoinsButton;

		public GameObject[] shopScreens;

		public Image[] shopImages;

		public GameObject upgradeButton;

		public ShopSlot[] catapults;

		public ShopSlot[] helmets;

		public ShopSlot[] projectiles;

		public ShopSlot[] shields;

		internal bool isActive;

		private void Start()
		{
			isActive = false;
		}

		public void PrepareShop()
		{
			ExitWEaponTab();
			ExitShieldTab();
			CheckSavings();
		}

		public void EnterShop()
		{
			isActive = true;
			int lastOpenedShopWindow = InitController.instance.lastOpenedShopWindow;
			if (lastOpenedShopWindow != -1)
			{
				for (int i = 0; i < shopScreens.Length; i++)
				{
					if (i == lastOpenedShopWindow)
					{
						switch (i)
						{
						case 2:
							EnterWeaponTab();
							break;
						case 3:
							EnterShieldTab();
							break;
						}
						shopScreens[i].SetActive(value: true);
						shopImages[i].color = new Color(0.2f, 0.2f, 0.8f, 0.6f);
					}
					else
					{
						shopScreens[i].SetActive(value: false);
					}
				}
			}
			else
			{
				shopImages[0].color = new Color(0.2f, 0.2f, 0.8f, 0.6f);
			}
		}

		public void EnterDonationPanel()
		{
			for (int i = 0; i < shopImages.Length; i++)
			{
				shopImages[i].color = new Color(1f, 1f, 1f, 1f);
			}
		}

		private void ChangeScreenAfterUpgrade()
		{
			for (int i = 0; i < shopScreens.Length; i++)
			{
				shopScreens[i].SetActive(value: false);
			}
			ExitShieldTab();
			ExitWEaponTab();
			shopScreens[0].SetActive(value: true);
			ClickedSlot(0);
		}

		public void ClickedSlot(int index)
		{
			SoundMgr.instance.ButtonPress();
			for (int i = 0; i < shopImages.Length; i++)
			{
				if (i == index)
				{
					shopImages[i].color = new Color(0.2f, 0.2f, 0.8f, 0.6f);
				}
				else
				{
					shopImages[i].color = new Color(1f, 1f, 1f, 1f);
				}
			}
		}

		public void ChangeShopWindow(int index)
		{
			InitController.instance.lastOpenedShopWindow = index;
		}

		private void CheckSavings()
		{
			CheckHelmetSavings();
			CheckCatapultSavings();
			CheckShieldSavings();
			CheckProjectileSavings();
		}

		private void CheckHelmetSavings()
		{
			for (int i = 0; i < helmets.Length; i++)
			{
				helmets[i].isBought = NewDataController.instance.IsHelmetBought(i);
				if (helmets[i].isBought)
				{
					if (NewDataController.instance.GetEquipedHelmet() == i)
					{
						helmets[i].isEquiped = true;
					}
					else
					{
						helmets[i].isEquiped = false;
					}
				}
			}
		}

		private void CheckCatapultSavings()
		{
			for (int i = 0; i < catapults.Length; i++)
			{
				if (NewDataController.instance.CanBuyCatapult(i) == 1)
				{
					catapults[i].UnlockSlot();
					if (NewDataController.instance.IsCatapultBought(i) == 1)
					{
						catapults[i].isBought = true;
						if (NewDataController.instance.GetCurrentCatapultIndex() == i)
						{
							catapults[i].isEquiped = true;
						}
						else
						{
							catapults[i].isEquiped = false;
						}
					}
					else
					{
						catapults[i].isBought = false;
					}
				}
				else
				{
					catapults[i].LockSlot();
				}
			}
		}

		private void CheckShieldSavings()
		{
			for (int i = 0; i < shields.Length; i++)
			{
				if (NewDataController.instance.GetShieldIsBought(i))
				{
					shields[i].isBought = true;
				}
				else
				{
					shields[i].isBought = false;
				}
			}
			if (NewDataController.instance.GetEquipedShield() != 999)
			{
				shields[NewDataController.instance.GetEquipedShield()].isEquiped = true;
			}
			ChangeSlotImage();
		}

		private void CheckLock()
		{
			for (int i = 0; i < catapults.Length; i++)
			{
				if (NewDataController.instance.CanBuyCatapult(i) == 1)
				{
					catapults[i].UnlockSlot();
				}
			}
		}

		private void CheckProjectileSavings()
		{
			for (int i = 0; i < projectiles.Length; i++)
			{
				if (i == 0)
				{
					projectiles[i].SetItemCount(-1f);
					continue;
				}
				int projectileCount = ProjectileControl.instance.GetProjectileCount(i);
				projectiles[i].SetItemCount(projectileCount);
			}
			for (int j = 0; j < 2; j++)
			{
				ProjectileType projectileType = ProjectileControl.instance.CheckSlotProjectile(j);
				if (j == 0)
				{
					if (projectileType == ProjectileType.Stone)
					{
						firstWeaponSlot.ClearSlot();
						continue;
					}
					firstWeaponSlot.SetSprite(ProjectileControl.instance.GetSpriteByType(projectileType));
					int equipedProjectileIndex = ProjectileControl.instance.GetEquipedProjectileIndex(0);
					projectiles[equipedProjectileIndex].isEquiped = true;
				}
				else if (projectileType == ProjectileType.Stone)
				{
					secondWeaponSlot.ClearSlot();
				}
				else
				{
					secondWeaponSlot.SetSprite(ProjectileControl.instance.GetSpriteByType(projectileType));
					int equipedProjectileIndex2 = ProjectileControl.instance.GetEquipedProjectileIndex(1);
					projectiles[equipedProjectileIndex2].isEquiped = true;
				}
			}
		}

		public bool BuyHelmet(int index)
		{
			int buyValue = helmets[index].buyValue;
			if (NewDataController.instance.IsEnoughMoney(buyValue))
			{
				NewDataController.instance.BuyHelmet(index);
				NewDataController.instance.SaveHelmetIsBought(index);
				NewDataController.instance.CheckoutMoney(buyValue);
				NewDataController.instance.SavePlayerMoney();
				AchievementManager.instance.AchievementProgress(AchieventType.HelmetUnlock, AchievementRegion.AllGame, 1);
				AchievementManager.instance.AchievementProgress(AchieventType.MoneyWaste, AchievementRegion.AllGame, buyValue);
				_mainControl.RefreshMoney();
				return true;
			}
			buyCoinsButton.transform.DOShakePosition(0.5f, 10f, 10, 45f);
			return false;
		}

		public void EquipHelmet(int index)
		{
			NewDataController.instance.EquipHelmet(index);
			NewDataController.instance.SaveEquipedHelmet();
			for (int i = 0; i < helmets.Length; i++)
			{
				if (i != index)
				{
					helmets[i].isEquiped = false;
				}
				else
				{
					helmets[i].isEquiped = true;
				}
			}
			try
			{
				_mainControl.EquipHelmet();
			}
			catch
			{
				UnityEngine.Debug.Log("(BYV) Cannot equip helmet");
			}
		}

		public bool BuyShield(int index)
		{
			int buyValue = shields[index].buyValue;
			if (NewDataController.instance.IsEnoughMoney(buyValue))
			{
				NewDataController.instance.CheckoutMoney(buyValue);
				NewDataController.instance.BuyShield(index);
				NewDataController.instance.SaveShieldIsBought(index);
				NewDataController.instance.SavePlayerMoney();
				switch (UpgradeShopData.instance.GetShieldType(index))
				{
				case ShieldTypes.None:
					UnityEngine.Debug.LogError("There is no shield type at index - " + index);
					break;
				case ShieldTypes.FullDestroy:
				case ShieldTypes.SpoonDestroy:
				case ShieldTypes.SpinDestroy:
					AchievementManager.instance.AchievementProgress(AchieventType.DestroyShieldUnlock, AchievementRegion.AllGame, 1);
					break;
				default:
					AchievementManager.instance.AchievementProgress(AchieventType.ReflectShieldUnlock, AchievementRegion.AllGame, 1);
					break;
				}
				AchievementManager.instance.AchievementProgress(AchieventType.MoneyWaste, AchievementRegion.AllGame, buyValue);
				_mainControl.RefreshMoney();
				return true;
			}
			buyCoinsButton.transform.DOShakePosition(0.5f, 10f, 10, 45f);
			return false;
		}

		public void EquipShield(int index)
		{
			NewDataController.instance.EquipShield(index);
			NewDataController.instance.SaveEqupedShield();
			for (int i = 0; i < shields.Length; i++)
			{
				if (i != index)
				{
					shields[i].isEquiped = false;
				}
				else
				{
					shields[i].isEquiped = true;
				}
			}
			ChangeSlotImage();
		}

		private void ChangeSlotImage()
		{
			shieldSlot.SetSprite(UpgradeShopData.instance.GetCurrentShieldIcon());
		}

		public void ClearShieldSlot()
		{
			if (isActive)
			{
				SoundMgr.instance.SlotClear();
			}
			if (NewDataController.instance.GetEquipedShield() != 999)
			{
				shields[NewDataController.instance.GetEquipedShield()].isEquiped = false;
				NewDataController.instance.ClearShieldSlot();
				NewDataController.instance.SaveEqupedShield();
			}
		}

		public bool BuyCatapult(int index)
		{
			int buyValue = catapults[index].buyValue;
			if (NewDataController.instance.IsEnoughMoney(buyValue))
			{
				NewDataController.instance.CheckoutMoney(buyValue);
				NewDataController.instance.BuyCatapult(index);
				NewDataController.instance.SaveCatapulIsBought(index);
				NewDataController.instance.SavePlayerMoney();
				AchievementManager.instance.AchievementProgress(AchieventType.CatapultUnlock, AchievementRegion.AllGame, 1);
				AchievementManager.instance.AchievementProgress(AchieventType.MoneyWaste, AchievementRegion.AllGame, buyValue);
				_mainControl.RefreshMoney();
				CheckLock();
				_mainControl.SetUpgradeCost();
				return true;
			}
			buyCoinsButton.transform.DOShakePosition(0.5f, 10f, 10, 45f);
			return false;
		}

		public void UpgradeCatapult()
		{
			if (NewDataController.instance.CanUpgradeCurrentCatapult())
			{
				int num = UpgradeShopData.instance.AskCurrentCatapultCost();
				if (NewDataController.instance.IsEnoughMoney(num))
				{
					NewDataController.instance.CheckoutMoney(num);
					NewDataController.instance.SavePlayerMoney();
					NewDataController.instance.UpgradeCurrentCatapult();
					NewDataController.instance.SaveCurrentCatapultUpgrade();
					AchievementManager.instance.AchievementProgress(AchieventType.MoneyWaste, AchievementRegion.AllGame, num);
					CheckLock();
					_mainControl.SpawnCatapult();
					_mainControl.RefreshMoney();
					if (base.gameObject.activeSelf)
					{
						SoundMgr.instance.UpgradePress();
					}
					ChangeScreenAfterUpgrade();
					_mainControl.SetUpgradeCost();
				}
				else
				{
					buyCoinsButton.transform.DOShakePosition(0.5f, 10f, 10, 45f);
				}
			}
			else
			{
				int currentCatapultIndex = NewDataController.instance.GetCurrentCatapultIndex();
				if (currentCatapultIndex < catapults.Length - 1)
				{
					DOTween.KillAll();
					catapults[currentCatapultIndex + 1].buyButton.transform.localScale = new Vector3(1f, 1f);
					catapults[currentCatapultIndex + 1].buyButton.transform.DOPunchScale(new Vector3(0.25f, 0.25f), 0.25f);
				}
			}
		}

		public void EquipCatapult(int index)
		{
			NewDataController.instance.EquipCatapult(index);
			NewDataController.instance.SaveCurrentCatapult();
			for (int i = 0; i < catapults.Length; i++)
			{
				if (i != index)
				{
					catapults[i].isEquiped = false;
				}
				else
				{
					catapults[i].isEquiped = true;
				}
			}
			_mainControl.SpawnCatapult();
			_mainControl.SetUpgradeCost();
		}

		public bool BuyProjectile(int index)
		{
			int buyValue = projectiles[index].buyValue;
			if (NewDataController.instance.IsEnoughMoney(buyValue))
			{
				NewDataController.instance.CheckoutMoney(buyValue);
				int num = ProjectileControl.instance.BuyProjectile(index);
				NewDataController.instance.SavePlayerMoney();
				projectiles[index].SetItemCount(num);
				AchievementManager.instance.AchievementProgress(AchieventType.MoneyWaste, AchievementRegion.AllGame, buyValue);
				_mainControl.RefreshMoney();
				return true;
			}
			buyCoinsButton.transform.DOShakePosition(0.5f, 10f, 10, 45f);
			return false;
		}

		public bool EquipProjectile(int projectileIndex, float cost)
		{
			int num = ProjectileControl.instance.EquipProjectile(projectileIndex);
			if (num > -1)
			{
				if (!projectiles[projectileIndex].isEquiped)
				{
					projectiles[projectileIndex].isEquiped = true;
				}
				switch (num)
				{
				case 0:
					firstWeaponSlot.SetSprite(ProjectileControl.instance.GetSpriteByIndex(projectileIndex));
					return true;
				case 1:
					secondWeaponSlot.SetSprite(ProjectileControl.instance.GetSpriteByIndex(projectileIndex));
					return true;
				}
			}
			return false;
		}

		public void ClearWeaponSlot(int slot)
		{
			if (isActive)
			{
				SoundMgr.instance.SlotClear();
			}
			int equipedProjectileIndex = ProjectileControl.instance.GetEquipedProjectileIndex(slot);
			if (equipedProjectileIndex > -1)
			{
				projectiles[equipedProjectileIndex].isEquiped = false;
			}
			ProjectileControl.instance.ClearSlot(slot);
		}

		public void EnterWeaponTab()
		{
			firstWeaponSlot.gameObject.SetActive(value: true);
			secondWeaponSlot.gameObject.SetActive(value: true);
		}

		public void ExitWEaponTab()
		{
			firstWeaponSlot.gameObject.SetActive(value: false);
			secondWeaponSlot.gameObject.SetActive(value: false);
		}

		public void EnterShieldTab()
		{
			shieldSlot.gameObject.SetActive(value: true);
		}

		public void ExitShieldTab()
		{
			shieldSlot.gameObject.SetActive(value: false);
		}

		public void PulseUpgrade()
		{
			DOTween.KillAll();
			upgradeButton.transform.localScale = new Vector3(1f, 1f);
			upgradeButton.transform.DOPunchScale(new Vector3(0.25f, 0.25f), 0.25f);
		}

		public void CheatCoins()
		{
			NewDataController.instance.AddMoney(100000);
			_mainControl.RefreshMoney();
		}
	}
}
