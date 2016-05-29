﻿using UnityEngine;
using System;
using System.Collections.Generic;
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
        new Level(10, 18, "PEOPLE"),
        new Level(10, 18, "WAXING"),
        new Level(10, 18, "BEFORE"),
        new Level(10, 18, "LOVELY"),
        new Level(10, 18, "SQUARE"),
        new Level(10, 18, "JOKING"),
    };

    static Level[] m_Zone2_Levels =
    {
        new Level(10, 18, "LITTLE"),
        new Level(10, 18, "THOUGH"),
        new Level(10, 18, "SYNTAX"),
        new Level(10, 18, "ZOOMED"),
        new Level(10, 18, "POTATO"),
        new Level(10, 18, "RHYTHM"),
    };

    static Level[] m_Zone3_Levels =
    {
        new Level(10, 18, "SHOULD"),
        new Level(10, 18, "APPEAR"),
        new Level(10, 18, "BURGER"),
        new Level(10, 18, "IMPALA"),
        new Level(10, 18, "PHLEGM"),
        new Level(10, 18, "FLORID"),
    };

    static Level[] m_Zone4_Levels =
    {
        new Level(10, 18, "KISSED"),
        new Level(10, 18, "NUMBER"),
        new Level(10, 18, "PLAQUE"),
        new Level(10, 18, "SORREL"),
        new Level(10, 18, "WAGGLY"),
        new Level(10, 18, "JINXED"),
    };

    static Level[] m_Zone5_Levels =
    {
        new Level(10, 18, "ADDING"),
        new Level(10, 18, "STICKY"),
        new Level(10, 18, "CREEPS"),
        new Level(10, 18, "SUGARY"),
        new Level(10, 18, "HASHED"),
        new Level(10, 18, "ECZEMA"),
    };

    static Level[] m_Zone6_Levels =
    {
        new Level(10, 18, "QUAINT"),
        new Level(10, 18, "SCREWS"),
        new Level(10, 18, "SPONGY"),
        new Level(10, 18, "DRAMAS"),
        new Level(10, 18, "WOBBLE"),
        new Level(10, 18, "DINGHY"),
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

    public Dictionary<string, string> m_Definitions = new Dictionary<string, string>() 
    {
        {"TEST", "DEFINITION"},
    };
}

