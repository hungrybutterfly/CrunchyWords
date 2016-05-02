using UnityEngine;
using System.Collections;

public class LevelData : MonoBehaviour 
{
    public class Level
    {
        public int m_MinWordCount;
        public int m_MaxWordCount;
        public string m_Word;

        public Level(int Min, int Max, string Word)
        {
            m_MinWordCount = Min;
            m_MaxWordCount = Max;
            m_Word = Word;
        }
    }

    public class ZoneData
    {
        public string m_Name;
        public Level[] m_Levels;

        public ZoneData(string Name, Level[] Levels)
        {
            m_Name = Name;
            m_Levels = Levels;
        }
    }

    static Level[] m_Zone1_Levels =
    {
        new Level(10, 18, "SUGARY"),
        new Level(10, 18, "PEAKED"),
        new Level(10, 18, "SUNDRY"),
        new Level(10, 18, "TUCKER"),
        new Level(10, 18, "DUFFEL"),
        new Level(10, 18, "FACADE"),
    };

    static Level[] m_Zone2_Levels =
    {
        new Level(10, 18, "REBELS"),
        new Level(10, 18, "ADDLED"),
        new Level(10, 18, "BARMAN"),
        new Level(10, 18, "LOOTER"),
        new Level(10, 18, "RETIRE"),
        new Level(10, 18, "ECHOED"),
    };

    static Level[] m_Zone3_Levels =
    {
        new Level(10, 18, "FLORID"),
        new Level(10, 18, "ROSILY"),
        new Level(10, 18, "WAVING"),
        new Level(10, 18, "COLLAR"),
        new Level(10, 18, "AFRAID"),
        new Level(10, 18, "HURDLE"),
    };

    static Level[] m_Zone4_Levels =
    {
        new Level(10, 18, "BASICS"),
        new Level(10, 18, "MIDWAY"),
        new Level(10, 18, "CHIEFS"),
        new Level(10, 18, "EXHORT"),
        new Level(10, 18, "CLOUDY"),
        new Level(10, 18, "MOULDS"),
    };

    static Level[] m_Zone5_Levels =
    {
        new Level(10, 18, "MAINLY"),
        new Level(10, 18, "KINDLY"),
        new Level(10, 18, "COAXES"),
        new Level(10, 18, "MUCKED"),
        new Level(10, 18, "MENDED"),
        new Level(10, 18, "INBORN"),
    };

    static Level[] m_Zone6_Levels =
    {
        new Level(10, 18, "BABIES"),
        new Level(10, 18, "HITHER"),
        new Level(10, 18, "CREOLE"),
        new Level(10, 18, "QUAINT"),
        new Level(10, 18, "BESETS"),
        new Level(10, 18, "GRAVER"),
    };

    public ZoneData[] m_Zones =
    {
        new ZoneData("A Zone", m_Zone1_Levels),
        new ZoneData("B Zone", m_Zone2_Levels),
        new ZoneData("C Zone", m_Zone3_Levels),
        new ZoneData("D Zone", m_Zone4_Levels),
        new ZoneData("E Zone", m_Zone5_Levels),
        new ZoneData("F Zone", m_Zone6_Levels),
    };
}
