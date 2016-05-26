using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneSettings : MonoBehaviour 
{
    public bool m_UseZoneColour = false;

    public Color GetPanelColour()
    {
        // get the current zone data
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        LevelData Data = GameObject.Find("SessionManager").GetComponent<LevelData>();

        int ZoneIndex = Session.m_CurrentZone;
        LevelData.ZoneData Zone = Data.m_Zones[ZoneIndex];

        // set the bg colour
        Color Colour;
        MyMisc.HexToColour(Data.m_Zones[ZoneIndex].m_Colour, out Colour);

        // set the zone panel colour
        Colour.r *= 0.85f;
        Colour.g *= 0.85f;
        Colour.b *= 0.85f;

        return Colour;
    }

    void Start()
    {
        if (m_UseZoneColour)
        {
            // get the current zone data
            SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

            // populate the ScrollView with buttons according to the level data
            LevelData Data = GameObject.Find("SessionManager").GetComponent<LevelData>();

            int ZoneIndex = Session.m_CurrentZone;
            LevelData.ZoneData Zone = Data.m_Zones[ZoneIndex];

            // set the bg colour
            Color Colour;
            MyMisc.HexToColour(Data.m_Zones[ZoneIndex].m_Colour, out Colour);
            GameObject.Find("Main Camera").GetComponent<Camera>().backgroundColor = Colour;

            // set the zone panel colour
            Colour.r *= 0.85f;
            Colour.g *= 0.85f;
            Colour.b *= 0.85f;

            if (GameObject.Find("TitleBar"))
            {
                Image Bar = GameObject.Find("TitleBar").GetComponent<Image>();
                Bar.color = Colour;
            }
            if (GameObject.Find("BottomBar"))
            {
                Image Bar = GameObject.Find("BottomBar").GetComponent<Image>();
                Bar.color = Colour;
            }
        }
    }
}
