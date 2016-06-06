using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerSettings : MonoBehaviour 
{
    public Sprite m_SFXOn;
    public Sprite m_SFXOff;
    public Sprite m_MusicOn;
    public Sprite m_MusicOff;

    GameObject m_Root;

	void Start () 
    {
        m_Root = GameObject.Find("Canvas").transform.Find("SettingsRoot").gameObject;

        UpdateButtons();	
	}

    public void SetIsActive(bool Active)
    {
        gameObject.SetActive(Active);
        UpdateButtons();
    }

    void UpdateButtons()
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        Image Sprite = m_Root.transform.Find("SFX").Find("Image").GetComponent<Image>();
        if (Session.m_Settings.m_SFXEnabled == 1)
            Sprite.sprite = m_SFXOn;
        else
            Sprite.sprite = m_SFXOff;

        Sprite = m_Root.transform.Find("Music").Find("Image").GetComponent<Image>();
        if (Session.m_Settings.m_MusicEnabled == 1)
            Sprite.sprite = m_MusicOn;
        else
            Sprite.sprite = m_MusicOff;
    }

    public void SFXClicked()
    {
        SessionManager.PlaySound("Option_Select");

        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        SessionManager.MetricsLogEventWithParameters("SFXClicked", new Dictionary<string, string>() { { "On", Session.m_Settings.m_SFXEnabled.ToString() } });

        Session.ToggleSFX();
        UpdateButtons();
    }

    public void MusicClicked()
    {
        SessionManager.PlaySound("Option_Select");

        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        SessionManager.MetricsLogEventWithParameters("MusicClicked", new Dictionary<string, string>() { { "On", Session.m_Settings.m_MusicEnabled.ToString() } });

        Session.ToggleMusic();
        UpdateButtons();
    }

    public void DoneClicked()
    {
        SessionManager.MetricsLogEvent("SettingsDoneClicked");
        SessionManager.PlaySound("Option_Back");

        SetIsActive(false);
    }
}
