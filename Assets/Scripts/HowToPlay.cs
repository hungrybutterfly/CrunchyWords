using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HowToPlay : MonoBehaviour 
{

    int m_FlashTimer;

	void Start () 
    {
        m_FlashTimer = 0;

        // is the dictionary loading
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        if (Session.m_DictionaryObject == null)
        {
            Text Tap = GameObject.Find("Tap").GetComponent<Text>();
            Tap.text = "Loading...";
        }
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
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.ChangeScene("Cover");
    }
}
