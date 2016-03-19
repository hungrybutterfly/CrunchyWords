using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Results : MonoBehaviour {

    public Sprite NoStar;

	void Start () 
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        int WrongWords = Session.m_LastScoreWrong;

        // update the wrong words text
        Text WordsWrong = GameObject.Find("WordsWrong").GetComponent<Text>();
        WordsWrong.text = WrongWords.ToString() + " Wrong";

        // update the stars
        Image Star = GameObject.Find("Star3").GetComponent<Image>();
        if (WrongWords > 0)
            Star.sprite = NoStar;
        Star = GameObject.Find("Star2").GetComponent<Image>();
        if (WrongWords > 1)
            Star.sprite = NoStar;
        Star = GameObject.Find("Star1").GetComponent<Image>();
        if (WrongWords > 2)
            Star.sprite = NoStar;
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
        Session.ChangeScene("Play");
    }

    public void StatsClicked()
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.ChangeScene("Stats");
    }
}
