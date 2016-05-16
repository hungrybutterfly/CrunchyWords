using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ZoneSelector : MonoBehaviour
{
    bool m_Level;
    int m_Index;

	public void Init (bool Level, int Index) 
    {
        m_Level = Level;
        m_Index = Index;

        GetComponent<Button>().onClick.AddListener(() => { OnClick(); });
    }

    void OnClick() 
    {
        // set the zone and jump to level select
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        if (m_Level)
        {
            Session.m_CurrentLevel = m_Index;
            Session.ChangeScene("Play");
            SessionManager.MetricsLogEventWithParameters("ZoneSelectorLevel", new Dictionary<string, string>() { { "Level", m_Index.ToString() } });
        }
        else
        {
            Session.m_CurrentZone = m_Index;
            Session.ChangeScene("Level");
            SessionManager.MetricsLogEventWithParameters("ZoneSelectorZone", new Dictionary<string, string>() { { "Zone", m_Index.ToString() } });
        }

        SessionManager.PlaySound("Option_Select");
    }
}
