using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HintButton : MonoBehaviour 
{
    // has the hint button been tapped in preparation for a word to be tapped
    bool m_Ready = false;

    // nudge to player to tap the hint button by flashing it when a word has been tapped
    bool m_Nudge = false;

    // timer to flash button
    float m_NudgeTimer = 0;

    // used for flashing
    Color m_OriginalColour;

	void Start () 
    {
        m_OriginalColour = GetComponent<Image>().color;
	}

    public void SetReady(bool Ready)
    {
        m_Ready = Ready;

        if (!Ready)
            GetComponent<Image>().color = m_OriginalColour;
        else
        {
            Color Colour = GameObject.Find("SessionManager").GetComponent<SessionManager>().m_HintColour;
            GetComponent<Image>().color = Colour;
        }
    }

    public bool GetReady()
    {
        return m_Ready;
    }

    public void SetNudge(bool Nudge)
    {
        m_Nudge = Nudge;
        m_NudgeTimer = 0;
        GetComponent<Image>().color = m_OriginalColour;
    }

	void Update () 
    {
        // flash the button
        if (m_Nudge)
        {
            m_NudgeTimer += Time.deltaTime;
            if (((int)(m_NudgeTimer * 60)) % 30 < 20)
            {
                Color Colour = GameObject.Find("SessionManager").GetComponent<SessionManager>().m_HintColour;
                Colour.r = 1 - ((1 - Colour.r) * 0.5f);
                Colour.g = 1 - ((1 - Colour.g) * 0.5f);
                Colour.b = 1 - ((1 - Colour.b) * 0.5f);
                GetComponent<Image>().color = Colour;
            }
            else
                GetComponent<Image>().color = m_OriginalColour;
        }
    }
}
