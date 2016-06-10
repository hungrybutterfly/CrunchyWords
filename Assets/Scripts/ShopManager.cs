using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    enum eOption
    {        
        Option_1420Coins,
        Option_3530Coins,
        Option_10000Coins,
        Option_2XCoins,
        Option_2XCoinsRemoveStatics,
        Option_2XCoinsRemoveAllAds,
        Option_10XCoins,
        Option_10XCoinsRemoveAllAds,
        Option_InfiniteCoinsRemoveAllAds,
        Option_Watch,
    }

    public void Start()
    {
        SessionManager.MetricsLogEvent("ShopManager");

        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        // hide the debug buttons if this is an external version
        if (Session.m_ExternalVersion)
        {
            GameObject.Find("Add Coins").SetActive(false);
        }
    }

    public void Update()
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        // disable options that don't make sense to keep active
        if (Session.m_SaveData.sd_RemoveStaticAds != 0 || Session.m_SaveData.sd_RemoveALLAds != 0)
        {
            Button Option = GameObject.Find("ShopOption4").GetComponentInChildren<Button>();
            Option.interactable = false;
        }
        if (Session.m_SaveData.sd_RemoveALLAds != 0)
        {
            Button Option = GameObject.Find("ShopOption5").GetComponentInChildren<Button>();
            Option.interactable = false;
        }

        TotalCoins[] Coins = GameObject.FindObjectsOfType<TotalCoins>();
        for (int i = 0; i < Coins.Length; i++)
        {
            Coins[i].UpdateCoins();
        }
    }

    // process a successful purchase
    void OptionPurchasedSuccess(eOption Option)
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        switch (Option)
        {
            case eOption.Option_1420Coins:
                Session.m_SaveData.AddCoins(1420);
                break;
            case eOption.Option_3530Coins:
                Session.m_SaveData.AddCoins(3530);
                break;
            case eOption.Option_10000Coins:
                Session.m_SaveData.AddCoins(10000);
                break;
            case eOption.Option_2XCoins:
                Session.m_SaveData.sd_DoubleCoins = 1;
                break;
            case eOption.Option_2XCoinsRemoveStatics:
                Session.m_SaveData.sd_DoubleCoins = 1;
                Session.m_SaveData.sd_RemoveStaticAds = 1;
                break;
            case eOption.Option_2XCoinsRemoveAllAds:
                Session.m_SaveData.sd_DoubleCoins = 1;
                Session.m_SaveData.sd_RemoveALLAds = 1;
                break;
            case eOption.Option_10XCoins:
                Session.m_SaveData.sd_X10Coins = 1;
                break;
            case eOption.Option_10XCoinsRemoveAllAds:
                Session.m_SaveData.sd_X10Coins = 1;
                Session.m_SaveData.sd_RemoveALLAds = 1;
                break;
            case eOption.Option_InfiniteCoinsRemoveAllAds:
                Session.m_SaveData.sd_InfinteCoins = 1;
                Session.m_SaveData.sd_RemoveALLAds = 1;
                break;
            case eOption.Option_Watch:
                Session.m_SaveData.AddCoins(100);
                break;
        }

        Session.Save();
    }

    // process an unsuccessful purchase
    void OptionPurchasedFail(int Option)
    {
    }

    public void OptionClicked(string Option)
    {
        SessionManager.MetricsLogEventWithParameters("ShopOptionClicked", new Dictionary<string, string>() { { "Option", Option } });
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        SessionManager.PlaySound("Option_Select");

        // did player select to watch a video
		eOption OptionIndex = (eOption)int.Parse (Option);
		if (OptionIndex == eOption.Option_Watch) 
		{
			Session.m_AdvertStatic = false;
			Session.m_AdvertCount = 1;
			Session.ChangeScene ("Advert", LoadSceneMode.Additive);
		} 
		else
		{
			//Chris - This now splits depending on 'External Version'
			if (Session.m_ExternalVersion) 
			{
				//REAL IAP
				IAPurchaser.eIAPItems OptionIndexReal = (IAPurchaser.eIAPItems)int.Parse(Option);
				//Buy the correct item
				IAPurchaser PurchaseManager = Session.m_IAPManager;
				PurchaseManager.m_Callback = IAPReturn;
				PurchaseManager.BuyItem(OptionIndexReal);
			} 
			else 
			{
				//FAKE
				OptionPurchasedSuccess (OptionIndex);
			}
		}
    }

    ////////////////////////////////////////////////////////////////////////////////////////
    // Real IAP option - Return callback
    ////////////////////////////////////////////////////////////////////////////////////////
    public void IAPReturn(bool _success, IAPurchaser.eIAPItems _item)
    {
        //Take the item
        eOption OptionIndex = (eOption)((int)_item);

        //Not a success? go to fail...
        if (!_success)
        {
            OptionPurchasedFail((int)_item);
        }

        //Unlock/Purchase item
        OptionPurchasedSuccess(OptionIndex);
    }

    ////////////////////////////////////////////////////////////////////////////////////////
    // Real IAP option - Return callback
    ////////////////////////////////////////////////////////////////////////////////////////
    public void RestorePurchases()
    {
		SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
		IAPurchaser PurchaseManager = Session.m_IAPManager;
		PurchaseManager.m_Callback = IAPReturn;
        PurchaseManager.RestorePurchases();
    }


    public void BackClicked()
    {
        SessionManager.MetricsLogEvent("ShopBack");
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.ReturnScene("Shop");
        SessionManager.PlaySound("Option_Back");
    }

    public void AddCoinsClicked()
    {
        // cheat to add 1000 coins
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.m_SaveData.AddCoins(1000);
        SessionManager.PlaySound("Option_Select");
    }
}
