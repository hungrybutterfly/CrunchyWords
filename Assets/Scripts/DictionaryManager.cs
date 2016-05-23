using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;

public class MaxWord
{
	// Actual word itself in string form
	public string Word;
	// array of words that fit in the max word
	public string[] FitWords;
    // array of indexs into the Words array
    public int[] FitWordsIndex;
}

public class DictionaryManager : MonoBehaviour
{
	public string m_DictionaryFileName;
	public int m_MaxWordSize;
	public int m_MinWordSize;
	public int m_MaxRating;
	public int m_MaxWordsRequired;
	public int m_MinWordsRequired;

	public string[] m_Words;

	// array of first-letter indexes for fast searching
	int[] m_FirstLetterIndex;

    [Serializable]
    public class MaxWordIndices
	{
		public int Index;
		// index into the Words array for this max word
		public int[] Words;
		// array of indexs of words that fit into this max word
	}

    // some dictionary stats for Gary
    int m_OriginalWordCount;
    public int m_FinalWordCount;
    int m_OriginalMaxWordCount;
    int m_FinalMaxWordCount;
    int m_FinalMaxWordPlurals;
    int m_FinalMaxWordNonPlurals;

    // information used for the stats screen
    public int[] m_FinalLetterWords;

	MaxWordIndices[] m_MaxWords;

    List<List<string>> m_Levels;

    string m_MaxWordsFilename = "MaxWords.dat";

	// load the dictionary.txt file and create a list of strings
	void LoadFile ()
	{
		List<string> TempWords = new List<string> ();

		#if (UNITY_IOS || UNITY_EDITOR_OSX)

		TextAsset bindata = Resources.Load(m_DictionaryFileName) as TextAsset;
		string Text = bindata.ToString ();
		int StartIndex = 0;
		int Length;
		do {
			int EndIndex = Text.IndexOf ('\n', StartIndex);
			Length = EndIndex - StartIndex;
			if (Length > 0) 
			{
				// is this word small enough
				if (Length >= m_MinWordSize && Length <= m_MaxWordSize) 
				{
					// add it to the list
					string NewText = Text.Substring (StartIndex, Length);
					TempWords.Add (NewText);
				}
				StartIndex += Length + 1;
			}
		} while (Length > 0);

		#else //PC, Android etc.

		TextAsset bindata = Resources.Load (m_DictionaryFileName, typeof(TextAsset)) as TextAsset;
		string[] SplitString = { "\r\n" };
		string[] OriginalWords = bindata.text.Split (SplitString, System.StringSplitOptions.None);

		for (int i = 0; i < OriginalWords.Length; i++) 
        {
			// is this word small enough
			if (OriginalWords [i].Length >= m_MinWordSize && OriginalWords [i].Length <= m_MaxWordSize) 
            {
				TempWords.Add (OriginalWords [i]);
			}
		}
		#endif

        m_Words = TempWords.ToArray();

		// find all the indices to the first word of each letter of the alphabet
		m_FirstLetterIndex = new int[27];
		int Letter = 0;
		string OldLetter = "";
        for (int i = 0; i < m_Words.Length; i++)
        {
			// has the first letter change
            if (OldLetter != m_Words[i].Substring(0, 1))
            {
				m_FirstLetterIndex [Letter] = i;
                OldLetter = m_Words[i].Substring(0, 1);
				Letter++;
			}
		}
        m_FirstLetterIndex[Letter] = m_Words.Length;
	}

	// does the letters of Word fit into the letters of MaxWord
	bool DoesWordFitIntoMaxWord (int _MaxWordIndex, int _WordIndex)
	{
		bool[] LetterUsed = new bool[m_MaxWordSize];
		for (int i = 0; i < m_MaxWordSize; i++)
			LetterUsed [i] = false;

        string MaxWord = m_Words[_MaxWordIndex];
        string Word = m_Words[_WordIndex];

		// run through all the letters in the word
		for (int i = 0; i < Word.Length; i++) {
			// look for an unused letter in the max word
			int j = 0;
			for (; j < m_MaxWordSize; j++) {
				if (!LetterUsed [j] && Word [i] == MaxWord [j]) {
					LetterUsed [j] = true;
					break;
				}
			}

			// was an unused letter not found in the max word
			if (j == m_MaxWordSize)
				return false;
		}

		// if all letters passed then we have a match
		return true;
	}

