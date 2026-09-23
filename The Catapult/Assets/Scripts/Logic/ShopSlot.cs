using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
	internal class ShopSlot : MonoBehaviour
	{
		public PlayerShop playerShop;

		public ShopSlotType _slotType;

		public Button buyButton;

		public Button equipButton;

		public Button lockButton;

		public Text buyValueText;

		public Text itemCountText;

		public int shopIndex;

		public int buyValue;

		private bool _isBought;

		private bool _isEquiped;

		private bool isLocked;

		private float itemCount;

		public bool isBought
		{
			get
			{
				return _isBought;
			}
			set
			{
				_isBought = value;
				if (_slotType != ShopSlotType.Weapon)
				{
					buyButton.gameObject.SetActive(!value);
					equipButton.gameObject.SetActive(value);
					buyValueText.gameObject.SetActive(!value);
				}
			}
		}

		public bool isEquiped
		{
			get
			{
				return _isEquiped;
			}
			set
			{
				_isEquiped = value;
				if (!isBought)
				{
					return;
				}
				if (_slotType == ShopSlotType.Weapon)
				{
					if (shopIndex == 0)
					{
						equipButton.gameObject.SetActive(value: false);
					}
					else if (!_isEquiped)
					{
						equipButton.gameObject.SetActive(value: true);
					}
					else
					{
						equipButton.gameObject.SetActive(value: false);
					}
				}
				else if (!_isEquiped)
				{
					equipButton.gameObject.SetActive(value: true);
				}
				else
				{
					equipButton.gameObject.SetActive(value: false);
				}
			}
		}

		private void Start()
		{
			if (_slotType != 0)
			{
				isLocked = false;
			}
			buyValueText.text = buyValue.ToString();
		}

		public void BuyClick()
		{
			if (isLocked)
			{
				return;
			}
			switch (_slotType)
			{
			case ShopSlotType.Helmet:
				if (playerShop.BuyHelmet(shopIndex))
				{
					isBought = true;
					SoundMgr.instance.PurchaseSound();
					EquipItem();
				}
				break;
			case ShopSlotType.Catapult:
				if (playerShop.BuyCatapult(shopIndex))
				{
					isBought = true;
					SoundMgr.instance.PurchaseSound();
					EquipItem();
				}
				break;
			case ShopSlotType.Weapon:
				if (playerShop.BuyProjectile(shopIndex))
				{
					SoundMgr.instance.PurchaseSound();
					EquipItem();
					My_GoogleAnalytics.Instance.Down_ButtonBUYWeapons(shopIndex);
				}
				break;
			case ShopSlotType.Shield:
				if (playerShop.BuyShield(shopIndex))
				{
					isBought = true;
					SoundMgr.instance.PurchaseSound();
					EquipItem();
				}
				break;
			}
		}

		public void BlockEquip()
		{
			equipButton.gameObject.SetActive(value: false);
		}

		public void LockClick()
		{
			if (_slotType == ShopSlotType.Catapult)
			{
				playerShop.PulseUpgrade();
			}
		}

		public void EquipItem()
		{
			if (isLocked)
			{
				return;
			}
			switch (_slotType)
			{
			case ShopSlotType.Helmet:
				playerShop.EquipHelmet(shopIndex);
				if (playerShop.gameObject.activeSelf)
				{
					SoundMgr.instance.HelmetEquipPress();
				}
				break;
			case ShopSlotType.Weapon:
				if (itemCount > 0f && playerShop.EquipProjectile(shopIndex, buyValue) && playerShop.gameObject.activeSelf)
				{
					SoundMgr.instance.ProjectileEquipPress();
				}
				break;
			case ShopSlotType.Catapult:
				playerShop.EquipCatapult(shopIndex);
				if (playerShop.gameObject.activeSelf)
				{
					SoundMgr.instance.CatapultEquipPress();
				}
				break;
			case ShopSlotType.Shield:
				playerShop.EquipShield(shopIndex);
				if (playerShop.gameObject.activeSelf)
				{
					SoundMgr.instance.ShieldEquipPress();
				}
				break;
			}
		}

		public void LockSlot()
		{
			isLocked = true;
			lockButton.gameObject.SetActive(value: true);
		}

		public void UnlockSlot()
		{
			isLocked = false;
			lockButton.gameObject.SetActive(value: false);
		}

		public void SetItemCount(float value)
		{
			itemCount = value;
			ShopSlotType slotType = _slotType;
			if (slotType == ShopSlotType.Weapon)
			{
				if (value == -1f)
				{
					itemCountText.text = string.Empty;
					equipButton.gameObject.SetActive(value: false);
				}
				else if (value == 0f)
				{
					itemCountText.text = "x10";
				}
				else
				{
					itemCountText.text = value.ToString();
				}
			}
		}
	}
}
