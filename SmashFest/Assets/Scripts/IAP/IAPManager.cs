using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using IAP.Persisted;
using Inventory;
using Inventory.TimedInventory;
using LocalSave;
using Popups;
using Service;
using UnityEngine;
using UnityEngine.Purchasing;
using Util;

namespace IAP
{
	public class IAPManager : ServiceMonoBehaviour
	{
		private StoreController _storeController;

		private readonly List<IAPTrack> _activeTracks = new List<IAPTrack>();

		private bool _initialized;

		private bool _isRestoring;

		private Action<bool> _onRestoreComplete;

		public Action OnProductsUpdated;

		private GameObject _purchaseCanvas;

		private bool _initializing;

		protected override void Awake()
		{
			IAPManager existing = ServiceLocator.Get<IAPManager>();
			if (existing != null && existing != this)
			{
				Destroy(gameObject);
				return;
			}
			IAPManager duplicate = FindObjectsOfType<IAPManager>().FirstOrDefault(x => x != this);
			if (duplicate != null)
			{
				Destroy(gameObject);
				return;
			}
			base.Awake();
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
			Initialize();
		}

		public static void Init()
		{
			if (ServiceLocator.Get<IAPManager>() != null || UnityEngine.Object.FindObjectOfType<IAPManager>() != null)
			{
				return;
			}
			GameObject gameObject = new GameObject("IAPManager");
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
			gameObject.AddComponent<IAPManager>();
		}

		private void Initialize()
		{
			if (_initializing || _storeController != null)
			{
				return;
			}

			_initializing = true;
			_storeController = UnityIAPServices.StoreController();
			_storeController.OnPurchasePending += OnPurchasePending;
			_storeController.OnPurchaseFailed += OnPurchaseFailed;
			_storeController.OnPurchaseConfirmed += OnPurchaseConfirmed;
			_storeController.OnProductsFetched += OnProductsFetched;
			_storeController.OnProductsFetchFailed += OnProductsFetchFailed;
			_storeController.OnPurchasesFetched += OnPurchasesFetched;
			_storeController.OnPurchasesFetchFailed += OnPurchasesFetchFailed;
			EnsurePurchaseCanvas();
			ConnectAndFetchProducts();
		}

		private async void ConnectAndFetchProducts()
		{
			try
			{
				await _storeController.Connect();
				_storeController.FetchProducts(IAPLibrary.GetDefinitions());
			}
			catch (Exception exception)
			{
				Debug.LogError("[IAP] Connect failed: " + exception.Message);
			}
		}

		protected override void OnDestroy()
		{
			if (_storeController != null)
			{
				_storeController.OnPurchasePending -= OnPurchasePending;
				_storeController.OnPurchaseFailed -= OnPurchaseFailed;
				_storeController.OnPurchaseConfirmed -= OnPurchaseConfirmed;
				_storeController.OnProductsFetched -= OnProductsFetched;
				_storeController.OnProductsFetchFailed -= OnProductsFetchFailed;
				_storeController.OnPurchasesFetched -= OnPurchasesFetched;
				_storeController.OnPurchasesFetchFailed -= OnPurchasesFetchFailed;
			}
			base.OnDestroy();
		}

		private void OnProductsFetched(List<Product> products)
		{
			Debug.Log(string.Format("[IAP] Products fetched: {0}", products?.Count ?? 0));
			_initialized = true;
			_storeController.FetchPurchases();
			OnProductsUpdated?.Invoke();
		}

		private (decimal, string)? GetPrice(string productId)
		{
			if (_storeController == null || string.IsNullOrEmpty(productId))
			{
				return null;
			}

			Product product = _storeController.GetProductById(productId);
			if (product?.metadata == null)
			{
				return null;
			}

			string currency = product.metadata.isoCurrencyCode;
			if (string.IsNullOrEmpty(currency))
			{
				return null;
			}

			return (product.metadata.localizedPrice, currency);
		}

		public string GetPriceString(string productId)
		{
			(decimal price, string currency)? priceData = GetPrice(productId);
			if (!priceData.HasValue)
			{
				return "---";
			}

			return priceData.Value.currency == "USD"
				? string.Format(CultureInfo.InvariantCulture, "${0:0.00}", priceData.Value.price)
				: string.Format(CultureInfo.InvariantCulture, "{0} {1:0.00}", priceData.Value.currency, priceData.Value.price);
		}

