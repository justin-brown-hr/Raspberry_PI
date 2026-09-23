using DG.Tweening;
using Logic;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
	internal class IngameShopControls : MonoBehaviour
	{
		public Image selectImage;

		public Image projectileImage;

		public Text countText;

		public Text costText;

		public int slotIndex;

		private ProjectileType projectileTypeInSlot;

		private float rechargeTimer;

		private float rechargeTime;

		private float rechargeTick;

		private bool isSelected;

		private int buyValue;

		public void Initialize(ProjectileType type, float recharge)
		{
			if (slotIndex == -1)
			{
				if (type != 0)
				{
					UnityEngine.Debug.LogError("Sometimes goes wrong at projectile UI");
					return;
				}
				projectileTypeInSlot = type;
				projectileImage.sprite = ProjectileControl.instance.GetSpriteByType(projectileTypeInSlot);
				rechargeTime = recharge;
				countText.text = '∞'.ToString();
				countText.fontSize = 35;
				return;
			}
			if (type == ProjectileType.Stone)
			{
				base.gameObject.SetActive(value: false);
				return;
			}
			projectileTypeInSlot = type;
			projectileImage.sprite = ProjectileControl.instance.GetSpriteByType(projectileTypeInSlot);
			rechargeTime = recharge;
			int costByType = ProjectileControl.instance.GetCostByType(projectileTypeInSlot);
			int projectileCountByType = ProjectileControl.instance.GetProjectileCountByType(projectileTypeInSlot);
			if (projectileCountByType == 0)
			{
				countText.text = string.Empty;
			}
			else
			{
				countText.text = projectileCountByType.ToString();
			}
			buyValue = costByType;
			SetCost(buyValue);
			rechargeTimer = 0f;
		}

		public void Activate()
		{
			IngameShopControls[] controls = IngameShop.instance._controls;
			for (int i = 0; i < controls.Length; i++)
			{
				if (controls[i] != this)
				{
					controls[i].isSelected = false;
					controls[i].selectImage.color = new Color(1f, 0f, 0f, 0f);
				}
			}
			isSelected = true;
			selectImage.color = new Color(1f, 0f, 0f, 1f);
			selectImage.fillAmount = 1f;
		}

		public void Clicked()
		{
			if (!isSelected)
			{
				Activate();
				SoundMgr.instance.ButtonPress();
				GlobalLogic.instance.ChangeProjectile(slotIndex);
			}
			else if (slotIndex != -1 && GlobalLogic.instance.BuyProjectile(buyValue))
			{
				AchievementManager.instance.AchievementProgress(AchieventType.MoneyWaste, AchievementRegion.OneRound, buyValue);
				SoundMgr.instance.PurchaseSound();
				int num = ProjectileControl.instance.BuyProjectileIngame(slotIndex);
				if (num > 0)
				{
					RefreshCount(num);
				}
			}
		}

		public void SetCost(float value)
		{
			costText.text = value.ToString();
		}

		public void StartRecharge()
		{
			selectImage.fillAmount = 0f;
			selectImage.DOFillAmount(1f, rechargeTime).SetEase(Ease.Linear);
		}

		public void RefreshCount(float count)
		{
			if (count > -1f)
			{
				if (count == 0f)
				{
					countText.text = "x10";
				}
				else
				{
					countText.text = count.ToString();
				}
			}
			else
			{
				countText.text = '∞'.ToString();
			}
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
	}
}
