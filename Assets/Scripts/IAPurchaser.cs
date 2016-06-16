using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

///////////////////////////////////////////////////
/// IAPurchaser.cs
/// Chris Dawson 2016
/// 
/// Allows the actual IAP of products
/// Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
/// 
///////////////////////////////////////////////////

//TODO
//We need google IDs

public class IAPurchaser : MonoBehaviour, IStoreListener
{
    //Reference to the Purchasing system.
    private static IStoreController m_StoreController;

    //Reference to store-specific Purchasing subsystems.
    private static IExtensionProvider m_StoreExtensionProvider;

    //Callback for the product desired
	public Action<bool, eIAPItems> m_Callback { get; set; }

    //Are we currently busy purchasing?
	private bool m_CurrentlyPurchasing = false;

    //Type of IAP
    public enum eIAPItems
    {
        IAP_1420Coins,
        IAP_3530Coins,
        IAP_10000Coins,
        IAP_2XCoins,
        IAP_2XCoinsRemoveStatics,
        IAP_2XCoinsRemoveAllAds,
        IAP_10XCoins,
        IAP_10XCoinsRemoveAllAds,
        IAP_InfiniteCoinsRemoveAllAds,
        IAP_LEN,
    };

    //Struct to hold the details for each IAP
    private struct IAPInfo
    {
        public eIAPItems type;
        public string identifier;
        public string appleID;
        public string androidID;
        public ProductType productType;
    };

    //Product identifiers
    IAPInfo[] m_SellableItems;

    //Currently purchasing ID
    IAPInfo m_ItemBeingPurchased;

