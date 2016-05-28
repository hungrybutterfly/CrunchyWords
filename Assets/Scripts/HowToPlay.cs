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

	void Start () 
    {
        SessionManager.MetricsLogEvent("HowToPlay");

        m_FlashTimer = 0;

        // is the dictionary loading
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        if (Session.m_DictionaryObject == null)
        {
            m_BackButton = GameObject.Find("Back");
            Text String = m_BackButton.GetComponentInChildren<Text>();
            String.text = "PLAY";
            m_BackButton.SetActive(false);

            m_DictionaryLoading = true;
        }
        else
        {
            if (GameObject.Find("Tap"))
                GameObject.Find("Tap").SetActive(false);

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

                m_BackButton.SetActive(true);
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
