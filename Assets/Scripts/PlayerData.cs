using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    // update this when the contents change. You'll also need to add an upgrade process in the SessionManager
    public const int sd_CurrentVersion = 4;

    public int sd_Version = sd_CurrentVersion;
    public int sd_PuzzlesSolved = 0;
    public int sd_CorrectSubmits = 0;
    public int sd_IncorrectSubmits = 0;
    public bool[] sd_WordFound;
    public int[] sd_WordFoundCounts;
    public int sd_TotalScore = 0;
    public int sd_RandomSeed = 12345;
    public int sd_CurrentLevel = 0;

    // represents which levels have been completed and which was the best word found
    [Serializable]
    public class LevelCompleteData
    {
        public int m_Zone;
        public int m_Level;
        public string m_BestWord;
        public int m_BestScore;
    };
    public List<LevelCompleteData> sd_LevelsComplete;

    public void AddCoins(int _Value)
    {
        sd_TotalScore += _Value;

        // look for an object called TotalCoins and update it
        TotalCoins Coins = GameObject.Find("TotalCoins").GetComponent<TotalCoins>();
        if (Coins != null)
        {
            Coins.UpdateCoins();
        }
    }

    public int FindLevelComplete(int _Zone, int _Level)
    {
        for (int i = 0; i < sd_LevelsComplete.Count;i++)
        {
            if (sd_LevelsComplete[i].m_Zone == _Zone && sd_LevelsComplete[i].m_Level == _Level)
                return i;
        }

        return -1;
    }

    // use the Index returned from FindLevelComplete
    public string GetLevelCompleteWord(int _Index)
    {
        return sd_LevelsComplete[_Index].m_BestWord;
    }

    // use the Index returned from FindLevelComplete
    public int GetLevelCompleteScore(int _Index)
    {
        return sd_LevelsComplete[_Index].m_BestScore;
    }

    public void LevelComplete(int _Zone, int _Level, string _Word, int _Score)
    {
        // does the level data already exist
        int Index = FindLevelComplete(_Zone, _Level);
        if (Index != -1)
        {
            // just update it
            if (sd_LevelsComplete[Index].m_BestScore < _Score)
                sd_LevelsComplete[Index].m_BestScore = _Score;
            if (sd_LevelsComplete[Index].m_BestWord.Length < _Word.Length)
                sd_LevelsComplete[Index].m_BestWord = _Word;
        }
        else
        {
            // create a new one
            LevelCompleteData Data = new LevelCompleteData();
            Data.m_Zone = _Zone;
            Data.m_Level = _Level;
            Data.m_BestWord = _Word;
            Data.m_BestScore = _Score;

            sd_LevelsComplete.Add(Data);
        }

        sd_PuzzlesSolved++;
        AddCoins(_Score);
    }
}
