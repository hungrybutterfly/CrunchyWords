using UnityEngine;
using System.Collections;

public class LevelData : MonoBehaviour 
{
    public class Level
    {
        public int m_MinWordCount;
        public int m_MaxWordCount;

        public Level(int Min, int Max)
        {
            m_MinWordCount = Min;
            m_MaxWordCount = Max;
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
        new Level(10, 18),
        new Level(10, 18),
        new Level(10, 18),
        new Level(10, 18),
        new Level(10, 18),
        new Level(10, 18),
    };

    static Level[] m_Zone2_Levels =
    {
        new Level(10, 18),
        new Level(10, 18),
        new Level(10, 18),
        new Level(10, 18),
        new Level(10, 18),
        new Level(10, 18),
    };

    static Level[] m_Zone3_Levels =
    {
        new Level(10, 18),
        new Level(10, 18),
        new Level(10, 18),
        new Level(10, 18),
        new Level(10, 18),
        new Level(10, 18),
    };

    public ZoneData[] m_Zones =
    {
        new ZoneData("A Zone", m_Zone1_Levels),
        new ZoneData("B Zone", m_Zone2_Levels),
        new ZoneData("C Zone", m_Zone3_Levels),
    };
}
