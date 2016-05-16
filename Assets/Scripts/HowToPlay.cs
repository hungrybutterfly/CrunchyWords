using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HowToPlay : MonoBehaviour 
{

    int m_FlashTimer;

    bool m_DictionaryLoading;

    float m_StartTime;

	void Start () 
    {
        SessionManager.MetricsLogEvent("HowToPlay");

        m_FlashTimer = 0;

        // is the dictionary loading
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        if (Session.m_DictionaryObject == null)
        {
            Text Tap = GameObject.Find("Tap").GetComponent<Text>();
            Tap.text = "Loading...";
            m_DictionaryLoading = true;
        }
        else
        {
            m_DictionaryLoading = false;
        }

        m_StartTime = Time.time;
    }
	
	void Update () 
    {
        // is the dictionary done loading
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        if (Session.m_DictionaryObject != null)
        {
            Text Tap = GameObject.Find("Tap").GetComponent<Text>();
            Tap.text = "TAP!";

            m_FlashTimer++;
            Color Colour = Tap.color;
            if (m_FlashTimer % 30 < 20)
                Colour.a = 1;
            else
                Colour.a = 0;
            Tap.color = Colour;
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
    }
}
