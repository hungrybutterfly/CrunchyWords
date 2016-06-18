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

	public void Start () 
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        m_Root = GameObject.Find("Canvas").transform.Find("SettingsRoot").gameObject;

        // set the version text
        GameObject versionObject = GameObject.Find("VersionText");
        if (versionObject)
        {
            Text Version = versionObject.GetComponent<Text>();
            Version.text = Session.m_Version;
        }

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

        Transform ButtonTransform = m_Root.transform.Find("SFX");
        Transform ImageTransform = ButtonTransform.Find("Image");
        Image Sprite = ImageTransform.GetComponent<Image>();
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
