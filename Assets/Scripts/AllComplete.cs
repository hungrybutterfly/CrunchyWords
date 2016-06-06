using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AllComplete : MonoBehaviour 
{
    void Start()
    {
        SessionManager.MetricsLogEvent("AllComplete");

        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        // make sure we don't see the 'zone complete' ceremony
        Session.m_ZoneComplete = false;
    }

    public void EmailUsClicked()
    {
        SessionManager.MetricsLogEvent("EmailUs");

        //email Id to send the mail to
        string email = "support@denki.co.uk";
        //subject of the mail
        string subject = EscapeURL("More Word Chains packs please!");
        //body of the mail which consists of Device Model and its Operating System
        string body = EscapeURL("Please give me some more Word Chains packs to play!\n\n\n\n" +
         "________" +
         "\n\nPlease Do Not Modify This\n\n" +
         "Model: " + SystemInfo.deviceModel + "\n\n" +
            "OS: " + SystemInfo.operatingSystem + "\n\n" +
         "________");
        //Open the Default Mail App
        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
    }

    string EscapeURL(string url)
    {
        return WWW.EscapeURL(url).Replace("+", "%20");
    }

    public void BackClicked()
    {
        SessionManager.MetricsLogEvent("AllCompleteBack");

        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.ChangeScene("Zone");
    }
}
