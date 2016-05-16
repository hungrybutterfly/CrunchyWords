using UnityEngine;
using System.Collections;

public class ShopManager : MonoBehaviour 
{
    public void Start()
    {
        SessionManager.MetricsLogEvent("ShopManager");
    }

    public void AddCoinsClicked()
    {
        // cheat to add 1000 coins
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.m_SaveData.AddCoins(1000);
    }
}
