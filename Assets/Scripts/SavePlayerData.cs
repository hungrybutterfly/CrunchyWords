using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using MiniJSON;

[Serializable]
public class JSONPlayerData
{
    public Dictionary<string, object> jssd_Dictionary;
};

[Serializable]
public class SavePlayerData
{
    // update this when the contents change. You'll also need to add an upgrade process in the SessionManager
    public const int sd_CurrentVersion = 4;

    //JSON Save related
    public Dictionary<string, object> sd_Dictionary;

    //Save variables
    public int sd_Version = sd_CurrentVersion;
    public int sd_PuzzlesSolved = 0;
    public int sd_CorrectSubmits = 0;
    public int sd_IncorrectSubmits = 0;
    public int sd_TotalScore = 0;
    public int sd_RandomSeed = 12345;
    public int sd_CurrentLevel = 0;
    public List<string> sd_WordFound;
    [Serializable]
    public class SaveLevelData
    {
        public int m_Zone = 0;
        public int m_Level = 0;
        public string m_BestWord = "";
        public int m_BestScore = 0;
    };
    public List<SaveLevelData> sd_LevelsComplete;

    //Reset and Initialise Save Data
    public void InitSaveData()
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        // clear the scores
        sd_PuzzlesSolved = 0;
        sd_CorrectSubmits = 0;
        sd_IncorrectSubmits = 0;

        // create and clear the arrays
        DictionaryManager Dictionary = GameObject.Find("DictionaryManager").GetComponent<DictionaryManager>();
        int Size = Dictionary.m_Words.Length;
        sd_WordFound = new List<string>();
        sd_WordFound.Clear();
        Size = 26;
        Session.m_WordFoundCounts = new int[Size];
        for (int i = 0; i < Size; i++)
            Session.m_WordFoundCounts[i] = 0;

        sd_TotalScore = Session.m_StartingCoins;
        sd_RandomSeed = 12345;
        sd_CurrentLevel = 0;

        sd_LevelsComplete = new List<SavePlayerData.SaveLevelData>();
    }

    //Take all the savedata and convert it into a Dictionary (Ready for saving)
    public void ConvertToJSON()
    {
        //Serialise the data
        sd_Dictionary = new Dictionary<string, object>();
        //Save Data
        sd_Dictionary.Add("sd_Version", sd_Version);
        sd_Dictionary.Add("sd_PuzzlesSolved", sd_PuzzlesSolved);
        sd_Dictionary.Add("sd_CorrectSubmits", sd_CorrectSubmits);
        sd_Dictionary.Add("sd_IncorrectSubmits", sd_IncorrectSubmits);
        sd_Dictionary.Add("sd_TotalScore", sd_TotalScore);
        sd_Dictionary.Add("sd_RandomSeed", sd_RandomSeed);
        sd_Dictionary.Add("sd_CurrentLevel", sd_CurrentLevel);
        sd_Dictionary.Add("sd_WordFound", sd_WordFound);
        sd_Dictionary.Add("sd_LevelsCompleteAmount", sd_LevelsComplete.Count);
        for (int i = 0; i < sd_LevelsComplete.Count; ++i)
        {
            string buffer = "sd_LevelsComplete_" + "m_Zone_" + i;
            sd_Dictionary.Add(buffer, sd_LevelsComplete[i].m_Zone);
            buffer = "sd_LevelsComplete_" + "m_Level_" + i;
            sd_Dictionary.Add(buffer, sd_LevelsComplete[i].m_Level);
            buffer = "sd_LevelsComplete_" + "m_BestWord_" + i;
            sd_Dictionary.Add(buffer, sd_LevelsComplete[i].m_BestWord);
            buffer = "sd_LevelsComplete_" + "m_BestScore_" + i;
            sd_Dictionary.Add(buffer, sd_LevelsComplete[i].m_BestScore);
        }
    }

    //Take the serialised dictionary, convert it and restore the values (After Loading)
    public void LoadDataFromJSON()
    {
        InitSaveData();

        //Now find it and load each key
        if (sd_Dictionary.ContainsKey("sd_PuzzlesSolved")) { sd_PuzzlesSolved = (int)sd_Dictionary["sd_PuzzlesSolved"]; }
        if (sd_Dictionary.ContainsKey("sd_CorrectSubmits")) { sd_CorrectSubmits = (int)sd_Dictionary["sd_CorrectSubmits"]; }
        if (sd_Dictionary.ContainsKey("sd_IncorrectSubmits")) { sd_IncorrectSubmits = (int)sd_Dictionary["sd_IncorrectSubmits"]; }
        if (sd_Dictionary.ContainsKey("sd_TotalScore")) { sd_TotalScore = (int)sd_Dictionary["sd_TotalScore"]; }
        if (sd_Dictionary.ContainsKey("sd_RandomSeed")) { sd_RandomSeed = (int)sd_Dictionary["sd_RandomSeed"]; }
        if (sd_Dictionary.ContainsKey("sd_CurrentLevel")) { sd_CurrentLevel = (int)sd_Dictionary["sd_CurrentLevel"]; }
        if (sd_Dictionary.ContainsKey("sd_WordFound")) { sd_WordFound = (List<string>)sd_Dictionary["sd_WordFound"]; }
        if (sd_Dictionary.ContainsKey("sd_LevelsCompleteAmount"))
        {
            int count = (int)sd_Dictionary["sd_LevelsCompleteAmount"];
            for (int i = 0; i < count; ++i)
            {
                SaveLevelData Data = new SaveLevelData();
                Data.m_Zone = 0;
                Data.m_Level = 0;
                Data.m_BestWord = "";
                Data.m_BestScore = 0;

                string buffer = "sd_LevelsComplete_" + "m_Zone_" + i;
                if (sd_Dictionary.ContainsKey("buffer")) { Data.m_Zone = (int)sd_Dictionary[buffer]; }
                buffer = "sd_LevelsComplete_" + "m_Level_" + i;
                if (sd_Dictionary.ContainsKey("buffer")) { Data.m_Level = (int)sd_Dictionary[buffer]; }
                buffer = "sd_LevelsComplete_" + "m_BestWord_" + i;
                if (sd_Dictionary.ContainsKey("buffer")) { Data.m_BestWord = (string)sd_Dictionary[buffer]; }
                buffer = "sd_LevelsComplete_" + "m_BestScore_" + i;
                if (sd_Dictionary.ContainsKey("buffer")) { Data.m_BestScore = (int)sd_Dictionary[buffer]; }

                sd_LevelsComplete.Add(Data);
            }
        }
        //Now find the 'start with' alphabet array from words discovered
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.m_WordFoundCounts = new int[26];
        for (int i = 0; i < sd_WordFound.Count; ++i)
        {
            ++Session.m_WordFoundCounts[(Convert.ToInt32((sd_WordFound[0][0] - 65)))];
        }
    }

    //Has the player already entered/discovered this word?
    public bool IsWordAlreadyFound(string _word)
    {
        for (int i = 0; i < sd_WordFound.Count; ++i)
        {
            if (_word.Equals(sd_WordFound[i]))
            {
                return true;
            }
        }
        return false;
    }

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
        for (int i = 0; i < sd_LevelsComplete.Count; i++)
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
            SaveLevelData Data = new SaveLevelData();
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