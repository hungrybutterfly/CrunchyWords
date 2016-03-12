using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MaxWord
{
	public string Word;
	// index into the Words array for this max word
	public string[] FitWords;
	// array of words that fit in the max word
}

public class DictionaryManager : MonoBehaviour
{
	public string m_DictionaryFileName;
	public int m_MaxWordSize;
	public int m_MinWordSize;
	public int m_MaxRating;
	public int m_MaxWordsRequired;
	public int m_MinWordsRequired;

	string[] m_Words;

	// array of first-letter indexes for fast searching
	int[] m_FirstLetterIndex;

	public class MaxWordIndices
	{
		public int Index;
		// index into the Words array for this max word
		public int[] Words;
		// array of indexs of words that fit into this max word
	}

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

        int MaxWordIndex = Random.Range(0, m_MaxWords.Length - 1);

		// turn the index into a string
        int Index = m_MaxWords[MaxWordIndex].Index;
        Word.Word = m_Words[Index];

		// turn the fit word indices into strings
        int Length = m_MaxWords[MaxWordIndex].Words.Length;
		Word.FitWords = new string[Length];
		for (int i = 0; i < Length; i++) {
            Index = m_MaxWords[MaxWordIndex].Words[i];
            Word.FitWords[i] = m_Words[Index];
		}

		return Word;
	}

	public void Init ()
	{
		// load the data file
		LoadFile ();

		// look for the max words
		FindMaxWords ();
	}

	void Update ()
	{	
	}
}
