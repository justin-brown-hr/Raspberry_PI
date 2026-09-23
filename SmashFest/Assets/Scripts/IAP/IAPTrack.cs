using System;

namespace IAP
{
	public class IAPTrack
	{
		public string ProductId;

		public string Guid;

		public Action<IAPResult> OnComplete;

		public IAPSource Source;
	}
}