	// create and array of indexs into the Words array of words that fit into the MaxWord
	int[] FindWordsInMaxWord (int _MaxWordIndex)
	{
		List<int> TempWords = new List<int> ();

		// run through each letter of the alphabet
		for (int i = 0; i < 26; i++) {
			// find the range of indices
			int First = m_FirstLetterIndex [i];
			int Last = m_FirstLetterIndex [i + 1];

			// get the first letter
            string Letter = m_Words[First].Substring(0, 1);

			// check if this letter fits in the max word
            if (m_Words[_MaxWordIndex].Contains(Letter))
            {
				// find all the words that fit into the max word
				for (int j = First; j < Last; j++) {
					// run through each letter of this new word and see if it can fit into the max word
					if (DoesWordFitIntoMaxWord (_MaxWordIndex, j))
						TempWords.Add (j);
				}
			}
		}

		return TempWords.ToArray ();
	}

	// make an array of the max length words in the Words array
	void FindMaxWords ()
	{
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        if (Session.m_GenerateMaxWords)
        {
            List<MaxWordIndices> TempMaxWords = new List<MaxWordIndices>();

            // go through all the words
            for (int i = 0; i < m_Words.Length; i++)
            {
                // is this the right length word
                if (m_Words[i].Length == m_MaxWordSize)
                {
                    // create a new max word
                    MaxWordIndices Word = new MaxWordIndices();
                    Word.Index = i;
                    Word.Words = FindWordsInMaxWord(i);

                    // does the word list meet the min/max requirements
                    if (Word.Words.Length >= m_MinWordsRequired && Word.Words.Length <= m_MaxWordsRequired)
                        TempMaxWords.Add(Word);
                }
            }

            m_MaxWords = TempMaxWords.ToArray();

            // save the list out
            FileStream file = File.Create(Application.persistentDataPath + "/" + m_MaxWordsFilename);
            file.Close();
            Debug.Log("Dictionary CREATED -> " + Application.persistentDataPath + "/" + m_MaxWordsFilename);

            BinaryFormatter bf = new BinaryFormatter();
            file = File.Open(Application.persistentDataPath + "/" + m_MaxWordsFilename, FileMode.Open);
            bf.Serialize(file, m_MaxWords);
            file.Close();
            Debug.Log("Dictionary SAVED -> " + Application.persistentDataPath + "/" + m_MaxWordsFilename);
        }
        else
        {
            // load the list in
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file;
            if (BitConverter.IsLittleEndian)
                file = File.Open("Assets/Resources/MaxWordsLittle.dat", FileMode.Open);
            else
                file = File.Open("Assets/Resources/MaxWordsBig.dat" + m_MaxWordsFilename, FileMode.Open);
            m_MaxWords = (MaxWordIndices[]) bf.Deserialize(file);
            file.Close();
        }
	}

	// get the max word for the current zone/level
    public MaxWord GetLevelWord()
    {
        MaxWord Word = new MaxWord();

        // work out how many words we need based on the current level
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        // set the random seed based on current zone/level
        int Zone = Session.m_CurrentZone;
        int Level = Session.m_CurrentLevel;

        LevelData Data = GameObject.Find("SessionManager").GetComponent<LevelData>();
        string MaxWord = Data.m_Zones[Zone].m_Levels[Level].m_Word;

        // get the precomputed level index
//        string MaxWord = m_Levels[Zone][Level];
        int MaxWordIndex = 0;
        for (; MaxWordIndex < m_MaxWords.Length; MaxWordIndex++)
        {
            if (MaxWord == m_Words[m_MaxWords[MaxWordIndex].Index])
                break;
        }

        // did we find the max word
        if (MaxWordIndex < m_MaxWords.Length)
        {
            // turn the index into a string
            int Index = m_MaxWords[MaxWordIndex].Index;
            Word.Word = m_Words[Index];

            // turn the fit word indices into strings
            int Length = m_MaxWords[MaxWordIndex].Words.Length;
            Word.FitWords = new string[Length];
            Word.FitWordsIndex = new int[Length];
            for (int i = 0; i < Length; i++)
            {
                Index = m_MaxWords[MaxWordIndex].Words[i];
                Word.FitWords[i] = m_Words[Index];
                Word.FitWordsIndex[i] = Index;
            }
        }
        else
        {
            // the word couldn't be found.
            // need to do something here
        }

        return Word;
    }

