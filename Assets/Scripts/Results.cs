using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Results : MonoBehaviour {

    public Sprite NoStar;

    // turn this on to have stars taken away with wrong words
    static bool m_WrongWords = false;

	void Start () 
    {
        SessionManager.MetricsLogEvent("Results");

        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        Text Score = GameObject.Find("Score").GetComponent<Text>();
        string Number = Session.FormatNumberString(Session.m_LastScore.ToString());
        Score.text = Number;

        Text Words = GameObject.Find("FromWords").GetComponent<Text>();
        Words.text = "From " + Session.m_LastWordsRight.ToString() + " Word";
        if (Session.m_LastWordsRight != 1)
            Words.text += "s";

        Text Chain = GameObject.Find("Chain").GetComponent<Text>();
        Chain.text = "Best Chain " + Session.m_BestChain.ToString();

        Text ContinuousChain = GameObject.Find("Continuous Chain").GetComponent<Text>();
        ContinuousChain.text = "Continuous Chain " + Session.m_SaveData.sd_CurrentChain.ToString();
    }
	
	public void AgainClicked() 
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.m_UseLastWord = true;
        Session.ChangeScene("Play");
    }

    public void NextClicked()
    {
        SessionManager.MetricsLogEvent("ResultsNext");

        SessionManager.PlaySound("Option_Select");

        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        SessionManager.MetricsLogEventWithParameters("SaveData", new Dictionary<string, string>() 
        { 
            { "BestChain", Session.m_SaveData.sd_BestChain.ToString() }, 
            { "CorrectSubmits", Session.m_SaveData.sd_CorrectSubmits.ToString() }, 
            { "IncorrectSubmits", Session.m_SaveData.sd_IncorrectSubmits.ToString() }, 
            { "LevelsComplete", Session.m_SaveData.sd_LevelsComplete.ToString() }, 
            { "TotalScore", Session.m_SaveData.sd_TotalScore.ToString() }, 
        });

        // move to the next level
        Session.m_SaveData.sd_CurrentLevel++;
        Session.Save();

        Session.ChangeScene("Advert");
    }

    public void StatsClicked()
    {
        SessionManager.MetricsLogEvent("ResultsStats");

        SessionManager.PlaySound("Option_Select");

        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.ChangeScene("Stats");
    }
}