		public string GetPriceString(IAPItemType itemType)
		{
			IAPItemData data = IAPLibrary.GetDataByIAPItemType(itemType);
			return data == null ? "---" : GetPriceString(data.ProductId);
		}

		private void OnProductsFetchFailed(ProductFetchFailed failure)
		{
			Debug.LogError("[IAP] Products fetch failed: " + failure?.FailureReason);
		}

		private void OnPurchasesFetched(Orders orders)
		{
			int confirmedCount = orders?.ConfirmedOrders?.Count ?? 0;
			int pendingCount = orders?.PendingOrders?.Count ?? 0;
			int deferredCount = orders?.DeferredOrders?.Count ?? 0;
			Debug.Log(string.Format("[IAP] Purchases fetched: {0} confirmed, {1} pending, {2} deferred", confirmedCount, pendingCount, deferredCount));

			if (!_isRestoring)
			{
				return;
			}

			RestoreEntitlementsFromOrders(orders);
			OnProductsUpdated?.Invoke();
			FinishRestore(true);
		}

		private void OnPurchasesFetchFailed(PurchasesFetchFailureDescription failure)
		{
			string message = failure == null
				? "unknown"
				: string.Format("{0} - {1}", failure.FailureReason, failure.Message);
			Debug.LogError("[IAP] Purchases fetch failed: " + message);
			if (_isRestoring)
			{
				FinishRestore(false);
			}
		}

		private void ProcessEditorPurchase(string productId, IAPSource source, Action<IAPResult> onComplete)
		{
			IAPTrack track = new IAPTrack
			{
				ProductId = productId,
				Guid = Guid.NewGuid().ToString(),
				OnComplete = onComplete,
				Source = source
			};
			_activeTracks.Add(track);
			SetPurchaseCanvasVisible(true);
			ProcessPurchaseInternal(productId, track.Guid);
		}

		public void StartPurchase(IAPItemType itemType, IAPSource source, Action<IAPResult> onComplete)
		{
			IAPItemData data = IAPLibrary.GetDataByIAPItemType(itemType);
			if (data == null)
			{
				FailPurchaseStart(onComplete, PopupType.PurchaseFailed, "[IAP] No IAP data found for " + itemType);
				return;
			}

			StartPurchase(data.ProductId, source, onComplete);
		}

		public void StartPurchase(string productId, IAPSource source, Action<IAPResult> onComplete)
		{
			if (CommonUtils.IsEditor())
			{
				ProcessEditorPurchase(productId, source, onComplete);
				return;
			}

			if (!_initialized || _storeController == null)
			{
				FailPurchaseStart(onComplete, PopupType.PurchaseFailed, "[IAP] Not initialized");
				return;
			}

			if (_activeTracks.Any(x => x.ProductId == productId))
			{
				FailPurchaseStart(onComplete, null, "[IAP] Purchase already in progress for " + productId);
				return;
			}

			Product product = _storeController.GetProductById(productId);
			if (product == null || !product.availableToPurchase)
			{
				FailPurchaseStart(onComplete, PopupType.PurchaseFailed, "[IAP] Couldn't find purchasable product with id " + productId);
				return;
			}

			IAPTrack track = new IAPTrack
			{
				ProductId = productId,
				Guid = Guid.NewGuid().ToString(),
				OnComplete = onComplete,
				Source = source
			};
			_activeTracks.Add(track);
			SetPurchaseCanvasVisible(true);
			Debug.Log("[IAP] Starting purchase for " + productId);
			_storeController.PurchaseProduct(product);
		}

		public void RestorePurchases(Action<bool> onComplete = null)
		{
			if (!_initialized || _storeController == null)
			{
				onComplete?.Invoke(false);
				return;
			}

			if (_isRestoring)
			{
				_onRestoreComplete += onComplete;
				return;
			}

			_isRestoring = true;
			_onRestoreComplete = onComplete;
			Debug.Log("[IAP] Restoring purchases..");

			if (CommonUtils.IsIOS())
			{
				_storeController.RestoreTransactions(delegate(bool success, string error)
				{
					if (!success)
					{
						Debug.LogError("[IAP] Restore failed: " + error);
						FinishRestore(false);
					}
				});
				return;
			}

			_storeController.FetchPurchases();
		}

