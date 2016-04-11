using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Cover : MonoBehaviour 
{
    void Start()
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        Text Solved = GameObject.Find("Puzzles Solved").GetComponent<Text>();
        Solved.text = Session.m_SaveData.sd_PuzzlesSolved.ToString() + " Puzzles Solved";

        Text Vocabulary = GameObject.Find("Your Vocabulary").GetComponent<Text>();
        DictionaryManager Dictionary = GameObject.Find("DictionaryManager").GetComponent<DictionaryManager>();
        int FoundCount = 0;
        for (int i = 0; i < 26; i++)
            FoundCount += Session.m_SaveData.sd_WordFoundCounts[i];
        Vocabulary.text = "Vocabulary  " + FoundCount.ToString() + " / " + Dictionary.m_FinalWordCount.ToString();

        Text Coins = GameObject.Find("Coin Text").GetComponent<Text>();
        Coins.text = Session.m_SaveData.sd_TotalScore.ToString();
    }

	public void ClearClicked() 
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.CreateNewSaveData();
        Session.ChangeScene("Cover");
	}

    public void AddCoinsClicked()
    {
        // cheat to add 1000 coins
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.m_SaveData.sd_TotalScore += 1000;

        Text Coins = GameObject.Find("Coin Text").GetComponent<Text>();
        Coins.text = Session.m_SaveData.sd_TotalScore.ToString();
    }
}
