using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour 
{
    enum eOption
    {
        Coins1000,
        Coins2500,
        Coins6000,
        Coins1000Static,
        Coins1000ALL,
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
    }

    // process a successful purchase
    void OptionPurchasedSuccess(eOption Option)
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        switch (Option)
        {
        case eOption.Coins1000:
            Session.m_SaveData.AddCoins(1000);
            break;
        case eOption.Coins2500:
            Session.m_SaveData.AddCoins(2500);
            break;
        case eOption.Coins6000:
            Session.m_SaveData.AddCoins(6000);
            break;
        case eOption.Coins1000Static:
            Session.m_SaveData.AddCoins(1000);
            Session.m_SaveData.sd_RemoveStaticAds = 1;
            break;
        case eOption.Coins1000ALL:
            Session.m_SaveData.AddCoins(2500);
            Session.m_SaveData.sd_RemoveALLAds = 1;
            break;
        case eOption.Watch:
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
