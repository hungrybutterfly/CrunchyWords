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

        Text Score = GameObject.Find("Chain").GetComponent<Text>();
        string Number = Session.FormatNumberString(Session.m_BestChain.ToString());
        Score.text = Number;

        Text From = GameObject.Find("Possible").GetComponent<Text>();
        Number = Session.FormatNumberString(Session.m_WordsAvailable.ToString());
        From.text = Number;

        Text Word = GameObject.Find("Word").GetComponent<Text>();
        Word.text = Session.m_LastWord.Word;

        Text Points = GameObject.Find("Points").GetComponent<Text>();
        Number = Session.FormatNumberString(Session.m_LastScore.ToString());
        Points.text = Number;

        Text UnbrokenChain = GameObject.Find("UnbrokenChain").GetComponent<Text>();
        UnbrokenChain.text = Session.m_SaveData.sd_CurrentChain.ToString();

        Text BestUnbrokenChain = GameObject.Find("BestUnbrokenChain").GetComponent<Text>();
        BestUnbrokenChain.text = Session.m_SaveData.sd_BestChain.ToString();

        Text Jumble = GameObject.Find("JumbleCount").GetComponent<Text>();
        Jumble.text = Session.m_JumblesUsed.ToString();

        Text Hint = GameObject.Find("HintCount").GetComponent<Text>();
        Hint.text = Session.m_HintsUsed.ToString();

        Text Lock = GameObject.Find("LockCount").GetComponent<Text>();
        Lock.text = Session.m_LocksUsed.ToString();
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

        Session.m_WatchAd = true;
        if (!Session.m_ZoneComplete)
            Session.ChangeScene("Level");
        else
            Session.ChangeScene("Zone");
    }

    public void StatsClicked()
    {
        SessionManager.MetricsLogEvent("ResultsStats");

        SessionManager.PlaySound("Option_Select");

        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.ChangeScene("Stats");
    }
}
