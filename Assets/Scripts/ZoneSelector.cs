using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
        }
        else
        {
            Session.m_CurrentZone = m_Index;
            Session.ChangeScene("Level");
        }
	}
}
