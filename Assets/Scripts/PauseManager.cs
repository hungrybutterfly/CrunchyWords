using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PauseManager : MonoBehaviour 
{
    public GameObject m_Root;

    [HideInInspector]
    public bool m_PauseEnabled = false;

	void Start () 
    {
    }

    public void SetIsEnabled(bool Active)
    {
        m_Root.gameObject.SetActive(Active);

        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        if (Active)
        {
            // set the total coins text
            GameObject Object = GameObject.Find("End Cost"); 
            Text Value = Object.GetComponentInChildren<Text>();
            GameManager Game = GameObject.Find("GameManager").GetComponent<GameManager>();
            Value.text = Game.m_RevealWordsCost.ToString();
        }

        // hide debug buttons for external release
        if (Session.m_ExternalVersion)
        {
            GameObject Object = GameObject.Find("End");
            Object.SetActive(false);
            Object = GameObject.Find("End Cost");
            Object.SetActive(false);
        }

        m_PauseEnabled = Active;
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
        Session.m_SaveData.AddCoins(Game.m_TotalScore);

        // move to the next puzzle
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
