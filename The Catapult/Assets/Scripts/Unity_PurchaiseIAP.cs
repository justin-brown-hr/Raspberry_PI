using Assets.All_Scripts;
using Logic;
using System;
using UnityEngine;
//PJusing UnityEngine.Purchasing;
using UnityEngine.SceneManagement;
/* PJ
public class Unity_PurchaiseIAP : MonoBehaviour, IStoreListener
{
	private enum buttons
	{
		gold50,
		gold100,
		no_ads
	}

	private static IStoreController m_StoreController;

	private static IExtensionProvider m_StoreExtensionProvider;

	public bool Status_Admob_Purchase = true;

	private static string PRODUCT_1000_coins;

	private static string PRODUCT_5000_coins;

	private static string PRODUCT_15000_coins;

	private static string PRODUCT_100000_coins;

	private static string PRODUCT_no_ads;

	public static Unity_PurchaiseIAP Instance
	{
		get;
		set;
	}

	private void Awake()
	{
		if (!Instance)
		{
			Instance = this;
			UnityEngine.Object.DontDestroyOnLoad(this);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		Inspection_NoADSButton();
		if (m_StoreController == null)
		{
			InitializePurchasing();
		}
	}

	private void InitializePurchasing()
	{
		if (!IsInitialized())
		{
			PRODUCT_1000_coins = "con1000coins";
			PRODUCT_5000_coins = "con5000coins";
			PRODUCT_15000_coins = "con15000coins";
			PRODUCT_100000_coins = "con100000coins";
			PRODUCT_no_ads = "no_ads";
			ConfigurationBuilder configurationBuilder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
			configurationBuilder.AddProduct(PRODUCT_1000_coins, ProductType.Consumable);
			configurationBuilder.AddProduct(PRODUCT_5000_coins, ProductType.Consumable);
			configurationBuilder.AddProduct(PRODUCT_15000_coins, ProductType.Consumable);
			configurationBuilder.AddProduct(PRODUCT_100000_coins, ProductType.Consumable);
			configurationBuilder.AddProduct(PRODUCT_no_ads, ProductType.NonConsumable);
			UnityPurchasing.Initialize(this, configurationBuilder);
		}
	}

	private bool IsInitialized()
	{
		return m_StoreController != null && m_StoreExtensionProvider != null;
	}

	public void Buy_1000coins()
	{
		Buy_ProductID(PRODUCT_1000_coins);
	}

	public void Buy_5000coins()
	{
		Buy_ProductID(PRODUCT_5000_coins);
	}

	public void Buy_15000coins()
	{
		Buy_ProductID(PRODUCT_15000_coins);
	}

	public void Buy_100000coins()
	{
		Buy_ProductID(PRODUCT_100000_coins);
	}

	public void Buy_no_ads()
	{
		Buy_ProductID(PRODUCT_no_ads);
	}

	private void Buy_ProductID(string productId)
	{
		if (IsInitialized())
		{
			Product product = m_StoreController.products.WithID(productId);
			if (product != null && product.availableToPurchase)
			{
				UnityEngine.Debug.Log(string.Format("< --- >urchaising product asycronums ", product.definition.id));
				m_StoreController.InitiatePurchase(product);
			}
			else
			{
				UnityEngine.Debug.Log("Purchaising failed  id = " + productId);
			}
		}
		else
		{
			UnityEngine.Debug.Log("<---- BuyProduct is Failed NON Initialized ---->");
		}
	}

	public void RestorePurchases()
	{
		if (!IsInitialized())
		{
			UnityEngine.Debug.Log(" <-------- RestorePurchases FAIL. Not initialized.  ------> ");
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
		{
			UnityEngine.Debug.Log("RestorePurchases started ...");
			IAppleExtensions extension = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
			extension.RestoreTransactions(delegate(bool result)
			{
				UnityEngine.Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
			});
		}
		else
		{
			UnityEngine.Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
		}
	}

	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		UnityEngine.Debug.Log("< ---- OnInitialized   PASS ----->");
		m_StoreController = controller;
		m_StoreExtensionProvider = extensions;
	}

	public void OnInitializeFailed(InitializationFailureReason error)
	{
		UnityEngine.Debug.Log("<------------ Ошибка иницыализации  Unity_PurchaiseIAP ------------->" + error.ToString());
		throw new NotImplementedException();
	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
	{
		if (e.purchasedProduct.definition.id == PRODUCT_1000_coins)
		{
			UnityEngine.Debug.Log("<------------ Мы приобрели 1000 золотых монет ------------->");
			Buy_Coins(1000);
		}
		else if (e.purchasedProduct.definition.id == PRODUCT_5000_coins)
		{
			UnityEngine.Debug.Log("<------------ Мы приобрели 5000 золотых монет ------------->");
			Buy_Coins(5000);
		}
		else if (e.purchasedProduct.definition.id == PRODUCT_15000_coins)
		{
			UnityEngine.Debug.Log("<------------ Мы приобрели 15000 золотых монет ------------->");
			Buy_Coins(15000);
		}
		else if (e.purchasedProduct.definition.id == PRODUCT_100000_coins)
		{
			UnityEngine.Debug.Log("<------------ Мы приобрели 100000 золотых монет ------------->");
			Buy_Coins(100000);
		}
		else if (e.purchasedProduct.definition.id == PRODUCT_no_ads)
		{
			Bouth_ADS();
		}
		else
		{
			UnityEngine.Debug.Log("<------------ Мы приобрели Что ТО ------------->" + e.ToString());
		}
		return PurchaseProcessingResult.Complete;
	}

	private void Buy_Coins(int count_coins)
	{
		NewDataController.instance.AddMoney(count_coins);
		NewDataController.instance.SavePlayerMoney();
		if (SceneManager.GetActiveScene().name == "PlayModeSelect")
		{
			Unity_PurchaseButtonController.Instance.Update_TextMoneyCount();
		}
		if (SceneManager.GetActiveScene().name == "GameScene")
		{
			GameMenuControl.instance.RefreshCoins();
		}
		My_GoogleAnalytics.Instance.Buy_COINS(count_coins);
		Bouth_ADS();
	}

	public void Bouth_ADS()
	{
		UnityEngine.Debug.Log("<------------ Мы приобрели БЛОКИРОВКА рекламы ------------->");
		if (SceneManager.GetActiveScene().name == "MainScene")
		{
			Unity_PurchaseButtonController.Instance.Set_VisibleButtons(3, stat: false);
			return;
		}
		PlayerPrefs.SetInt(Status_Scene.Inst.status_NoADS, 1);
		Start_Admob.Instance.DisableAdmob();
		Inspection_NoADSButton();
	}

	public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
	{
		UnityEngine.Debug.Log("<------------(OnPurchaseFailed) Product ------------->" + i.ToString() + " --------- PurchaseFailureReason ----->" + p.ToString());
		throw new NotImplementedException();
	}

	public void Inspection_NoADSButton()
	{
		if (!PlayerPrefs.HasKey(Status_Scene.Inst.status_NoADS))
		{
			PlayerPrefs.SetInt(Status_Scene.Inst.status_NoADS, 0);
			return;
		}
		if (PlayerPrefs.GetInt(Status_Scene.Inst.status_NoADS) == 0)
		{
			Status_Admob_Purchase = true;
			return;
		}
		Status_Admob_Purchase = false;
		Start_Admob.Instance.DisableAdmob();
	}
}
*/