using DG.Tweening;
using Logic;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
	public class IngameShieldControl : MonoBehaviour
	{
		public Image rechargeImage;

		public Image spriteImage;

		private float _rechargeTime;

		private float _activeTime;

		private bool isRecharging;

		private bool isActive;

		private void Awake()
		{
			isRecharging = false;
			isActive = false;
		}

		public void InitSlot(Sprite sprite, float activeTime, float rechargeTime)
		{
			spriteImage.sprite = sprite;
			_activeTime = activeTime;
			_rechargeTime = rechargeTime;
		}

		public bool ShieldSlotCkick()
		{
			return Activate();
		}

		public bool Activate()
		{
			if (!isActive && !isRecharging && GlobalLogic.instance.playerCatapult._currentShield != null && !GlobalLogic.instance.playerCatapult.beingDestroyed)
			{
				Invoke("Recharge", _activeTime);
				isActive = true;
				GetComponent<Button>().interactable = false;
				rechargeImage.fillAmount = 1f;
				rechargeImage.DOFillAmount(0f, _activeTime).SetEase(Ease.Linear);
				return true;
			}
			return false;
		}

		public void Recharge()
		{
			isActive = false;
			isRecharging = true;
			rechargeImage.fillAmount = 0f;
			IngameShop.instance.DisableShield();
			rechargeImage.DOFillAmount(1f, _rechargeTime).SetEase(Ease.Linear);
			Invoke("EndRecharge", _rechargeTime);
		}

		public void EndRecharge()
		{
			rechargeImage.fillAmount = 1f;
			GetComponent<Button>().interactable = true;
			isRecharging = false;
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
