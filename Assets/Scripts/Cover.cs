using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Cover : MonoBehaviour 
{
    public Sprite m_SFXOn;
    public Sprite m_SFXOff;
    public Sprite m_MusicOn;
    public Sprite m_MusicOff;

    void Start()
    {
        SessionManager.MetricsLogEvent("Cover");

        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        // set the version text
		GameObject versionObject = GameObject.Find("VersionText");
		if (versionObject) 
		{
			Text Version = versionObject.GetComponent<Text> ();
			Version.text = Session.m_Version;
		}

        // temp code chopped out. Leave for a bit in case we need it back in
/*		GameObject puzzleSolvedObject = GameObject.Find ("Puzzles Solved");
		if (puzzleSolvedObject) 
		{
			Text Solved = puzzleSolvedObject.GetComponent<Text>();
			Solved.text = Session.m_SaveData.sd_PuzzlesSolved.ToString () + " Puzzles Solved";
		}

		GameObject vocabularyObject = GameObject.Find ("Your Vocabulary");
		if (vocabularyObject) 
		{
			Text Vocabulary = vocabularyObject.GetComponent<Text> ();
			DictionaryManager Dictionary = GameObject.Find ("DictionaryManager").GetComponent<DictionaryManager> ();
			int FoundCount = 0;
			for (int i = 0; i < 26; i++)
				FoundCount += Session.m_WordFoundCounts [i];
			string Number = Session.FormatNumberString (Dictionary.m_FinalWordCount.ToString ());
			Vocabulary.text = "Total Words  " + FoundCount.ToString () + " / " + Number;
		}

		GameObject chainObject = GameObject.Find ("Best Chain");
		if(chainObject)
		{
			Text Chain = chainObject.GetComponent<Text>();
			Chain.text = "Best Chain " + Session.m_SaveData.sd_BestChain.ToString();
		}*/

        // hide the debug buttons if this is an external version
        if (Session.m_ExternalVersion)
        {
            GameObject Button = GameObject.Find("Clear");
			if (Button) 
			{
				Button.SetActive (false);
			}
        }

        UpdateButtons();
    }

    void UpdateButtons()
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        Image Sprite = GameObject.Find("SFX").transform.Find("Image").GetComponent<Image>();
        if (Session.m_Settings.m_SFXEnabled == 1)
            Sprite.sprite = m_SFXOn;
        else
            Sprite.sprite = m_SFXOff;

        Sprite = GameObject.Find("Music").transform.Find("Image").GetComponent<Image>();
        if (Session.m_Settings.m_MusicEnabled == 1)
            Sprite.sprite = m_MusicOn;
        else
            Sprite.sprite = m_MusicOff;
    }

    public void StartClicked()
    {
        SessionManager.MetricsLogEvent("CoverStartClicked");

        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.ChangeScene("Zone");
        SessionManager.PlaySound("Option_Select");
    }

    public void HowToPlayClicked()
    {
        SessionManager.MetricsLogEvent("CoverHowToPlayClicked");

        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.ChangeScene("HowToPlay");
        SessionManager.PlaySound("Option_Select");
    }

	public void ClearClicked() 
    {
        SessionManager.MetricsLogEvent("ClearClicked");

        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.CreateNewSaveData();
        Session.ChangeScene("Cover");
        SessionManager.PlaySound("Option_Select");
    }

    public void ShopClicked()
    {
        SessionManager.MetricsLogEvent("CoverShopClicked");

        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.ChangeScene("Shop", LoadSceneMode.Additive);
        SessionManager.PlaySound("Option_Select");
    }

    public void AddCoinsClicked()
    {
        SessionManager.MetricsLogEvent("AddCoinsClicked");

        // cheat to add 1000 coins
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.m_SaveData.AddCoins(1000);
        SessionManager.PlaySound("Option_Select");
    }

    public void SFXClicked()
    {
        SessionManager.MetricsLogEvent("SFXClicked");

        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.ToggleSFX();
        UpdateButtons();
    }

    public void MusicClicked()
    {
        SessionManager.MetricsLogEvent("SFXClicked");

        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.ToggleMusic();
        UpdateButtons();
    }
}
