using System;
using System.Collections.Generic;
using Inventory;
using Inventory.TimedInventory;
using UnityEngine.Purchasing;

namespace IAP
{
	public class IAPItemData
	{
		public IAPItemType IAPItemType;

		public string ProductId;

		public ProductType Type;

		public List<InventoryPayload> Payload;

		public List<TimedInventoryPayload> TimedPayload;

		public Action CustomProcessor;
	}
}