    void Start()
    {
        // If we haven't set up the Unity Purchasing reference
        if (m_StoreController == null)
        {
            //Setup IAPs
            m_SellableItems = new IAPInfo[(int)eIAPItems.IAP_LEN];

            //1420 Coins
            m_SellableItems[0].type = eIAPItems.IAP_1420Coins;
            m_SellableItems[0].identifier = "IAP_1420Coins";
            m_SellableItems[0].appleID = "WordChain.Coins1420";
            m_SellableItems[0].androidID = "com.unity3d.test.services.purchasing.consumable";
            m_SellableItems[0].productType = ProductType.Consumable;

            //3530 Coins
            m_SellableItems[1].type = eIAPItems.IAP_3530Coins;
            m_SellableItems[1].identifier = "IAP_3530Coins";
            m_SellableItems[1].appleID = "WordChain.Coins3530";
            m_SellableItems[1].androidID = "com.unity3d.test.services.purchasing.consumable";
            m_SellableItems[1].productType = ProductType.Consumable;

            //10000 Coins
            m_SellableItems[2].type = eIAPItems.IAP_10000Coins;
            m_SellableItems[2].identifier = "IAP_10000Coins";
            m_SellableItems[2].appleID = "WordChain.Coins10000";
            m_SellableItems[2].androidID = "com.unity3d.test.services.purchasing.consumable";
            m_SellableItems[2].productType = ProductType.Consumable;

            //2X Coins
            m_SellableItems[3].type = eIAPItems.IAP_2XCoins;
            m_SellableItems[3].identifier = "IAP_2XCoins";
            m_SellableItems[3].appleID = "WordChain.DoubleCoins";
            m_SellableItems[3].androidID = "com.unity3d.test.services.purchasing.nonconsumable";
            m_SellableItems[3].productType = ProductType.NonConsumable;

            //2X Coins - Remove Statics
            m_SellableItems[4].type = eIAPItems.IAP_2XCoinsRemoveStatics;
            m_SellableItems[4].identifier = "IAP_2XCoinsRemoveStatics";
            m_SellableItems[4].appleID = "WordChain.DoubleCoinsRemoveStatics";
            m_SellableItems[4].androidID = "com.unity3d.test.services.purchasing.nonconsumable";
            m_SellableItems[4].productType = ProductType.NonConsumable;

            //2X Coins - Remove Ads
            m_SellableItems[5].type = eIAPItems.IAP_2XCoinsRemoveAllAds;
            m_SellableItems[5].identifier = "IAP_2XCoinsRemoveAllAds";
            m_SellableItems[5].appleID = "WordChain.DoubleCoinsRemoveAllAds";
            m_SellableItems[5].androidID = "com.unity3d.test.services.purchasing.nonconsumable";
            m_SellableItems[5].productType = ProductType.NonConsumable;

            //10X Coins
            m_SellableItems[6].type = eIAPItems.IAP_10XCoins;
            m_SellableItems[6].identifier = "IAP_10XCoins";
            m_SellableItems[6].appleID = "WordChain.TenXCoins";
            m_SellableItems[6].androidID = "com.unity3d.test.services.purchasing.nonconsumable";
            m_SellableItems[6].productType = ProductType.NonConsumable;           

            //10X Coins - Remove Ads
            m_SellableItems[7].type = eIAPItems.IAP_10XCoinsRemoveAllAds;
            m_SellableItems[7].identifier = "IAP_10XCoinsRemoveAllAds";
            m_SellableItems[7].appleID = "WordChain.TenXCoinsRemoveAllAds";
            m_SellableItems[7].androidID = "com.unity3d.test.services.purchasing.nonconsumable";
            m_SellableItems[7].productType = ProductType.NonConsumable;

            //Infinite Coins - Remove Ads
            m_SellableItems[8].type = eIAPItems.IAP_InfiniteCoinsRemoveAllAds;
            m_SellableItems[8].identifier = "IAP_InfiniteCoinsRemoveAllAds";
            m_SellableItems[8].appleID = "WordChain.InfiniteCoinsRemoveAllAds";
            m_SellableItems[8].androidID = "com.unity3d.test.services.purchasing.nonconsumable";
            m_SellableItems[8].productType = ProductType.NonConsumable;

            //Init Callback
            m_Callback = null;
            m_ItemBeingPurchased = new IAPInfo();

            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }
    }

    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }

        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // Add all products to sell / restore by way of its identifier, associating the general identifier with its store-specific identifiers.
        for (int i = 0; i < (int)eIAPItems.IAP_LEN; ++i)
        {
			builder.AddProduct(m_SellableItems[i].identifier, m_SellableItems[i].productType, new IDs() {
                {
                    m_SellableItems[i].appleID,
                    AppleAppStore.Name
                },
                {
                    m_SellableItems[i].androidID,
                    GooglePlay.Name
                },
            });
        }

        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Public Section (Callable)
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void BuyItem(eIAPItems _item)
    {
        //In purchase mode?
		if (m_CurrentlyPurchasing) { return; }

        //Find the correct ID
        for (int i = 0; i < (int)eIAPItems.IAP_LEN; ++i)
        {
            if (m_SellableItems[i].type == _item)
            {
				//Set to purchase mode
				m_CurrentlyPurchasing = true;
                //Buy product
                BuyProductID(m_SellableItems[i].identifier);
                //Hold a reference
                m_ItemBeingPurchased = m_SellableItems[i];                
                return;
            }
        }
    }

    // Restore purchases previously made by this customer. Some platforms automatically restore purchases. Apple currently requires explicit purchase restoration for IAP.
    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
             Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) =>
            {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then no purchases are available to be restored.
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        m_CurrentlyPurchasing = false;

		//Handle Restore having no ID
		if (m_ItemBeingPurchased.identifier == null) 
		{
			Debug.Log ("Restoring Item in Purchase");
			for (int i = 0; i < (int)eIAPItems.IAP_LEN; ++i) {
				if (m_SellableItems [i].identifier == args.purchasedProduct.definition.id) {
					m_ItemBeingPurchased = m_SellableItems[i];
					break;
				}
			}		
		}

        if (String.Equals(args.purchasedProduct.definition.id, m_ItemBeingPurchased.identifier, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            m_Callback(true, m_ItemBeingPurchased.type);
        }
        else
        {
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
            m_Callback(false, m_ItemBeingPurchased.type);
        }

        return PurchaseProcessingResult.Complete;
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Private Section
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    private void BuyProductID(string productId)
    {
        // If the stores throw an unexpected exception, use try..catch to protect my logic here.
        try
        {
            // If Purchasing has been initialized ...
            if (IsInitialized())
            {
                // ... look up the Product reference with the general product identifier and the Purchasing system's products collection.
                Product product = m_StoreController.products.WithID(productId);

                // If the look up found a product for this device's store and that product is ready to be sold ... 
                if (product != null && product.availableToPurchase)
                {
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));// ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
                    m_StoreController.InitiatePurchase(product);
                }
                // Otherwise ...
                else
                {
                    // ... report the product look-up failure situation  
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            // Otherwise ...
            else
            {
                // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or retrying initiailization.
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }
        // Complete the unexpected exception handling ...
        catch (Exception e)
        {
            // ... by reporting any unexpected exception for later diagnosis.
            Debug.Log("BuyProductID: FAIL. Exception during purchase. " + e);
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
		m_CurrentlyPurchasing = false;

        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing this reason with the user.
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }
}