		private void OnPurchasePending(PendingOrder pending)
		{
			string productId = GetOrderProductId(pending);
			if (string.IsNullOrEmpty(productId))
			{
				Debug.LogWarning("[IAP] OnPurchasePending with no product id.");
				SetPurchaseCanvasVisible(false);
				_storeController?.ConfirmPurchase(pending);
				return;
			}

			Debug.Log("[IAP] Purchase pending: " + productId);
			ProcessPurchaseInternal(productId, pending.Info?.TransactionID);
			_storeController?.ConfirmPurchase(pending);
		}

		private void OnPurchaseConfirmed(Order order)
		{
			string productId = GetOrderProductId(order);
			Debug.Log("[IAP] Purchase confirmed: " + (string.IsNullOrEmpty(productId) ? "unknown" : productId));
		}

		private void OnPurchaseFailed(FailedOrder failed)
		{
			string productId = GetOrderProductId(failed);
			Debug.LogWarning(string.Format("[IAP] Purchase failed: {0} ({1})", string.IsNullOrEmpty(productId) ? "unknown" : productId, failed.FailureReason));

			IAPTrack track = FindTrack(productId);
			if (track == null)
			{
				SetPurchaseCanvasVisible(false);
				return;
			}

			_activeTracks.Remove(track);
			SetPurchaseCanvasVisible(false);
			PopupType popupType = failed.FailureReason == PurchaseFailureReason.UserCancelled
				? PopupType.PurchaseCancelled
				: PopupType.PurchaseFailed;
			track.OnComplete?.Invoke(IAPResult.Fail);
			ServiceLocator.Get<PopupController>()?.Open(popupType);
		}

		private void ProcessPurchaseInternal(string productId, string transactionId)
		{
			IAPTrack track = FindTrack(productId);
			IAPItemData data = IAPLibrary.GetDataById(productId);
			if (data == null)
			{
				Debug.LogError("[IAP] IAP data not found for productId: " + productId);
				if (track != null)
				{
					_activeTracks.Remove(track);
					SetPurchaseCanvasVisible(false);
					track.OnComplete?.Invoke(IAPResult.Fail);
					ServiceLocator.Get<PopupController>()?.Open(PopupType.PurchaseFailed);
				}
				return;
			}

			if (data.Payload != null)
			{
				foreach (InventoryPayload payload in data.Payload)
				{
					InventoryHelper.AddAmount(payload, false, InventoryEarnSource.Purchase);
				}
			}

			if (data.TimedPayload != null)
			{
				foreach (TimedInventoryPayload timedPayload in data.TimedPayload)
				{
					TimedInventoryHelper.AddTime(timedPayload, InventoryEarnSource.Purchase);
				}
			}

			data.CustomProcessor?.Invoke();

			if (track == null)
			{
				SetPurchaseCanvasVisible(false);
				Debug.Log("[IAP] Processed purchase without active track: " + productId);
				return;
			}

			(decimal price, string currency)? priceData = GetPrice(productId);
			PersistPurchase(productId, priceData.HasValue ? (float)priceData.Value.price : 0f, priceData.HasValue ? priceData.Value.currency : string.Empty);
			_activeTracks.Remove(track);
			SetPurchaseCanvasVisible(false);
			track.OnComplete?.Invoke(IAPResult.Success);
			OnProductsUpdated?.Invoke();
			ServiceLocator.Get<PopupController>()?.Open(PopupType.PurchaseSuccess);
		}

		private void PersistPurchase(string productId, float price, string currency)
		{
			if (SaveService.Data == null)
			{
				return;
			}

			if (SaveService.Data.Purchases == null)
			{
				SaveService.Data.Purchases = PersistedPurchases.GetDefault();
			}
			if (SaveService.Data.Purchases.purchases == null)
			{
				SaveService.Data.Purchases.purchases = new List<PersistedPurchaseItem>();
			}

			SaveService.Data.Purchases.purchases.Add(new PersistedPurchaseItem
			{
				productId = productId,
				price = price,
				currency = currency
			});
		}

