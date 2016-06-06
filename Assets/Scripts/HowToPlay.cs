using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HowToPlay : MonoBehaviour 
{

    int m_FlashTimer;

    bool m_DictionaryLoading;

    float m_StartTime;

    GameObject m_BackButton;
    GameObject m_PlayButton;

	void Start () 
    {
        SessionManager.MetricsLogEvent("HowToPlay");

        m_FlashTimer = 0;

        m_BackButton = GameObject.Find("Back");
        m_PlayButton = GameObject.Find("Play");

        // is the dictionary loading
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        if (Session.m_Settings.m_HowToSeen == 0)
        {
            Session.m_Settings.m_HowToSeen = 1;
            Session.SaveSettings();

            m_BackButton.SetActive(false);
            m_PlayButton.SetActive(false);

            m_DictionaryLoading = true;
        }
        else
        {
            if (GameObject.Find("Tap"))
                GameObject.Find("Tap").SetActive(false);

            m_PlayButton.SetActive(false);

            m_DictionaryLoading = false;
        }

        GameObject.Find("Content").GetComponent<RectTransform>().sizeDelta = GameObject.Find("Content").GetComponent<RectTransform>().sizeDelta;

        m_StartTime = Time.time;
    }
	
	void Update () 
    {
        if (m_DictionaryLoading)
        {
            // is the dictionary done loading
            SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
            if (Session.m_DictionaryObject != null)
            {
                if (GameObject.Find("Tap"))
                    GameObject.Find("Tap").SetActive(false);

                m_PlayButton.SetActive(true);
            }
        }
    }

    public void Click()
    {
        int TotalTime = (int) (Time.time - m_StartTime);
        int Loading = 0;
        if (m_DictionaryLoading)
            Loading = 1;

        SessionManager.MetricsLogEventWithParameters("HowToPlayEnd", new Dictionary<string, string>() 
        { 
            { "Time", TotalTime.ToString() },
            { "Loading", Loading.ToString() },
        });

        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.ChangeScene("Cover");

        SessionManager.PlaySound("Option_Back");
    }
}
