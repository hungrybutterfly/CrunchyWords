using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TotalCoins : MonoBehaviour {

	void Start () 
    {
        UpdateCoins();
	}
	
	public void UpdateCoins () 
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        int Coins = Session.m_SaveData.sd_TotalScore;

        Text NumberText = GetComponentInChildren<Text>();
        string Number = Session.FormatNumberString(Coins.ToString());
        NumberText.text = Number;
    }
}
