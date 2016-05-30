using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

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

    public void SFXClicked()
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        SessionManager.MetricsLogEventWithParameters("SFXClicked", new Dictionary<string, string>() { { "On", Session.m_Settings.m_SFXEnabled.ToString() } });

        Session.ToggleSFX();
        UpdateButtons();
    }

    public void MusicClicked()
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        SessionManager.MetricsLogEventWithParameters("MusicClicked", new Dictionary<string, string>() { { "On", Session.m_Settings.m_MusicEnabled.ToString() } });

        Session.ToggleMusic();
        UpdateButtons();
    }
}
