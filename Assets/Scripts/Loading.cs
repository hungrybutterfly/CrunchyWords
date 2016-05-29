using UnityEngine;
using System.Collections;

public class Loading : MonoBehaviour 
{
    int m_Frame = 0;

	void Start () 
    {
        SessionManager.MetricsLogEvent("Loading");
    }

    void Update()
    {
        m_Frame++;
        if(m_Frame > 120)
        {
            SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
            if (Session.m_Settings.m_HowToSeen == 0)
            {
                Session.ChangeScene("ChooseIcon");
            }
            else
            {
                Session.ChangeScene("Cover");
            }
        }
    }
}
