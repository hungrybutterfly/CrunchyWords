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
        Coins100ALL,
        Watch,
    }

    public void Start()
    {
        SessionManager.MetricsLogEvent("ShopManager");
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
        case eOption.Coins100ALL:
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
            Session.ChangeScene("Advert", LoadSceneMode.Additive);
        }
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