	// get a random max length word and the words that fit into it
	public string GetRandomWord ()
	{
//		MaxWord Word = new MaxWord ();

        // work out how many words we need based on the current level
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        // set the random seed based on current zone/level
        int Zone = Session.m_CurrentZone;
        int Level = Session.m_CurrentLevel;

        // fetch the min/max words
        LevelData Data = GameObject.Find("SessionManager").GetComponent<LevelData>();
        int MinWords = Data.m_Zones[Zone].m_Levels[Level].m_MinWordCount;
        int MaxWords = Data.m_Zones[Zone].m_Levels[Level].m_MaxWordCount;

        // look for a random word until we find one within the min-max range
        int MaxWordIndex = 0;
        bool Again = false;
        int Attempts = 0;
        do
        {
            Again = false;
            MaxWordIndex = UnityEngine.Random.Range(0, m_MaxWords.Length - 1);

            // make sure the chosen word is within min-max
            int Count = m_MaxWords[MaxWordIndex].Words.Length;
            if (Count < MinWords || Count > MaxWords)
                Again = true;

            // make sure we don't choose forever
            Attempts++;
            if (Attempts == 1000)
                Again = false;

        } while (Again);

        return m_Words[m_MaxWords[MaxWordIndex].Index];
	}

    public void CalcStats()
    {
        // calc some figures for Gary
        m_OriginalWordCount = m_Words.Length;

        // get a count of all the finals words used
        m_FinalWordCount = 0;
        bool[] WordUsed = new bool[m_Words.Length];
        for (int i = 0; i < m_Words.Length; i++)
            WordUsed[i] = false;
        for (int i = 0; i < m_MaxWords.Length; i++)
        {
            for (int j = 0; j < m_MaxWords[i].Words.Length; j++)
            {
                int Index = m_MaxWords[i].Words[j];
                if (!WordUsed[Index])
                {
                    WordUsed[Index] = true;
                    m_FinalWordCount++;
                }
            }
        }

        // count how many max length words were in the original dictionary
        m_OriginalMaxWordCount = 0;
        for (int i = 0; i < m_Words.Length; i++)
        {
            if (m_Words[i].Length == m_MaxWordSize)
                m_OriginalMaxWordCount++;
        }

        m_FinalMaxWordCount = m_MaxWords.Length;

        // count how many max words are plurals
        m_FinalMaxWordPlurals = 0;
        for (int i = 0; i < m_MaxWords.Length; i++)
        {
            // get the word via the index
            int Index = m_MaxWords[i].Index;
            char LastLetter = m_Words[Index][m_MaxWordSize - 1];
            // is the last letter of this word an 'S'
            if (LastLetter == 'S')
                m_FinalMaxWordPlurals++;
        }

        m_FinalMaxWordNonPlurals = m_FinalMaxWordCount - m_FinalMaxWordPlurals;

        // work out how words were found for each letter
        m_FinalLetterWords = new int[26];
        for (int i = 0; i < 26; i++)
            m_FinalLetterWords[i] = 0;
        for (int i = 0; i < m_Words.Length; i++)
        { 
            if (WordUsed[i])
            {
                int Letter = System.Convert.ToInt32(m_Words[i][0]) - 65;
                m_FinalLetterWords[Letter]++;
            }
        }
    }

    void PrintStats()
    {
        for(int i = 0;i < m_MaxWords.Length;i++)
        {
            int Index = m_MaxWords[i].Index;
            Debug.Log(m_Words[Index] + " " + m_MaxWords[i].Words.Length + "\n");
        }
    }

    bool IsWordUsed(string NewWord, int MaxZone, int MaxLevel)
    {
        // find if a given word already exists in the level data
        LevelData Data = GameObject.Find("SessionManager").GetComponent<LevelData>();
        for (int i = 0; i < Data.m_Zones.Length; i++)
        {
            int NumLevels = Data.m_Zones[i].m_Levels.Length;
            for (int j = 0; j < NumLevels; j++)
            {
                if (i == MaxZone && j == MaxLevel)
                    return false;

                if (m_Levels[i][j] == NewWord)
                    return true;
            }
        }

        return false;
    }

    void InitLevels()
    {
        UnityEngine.Random.seed = 123457;

        // pre-plan which words are used for the levels so we don't get duplications.
        // This will just be used initially but I'll need to save this list at some point so it can be
        // hand edited to cope with an ever changing word list.
        m_Levels = new List<List<string>>();

        LevelData Data = GameObject.Find("SessionManager").GetComponent<LevelData>();

        for (int i = 0; i < Data.m_Zones.Length; i++)
        {
            m_Levels.Add(new List<string>());

            int NumLevels = Data.m_Zones[i].m_Levels.Length;
            for (int j = 0; j < NumLevels; j++)
            {
                bool Used;
                string NewWord;
                do
                {
                    NewWord = GetRandomWord();
                    Used = IsWordUsed(NewWord, i, j);
                } while (Used);

                m_Levels[i].Add(NewWord);
            }
        }
    }

	public void Init ()
	{
		// load the data file
		LoadFile ();

		// look for the max words
		FindMaxWords ();

        CalcStats();

        //PrintStats();

//        InitLevels();
	}

	void Update ()
	{	
	}
}
