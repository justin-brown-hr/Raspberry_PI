using System;
using System.Collections.Generic;
using ABTesting;
using Gameplay;
using IAP;
using LocalSave;
using UnityEngine;

namespace Shop
{
	public class ShopSection : MonoBehaviour
	{
		[SerializeField]
		private ShopSectionType sectionType;

		[SerializeField]
		private List<ShopItemBase> items;

		[SerializeField]
		private GameObject header;

		public void Init(IAPSource source, Action onPurchaseSuccess)
		{
			if (items == null)
			{
				return;
			}
			foreach (ShopItemBase item in items)
			{
				if (item != null)
				{
					item.Init(source, onPurchaseSuccess, null);
				}
			}
		}

		public bool ShouldShowSection()
		{
			if (sectionType != ShopSectionType.Bundle)
			{
				return true;
			}
			if (!ControlledKeyHelper.GetBool(ControlledKeys.BundlesEnabled))
			{
				return false;
			}
			int unlockLevel = PrelevelBoosterHelper.GetUnlockLevelForType(PrelevelBoosterType.Rocket);
			return SaveService.Data == null || SaveService.Data.Level >= unlockLevel;
		}

		public void SetHeaderActive(bool active)
		{
			if (header != null)
			{
				header.SetActive(active);
			}
		}
	}
}
