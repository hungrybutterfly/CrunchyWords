using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Cover : MonoBehaviour 
{
    void Start()
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        // set the version text
        Text Version = GameObject.Find("VersionText").GetComponent<Text>();
        Version.text = Session.m_Version;

        Text Solved = GameObject.Find("Puzzles Solved").GetComponent<Text>();
        Solved.text = Session.m_SaveData.sd_PuzzlesSolved.ToString() + " Puzzles Solved";

        Text Vocabulary = GameObject.Find("Your Vocabulary").GetComponent<Text>();
        DictionaryManager Dictionary = GameObject.Find("DictionaryManager").GetComponent<DictionaryManager>();
        int FoundCount = 0;
        for (int i = 0; i < 26; i++)
            FoundCount += Session.m_SaveData.sd_WordFoundCounts[i];
        Vocabulary.text = "Total Words  " + FoundCount.ToString() + " / " + Dictionary.m_FinalWordCount.ToString();

        // hide the debug buttons if this is an external version
        if (Session.m_ExternalVersion)
        {
            GameObject Button = GameObject.Find("Add Coins");
            Button.SetActive(false);

            Button = GameObject.Find("Clear");
            Button.SetActive(false);
        }
    }

	public void ClearClicked() 
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.CreateNewSaveData();
        Session.ChangeScene("Cover");
	}
}
