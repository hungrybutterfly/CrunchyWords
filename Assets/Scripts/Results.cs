using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Results : MonoBehaviour {

    public Sprite NoStar;

    // turn this on to have stars taken away with wrong words
    static bool m_WrongWords = false;

	void Start () 
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        Text Score = GameObject.Find("Score").GetComponent<Text>();
        string Number = Session.FormatNumberString(Session.m_LastScore.ToString());
        Score.text = Number;

        Text Words = GameObject.Find("FromWords").GetComponent<Text>();
        Words.text = "From " + Session.m_LastWordsRight.ToString() + " Word";
        if (Session.m_LastWordsRight != 1)
            Words.text += "s";

        Text TotalScore = GameObject.Find("Total Score").GetComponent<Text>();
        Number = Session.FormatNumberString(Session.m_SaveData.sd_TotalScore.ToString());
        TotalScore.text = Number;
    }
	
	public void AgainClicked() 
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.m_UseLastWord = true;
        Session.ChangeScene("Play");
    }

    public void NextClicked()
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        // move to the next level
        Session.m_SaveData.sd_CurrentLevel++;
        Session.Save();

        // was this the last level in the zone
        LevelData Data = GameObject.Find("LevelData").GetComponent<LevelData>();
        if (Session.m_CurrentLevel == Data.m_Zones[Session.m_CurrentZone].m_Levels.Length - 1)
            Session.ChangeScene("Zone");
        else
            Session.ChangeScene("Level");
    }

    public void StatsClicked()
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.ChangeScene("Stats");
    }
}
