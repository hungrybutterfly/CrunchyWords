using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class Cover : MonoBehaviour 
{
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

    public void SettingsClicked()
    {
        SessionManager.MetricsLogEvent("CoverSettingsClicked");

        SessionManager.PlaySound("Option_Select");

        PlayerSettings Settings = GameObject.Find("Canvas").transform.Find("SettingsRoot").GetComponent<PlayerSettings>();
        Settings.SetIsActive(true);
    }
}
