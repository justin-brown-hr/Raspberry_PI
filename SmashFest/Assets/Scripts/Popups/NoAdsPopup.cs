using System.Collections.Generic;
using IAP;
using IAP.Persisted;
using LocalSave;
using Service;
using TMPro;
using UnityEngine;

namespace Popups
{
	public class NoAdsPopup : BasePopup
	{
		[SerializeField]
		private FlowButton closeButton;

		[SerializeField]
		private FlowButton purchaseButton;

		[SerializeField]
		private TextMeshProUGUI priceText;

		private int _lastPreparedFrame = -1;

		private void OnEnable()
		{
			IAPManager iapManager = ServiceLocator.Get<IAPManager>();
			if (iapManager != null)
			{
				iapManager.OnProductsUpdated -= OnProductsUpdated;
				iapManager.OnProductsUpdated += OnProductsUpdated;
			}
			if (closeButton != null)
			{
				closeButton.OnClick.RemoveListener(OnCloseClicked);
				closeButton.OnClick.AddListener(OnCloseClicked);
			}
			if (purchaseButton != null)
			{
				purchaseButton.OnClick.RemoveListener(OnPurchaseClicked);
				purchaseButton.OnClick.AddListener(OnPurchaseClicked);
			}
			if (Time.frameCount != _lastPreparedFrame)
			{
				Prepare(null);
			}
		}

		public override void Prepare(Dictionary<string, object> openParameters)
		{
			_lastPreparedFrame = Time.frameCount;
			if (priceText != null)
			{
				priceText.text = ServiceLocator.Get<IAPManager>()?.GetPriceString(IAPItemType.NoAds) ?? "---";
			}
			if (purchaseButton != null)
			{
				bool alreadyOwned = (SaveService.Data?.HasNoAds ?? false) || PersistedPurchasesHelper.GetIAPItemBought(IAPItemType.NoAds);
				purchaseButton.Interactable = !alreadyOwned;
			}
		}

		private void OnPurchaseClicked()
		{
			ServiceLocator.Get<IAPManager>()?.StartPurchase(IAPItemType.NoAds, IAPSource.NoAdsPopup, delegate(IAPResult result)
			{
				if (result == IAPResult.Success)
				{
					Close();
				}
			});
		}

		private void OnCloseClicked()
		{
			Close();
		}

		private void OnProductsUpdated()
		{
			Prepare(null);
		}

		private void OnDisable()
		{
			IAPManager iapManager = ServiceLocator.Get<IAPManager>();
			if (iapManager != null)
			{
				iapManager.OnProductsUpdated -= OnProductsUpdated;
			}
			if (closeButton != null)
			{
				closeButton.OnClick.RemoveListener(OnCloseClicked);
			}
			if (purchaseButton != null)
			{
				purchaseButton.OnClick.RemoveListener(OnPurchaseClicked);
			}
		}
	}
}
