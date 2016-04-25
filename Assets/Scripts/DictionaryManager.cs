using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MaxWord
{
	// index into the Words array for this max word
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
		List<MaxWordIndices> TempMaxWords = new List<MaxWordIndices> ();

		// go through all the words
        for (int i = 0; i < m_Words.Length; i++)
        {
			// is this the right length word
            if (m_Words[i].Length == m_MaxWordSize)
            {
				// create a new max word
				MaxWordIndices Word = new MaxWordIndices ();
				Word.Index = i;
				Word.Words = FindWordsInMaxWord (i);

				// does the word list meet the min/max requirements
				if (Word.Words.Length >= m_MinWordsRequired && Word.Words.Length <= m_MaxWordsRequired)
					TempMaxWords.Add (Word);
			}
		}

        m_MaxWords = TempMaxWords.ToArray();
	}

	// get a random max length word and the words that fit into it
	public MaxWord GetRandomWord ()
	{
		MaxWord Word = new MaxWord ();

        // work out how many words we need based on the current level
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        // set the random seed based on current zone/level
        int Zone = Session.m_CurrentZone;
        int Level = Session.m_CurrentLevel;
        Random.seed = ((Zone * 20 + Level) + 1) * 1234567;

        // fetch the min/max words
        LevelData Data = GameObject.Find("LevelData").GetComponent<LevelData>();
        int MinWords = Data.m_Zones[Zone].m_Levels[Level].m_MinWordCount;
        int MaxWords = Data.m_Zones[Zone].m_Levels[Level].m_MaxWordCount;

        // look for a random word until we find one within the min-max range
        int MaxWordIndex = 0;
        bool Again = false;
        int Attempts = 0;
        do
        {
            Again = false;
            MaxWordIndex = Random.Range(0, m_MaxWords.Length - 1);

            // make sure the chosen word is within min-max
            int Count = m_MaxWords[MaxWordIndex].Words.Length;
            if (Count < MinWords || Count > MaxWords)
                Again = true;

            // make sure we don't choose forever
            Attempts++;
            if (Attempts == 1000)
                Again = false;

        } while (Again);

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

        return Word;
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

	public void Init ()
	{
		// load the data file
		LoadFile ();

		// look for the max words
		FindMaxWords ();

        CalcStats();
	}

	void Update ()
	{	
	}
}
