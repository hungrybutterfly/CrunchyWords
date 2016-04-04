using UnityEngine;
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
        Session.ChangeScene("Cover");
    }
}