		private void RestoreEntitlementsFromOrders(Orders orders)
		{
			if (orders?.ConfirmedOrders == null)
			{
				return;
			}

			foreach (ConfirmedOrder order in orders.ConfirmedOrders)
			{
				if (order?.Info?.PurchasedProductInfo == null)
				{
					continue;
				}

				foreach (IPurchasedProductInfo purchasedProduct in order.Info.PurchasedProductInfo)
				{
					if (purchasedProduct == null || string.IsNullOrEmpty(purchasedProduct.productId))
					{
						continue;
					}

					IAPItemData data = IAPLibrary.GetDataById(purchasedProduct.productId);
					if (data == null || !data.IsRestorable())
					{
						continue;
					}

					Debug.Log("[IAP] Restoring entitlement for " + purchasedProduct.productId);

					if (data.Payload != null)
					{
						foreach (InventoryPayload payload in data.Payload)
						{
							InventoryHelper.AddAmount(payload, false, InventoryEarnSource.Purchase);
						}
					}

					if (data.TimedPayload != null)
					{
						foreach (TimedInventoryPayload timedPayload in data.TimedPayload)
						{
							TimedInventoryHelper.AddTime(timedPayload, InventoryEarnSource.Purchase);
						}
					}

					data.CustomProcessor?.Invoke();
				}
			}
		}

		private void EnsurePurchaseCanvas()
		{
			if (_purchaseCanvas != null)
			{
				SetPurchaseCanvasVisible(false);
				return;
			}
			GameObject existingCanvas = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(delegate(GameObject x)
			{
				return x != null && (x.name == "PurchaseCanvas" || x.name == "PurchaseCanvas(Clone)");
			});
			if (existingCanvas != null)
			{
				_purchaseCanvas = existingCanvas;
				UnityEngine.Object.DontDestroyOnLoad(_purchaseCanvas);
				SetPurchaseCanvasVisible(false);
				return;
			}

			GameObject purchaseCanvasPrefab = Resources.Load<GameObject>("PurchaseCanvas");
			if (purchaseCanvasPrefab == null)
			{
				return;
			}

			_purchaseCanvas = UnityEngine.Object.Instantiate(purchaseCanvasPrefab);
			UnityEngine.Object.DontDestroyOnLoad(_purchaseCanvas);
			SetPurchaseCanvasVisible(false);
		}

		private void FailPurchaseStart(Action<IAPResult> onComplete, PopupType? popupType, string message)
		{
			Debug.LogWarning(message);
			SetPurchaseCanvasVisible(false);
			onComplete?.Invoke(IAPResult.Fail);
			if (popupType.HasValue)
			{
				ServiceLocator.Get<PopupController>()?.Open(popupType.Value);
			}
		}

		private void SetPurchaseCanvasVisible(bool visible)
		{
			if (_purchaseCanvas == null)
			{
				return;
			}
			if (_purchaseCanvas.activeSelf == visible)
			{
				return;
			}
			_purchaseCanvas.SetActive(visible);
		}

		private void FinishRestore(bool success)
		{
			_isRestoring = false;
			Action<bool> callback = _onRestoreComplete;
			_onRestoreComplete = null;
			callback?.Invoke(success);
		}

		private IAPTrack FindTrack(string productId)
		{
			if (!string.IsNullOrEmpty(productId))
			{
				return _activeTracks.FirstOrDefault(x => x.ProductId == productId);
			}

			return _activeTracks.Count == 1 ? _activeTracks[0] : null;
		}

		private static string GetOrderProductId(Order order)
		{
			if (order?.Info?.PurchasedProductInfo != null)
			{
				IPurchasedProductInfo purchasedProduct = order.Info.PurchasedProductInfo.FirstOrDefault(x => x != null && !string.IsNullOrEmpty(x.productId));
				if (purchasedProduct != null)
				{
					return purchasedProduct.productId;
				}
			}

			CartItem firstCartItem = order?.CartOrdered?.Items()?.FirstOrDefault();
			if (firstCartItem?.Product?.catalogListings != null && !string.IsNullOrEmpty(firstCartItem.CatalogListingId) && firstCartItem.Product.catalogListings.TryGetValue(firstCartItem.CatalogListingId, out CatalogListing listing))
			{
				return listing.definition?.storeSpecificId ?? firstCartItem.Product.uSku;
			}

			return firstCartItem?.Product?.uSku;
		}
	}
}
