namespace IAP
{
	public static class IAPExtensions
	{
		public static bool IsRestorable(this IAPItemData data)
		{
			return data != null && data.Type == UnityEngine.Purchasing.ProductType.NonConsumable;
		}
	}
}
