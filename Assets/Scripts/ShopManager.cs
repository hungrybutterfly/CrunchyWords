using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    enum eOption
    {
        Coins100,
        Coins250,
        Coins600,
        Coins100Static,
        Coins250ALL,
        Watch,
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
            case eOption.Coins100:
                Session.m_SaveData.AddCoins(100);
                break;
            case eOption.Coins250:
                Session.m_SaveData.AddCoins(250);
                break;
            case eOption.Coins600:
                Session.m_SaveData.AddCoins(600);
                break;
            case eOption.Coins100Static:
                Session.m_SaveData.AddCoins(100);
                Session.m_SaveData.sd_RemoveStaticAds = 1;
                break;
            case eOption.Coins250ALL:
                Session.m_SaveData.AddCoins(250);
                Session.m_SaveData.sd_RemoveALLAds = 1;
                break;
            case eOption.Watch:
                Session.m_SaveData.AddCoins(10);
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

        eOption OptionIndex = (eOption)int.Parse(Option);
        OptionPurchasedSuccess(OptionIndex);

        // did player select to watch a video
        if (OptionIndex == eOption.Watch)
        {
            Session.m_AdvertStatic = false;
            Session.m_AdvertCount = 1;
            Session.ChangeScene("Advert", LoadSceneMode.Additive);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////
    // Real IAP option
    ////////////////////////////////////////////////////////////////////////////////////////
    public void OptionClickedReal(string Option)
    {
#if (!UNITY_IOS)
        return;
#else
        SessionManager.MetricsLogEventWithParameters("ShopOptionClicked", new Dictionary<string, string>() { { "Option", Option } });
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        SessionManager.PlaySound("Option_Select");

        IAPurchaser.eIAPItems OptionIndex = (IAPurchaser.eIAPItems)int.Parse(Option);

        //Buy the correct item
		IAPurchaser PurchaseManager = Session.m_IAPManager;
		PurchaseManager.m_Callback = IAPReturn;
        PurchaseManager.BuyItem(OptionIndex);
#endif
    }

    ////////////////////////////////////////////////////////////////////////////////////////
    // Real IAP option - Return callback
    ////////////////////////////////////////////////////////////////////////////////////////
    public void IAPReturn(bool _success, IAPurchaser.eIAPItems _item)
    {
		if (_item == null) 
		{
			Debug.Log ("Issue - Item is null");
			return;
		}

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
        IAPurchaser PurchaseManager = GetComponent<IAPurchaser>();
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
