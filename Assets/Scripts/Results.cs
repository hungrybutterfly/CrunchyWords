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

        // update the wrong words text
        Text Score = GameObject.Find("Score").GetComponent<Text>();

        int StarsEarned = 0;
        if (m_WrongWords)
        {
            // stars are taken away with wrong words
            int WrongWords = Session.m_LastScoreWrong;
            StarsEarned = 3 - WrongWords;
            if (StarsEarned < 0)
                StarsEarned = 0;

            Score.text = WrongWords.ToString() + " Wrong";
        }
        else
        {
            // stars are earned with more completed words
            int WordsCompleted = Session.m_WordsCompleted;
            int PercentComplete = (WordsCompleted * 100) / Session.m_WordsAvailable;
            if (PercentComplete >= 50)
                StarsEarned = 1;
            if (PercentComplete >= 75)
                StarsEarned = 2;
            if (PercentComplete == 100)
                StarsEarned = 3;

            Score.text = PercentComplete.ToString() + "% Completed";
        }

        // update the stars
        Image Star = GameObject.Find("Star1").GetComponent<Image>();
        if (StarsEarned < 1)
            Star.sprite = NoStar;
        Star = GameObject.Find("Star2").GetComponent<Image>();
        if (StarsEarned < 2)
            Star.sprite = NoStar;
        Star = GameObject.Find("Star3").GetComponent<Image>();
        if (StarsEarned < 3)
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
