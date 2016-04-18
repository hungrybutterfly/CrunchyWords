using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PauseManager : MonoBehaviour 
{
    public GameObject m_Root;

	void Start () 
    {
    }

    public void SetIsEnabled(bool Active)
    {
        m_Root.gameObject.SetActive(Active);

        // set the total coins text
        GameObject Object = GameObject.Find("End Cost");
        Text Value = Object.GetComponentInChildren<Text>();
        GameManager Game = GameObject.Find("GameManager").GetComponent<GameManager>();
        Value.text = Game.m_RevealWordsCost.ToString();
    }

    public void ResumeClicked()
    {
        SetIsEnabled(false);
    }

    public void NewClicked()
    {
        GameManager Game = GameObject.Find("GameManager").GetComponent<GameManager>();
        Game.NewWord();
        SetIsEnabled(false);
    }

    public void QuitClicked()
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        // keep the player's score
        GameManager Game = GameObject.Find("GameManager").GetComponent<GameManager>();
        Session.m_SaveData.sd_TotalScore += Game.m_TotalScore;

        // move to the next puzzle
        DictionaryManager Dictionary = GameObject.Find("DictionaryManager").GetComponent<DictionaryManager>();
        Session.m_SaveData.sd_RandomSeed = Dictionary.m_RandomSeed;
        Session.m_SaveData.sd_CurrentLevel++;

        Session.Save();

        Session.ChangeScene("Cover");
    }

    public void FinishClicked()
    {
        GameManager Game = GameObject.Find("GameManager").GetComponent<GameManager>();
        Game.Finish();
        SetIsEnabled(false);
    }
}
