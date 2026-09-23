using System;
using IAP;
using TMPro;
using UnityEngine;
using Service;

namespace Shop
{
	public abstract class ShopItemBase : MonoBehaviour
	{
		[SerializeField]
		protected IAPItemType iapType;

		[SerializeField]
		protected FlowButton purchaseButton;

		[SerializeField]
		protected TextMeshProUGUI priceText;

		protected IAPItemData ItemData;

		protected IAPSource Source;

		protected Action OnPurchaseSuccess;

		protected Action OnPurchaseFail;

		private int _lastPreparedFrame = -1;

		private void OnEnable()
		{
			IAPManager iapManager = ServiceLocator.Get<IAPManager>();
			if (iapManager != null)
			{
				iapManager.OnProductsUpdated -= Prepare;
				iapManager.OnProductsUpdated += Prepare;
			}
			if (purchaseButton != null)
			{
				purchaseButton.OnClick.RemoveListener(Purchase);
				purchaseButton.OnClick.AddListener(Purchase);
			}
			if (Time.frameCount != _lastPreparedFrame)
			{
				Prepare();
			}
		}

		public virtual void Init(IAPSource source, Action onPurchaseSuccess, Action onPurchaseFail)
		{
			ItemData = IAPLibrary.GetDataByIAPItemType(iapType);
			Source = source;
			OnPurchaseSuccess = onPurchaseSuccess;
			OnPurchaseFail = onPurchaseFail;
			Prepare();
		}

		protected virtual void Prepare()
		{
			_lastPreparedFrame = Time.frameCount;
			if (ItemData == null)
			{
				return;
			}
			SetPriceString(ServiceLocator.Get<IAPManager>()?.GetPriceString(ItemData.ProductId));
		}

		public virtual void SetPriceString(string priceString)
		{
			if (priceText != null)
			{
				priceText.text = string.IsNullOrEmpty(priceString) ? "---" : priceString;
			}
		}

		protected virtual void Purchase()
		{
			if (ItemData == null)
			{
				return;
			}
			ServiceLocator.Get<IAPManager>()?.StartPurchase(ItemData.ProductId, Source, delegate(IAPResult result)
			{
				if (result == IAPResult.Success)
				{
					OnPurchaseSuccess?.Invoke();
				}
				else if (result == IAPResult.Fail)
				{
					OnPurchaseFail?.Invoke();
				}
			});
		}

		private void OnDisable()
		{
			IAPManager iapManager = ServiceLocator.Get<IAPManager>();
			if (iapManager != null)
			{
				iapManager.OnProductsUpdated -= Prepare;
			}
			if (purchaseButton != null)
			{
				purchaseButton.OnClick.RemoveListener(Purchase);
			}
		}
	}
}
