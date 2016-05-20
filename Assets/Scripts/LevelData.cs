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
        public string m_Colour;

        public ZoneData(string _Name, Level[] _Levels, string _Colour)
        {
            m_Name = _Name;
            m_Levels = _Levels;
            m_Colour = _Colour;
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
        new ZoneData("PACK #1", m_Zone1_Levels, "B6D7A8ff"),
        new ZoneData("PACK #2", m_Zone2_Levels, "FFE599ff"),
        new ZoneData("PACK #3", m_Zone3_Levels, "A4C2F4ff"),
        new ZoneData("PACK #4", m_Zone4_Levels, "EA9999ff"),
        new ZoneData("PACK #5", m_Zone5_Levels, "B4A7D6ff"),
        new ZoneData("PACK #6", m_Zone6_Levels, "F9CB9Cff"),
    };
}
