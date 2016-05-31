using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class PauseManager : MonoBehaviour 
{
    public GameObject m_Root;

    [HideInInspector]
    public bool m_PauseEnabled = false;

    public Sprite m_SFXOn;
    public Sprite m_SFXOff;
    public Sprite m_MusicOn;
    public Sprite m_MusicOff;

    void Start() 
    {
        SceneSettings Scene = GameObject.Find("SceneSettings").GetComponent<SceneSettings>();
        Color PanelColour = Scene.GetPanelColour();

        // recolour things to the zone
        Image Panel = m_Root.gameObject.transform.Find("Image").GetComponent<Image>();
        Panel.color = PanelColour;
        Panel = m_Root.gameObject.transform.Find("ScorePanel").GetComponent<Image>();
        Panel.color = PanelColour;

        PanelColour = Scene.GetBGColour();
        PanelColour.r = (1 - PanelColour.r) * 0.5f + PanelColour.r;
        PanelColour.g = (1 - PanelColour.g) * 0.5f + PanelColour.g;
        PanelColour.b = (1 - PanelColour.b) * 0.5f + PanelColour.b;
        PanelColour.a = 0.8f;
        Panel = m_Root.gameObject.transform.Find("OptionsPanel").GetComponent<Image>();
        Panel.color = PanelColour;

        UpdateButtons();
    }

    void UpdateButtons()
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        Image Sprite = m_Root.transform.Find("SFX").transform.Find("Image").GetComponent<Image>();
        if (Session.m_Settings.m_SFXEnabled == 1)
            Sprite.sprite = m_SFXOn;
        else
            Sprite.sprite = m_SFXOff;

        Sprite = m_Root.transform.Find("Music").transform.Find("Image").GetComponent<Image>();
        if (Session.m_Settings.m_MusicEnabled == 1)
            Sprite.sprite = m_MusicOn;
        else
            Sprite.sprite = m_MusicOff;
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
        SessionManager.PlaySound("Option_Back");

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
        SessionManager.MetricsLogEvent("Quit");

        SessionManager.PlaySound("Option_Back");

        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        Session.ChangeScene("Level");
    }

    public void FinishClicked()
    {
        SessionManager.PlaySound("Option_Select");

        GameManager Game = GameObject.Find("GameManager").GetComponent<GameManager>();
        Game.Finish();
        SetIsEnabled(false);
    }

    public void ShopClicked()
    {
        SessionManager.MetricsLogEvent("PauseShop");

        SessionManager.PlaySound("Option_Select");

        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.ChangeScene("Shop", LoadSceneMode.Additive);
    }

    public void SFXClicked()
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        SessionManager.MetricsLogEventWithParameters("PauseSFXClicked", new Dictionary<string, string>() { { "On", Session.m_Settings.m_SFXEnabled.ToString() } });

        Session.ToggleSFX();
        UpdateButtons();
    }

    public void MusicClicked()
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        SessionManager.MetricsLogEventWithParameters("PauseMusicClicked", new Dictionary<string, string>() { { "On", Session.m_Settings.m_MusicEnabled.ToString() } });

        Session.ToggleMusic();
        UpdateButtons();
    }
}
