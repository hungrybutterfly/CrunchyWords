using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MaxWord
{
    public string Word;                   // index into the Words array for this max word
    public string[] FitWords;             // array of words that fit in the max word
}

public class DictionaryManager : MonoBehaviour 
{
    public string DictionaryFile;
    public string DictionaryRatingsFile;
    public int MaxWordSize;
    public int MinWordSize;
    public int MaxRating;
    public int MaxWordsRequired;
    public int MinWordsRequired;

    string[] Words;

    // array of first-letter indexes for fast searching
    int[] FirstLetterIndex;

    public class MaxWordIndices
    {
        public int Index;                   // index into the Words array for this max word
        public int[] Words;                 // array of indexs of words that fit into this max word
    }
    MaxWordIndices[] MaxWords;

    // load the dictionary.txt file and create a list of strings
    void LoadFile()
    {
        List<string> TempWords = new List<string>();

        TextAsset bindata = Resources.Load(DictionaryFile, typeof(TextAsset)) as TextAsset;
        string[] SplitString = {"\r\n"};
        string[] OriginalWords = bindata.text.Split(SplitString, System.StringSplitOptions.None);
        bindata = Resources.Load(DictionaryRatingsFile) as TextAsset;
        string[] Ratings = bindata.text.Split(SplitString, System.StringSplitOptions.None);
        for(int i =0;i < OriginalWords.Length;i++)
        {
            // is this word small enough
            if (OriginalWords[i].Length >= MinWordSize && OriginalWords[i].Length <= MaxWordSize)
            {
                // does this word meet the rating requirement
                int Rating = int.Parse(Ratings[i]);
                if (Rating <= MaxRating)
                {
                    TempWords.Add(OriginalWords[i]);
                }
            }
        }

        Words = TempWords.ToArray();

        // find all the indices to the first word of each letter of the alphabet
        FirstLetterIndex = new int[27];
        int Letter = 0;
        string OldLetter = "";
        for(int i = 0;i < Words.Length;i++)
        {
            // has the first letter change
            if (OldLetter != Words[i].Substring(0, 1))
            {
                FirstLetterIndex[Letter] = i;
                OldLetter = Words[i].Substring(0, 1);
                Letter++;
            }
        }
        FirstLetterIndex[Letter] = Words.Length;
    }

    // does the letters of Word fit into the letters of MaxWord
    bool DoesWordFitIntoMaxWord(int MaxWordIndex, int WordIndex)
    {
        bool[] LetterUsed = new bool[MaxWordSize];
        for (int i = 0; i < MaxWordSize; i++)
            LetterUsed[i] = false;

        string MaxWord = Words[MaxWordIndex];
        string Word = Words[WordIndex];

        // run through all the letters in the word
        for (int i = 0; i < Word.Length; i++)
        {
            // look for an unused letter in the max word
            int j = 0;
            for (; j < MaxWordSize; j++)
            {
                if (!LetterUsed[j] && Word[i] == MaxWord[j])
                {
                    LetterUsed[j] = true;
                    break;
                }
            }

            // was an unused letter not found in the max word
            if (j == MaxWordSize)
                return false;
        }

        // if all letters passed then we have a match
        return true;
    }

    // create and array of indexs into the Words array of words that fit into the MaxWord
    int[] FindWordsInMaxWord(int MaxWordIndex)
    {
        List<int> TempWords = new List<int>();

        // run through each letter of the alphabet
        for (int i = 0; i < 26; i++)
        {
            // find the range of indices
            int First = FirstLetterIndex[i];
            int Last = FirstLetterIndex[i + 1];

            // get the first letter
            string Letter = Words[First].Substring(0, 1);

            // check if this letter fits in the max word
            if (Words[MaxWordIndex].Contains(Letter))
            {
                // find all the words that fit into the max word
                for (int j = First; j < Last; j++)
                {
                    // run through each letter of this new word and see if it can fit into the max word
                    if (DoesWordFitIntoMaxWord(MaxWordIndex, j))
                        TempWords.Add(j);
                }
            }
        }

        return TempWords.ToArray();
    }

    // make an array of the max length words in the Words array
    void FindMaxWords()
    {
        List<MaxWordIndices> TempMaxWords = new List<MaxWordIndices>();

        // go through all the words
        for (int i = 0; i < Words.Length; i++)
        {
            // is this the right length word
            if (Words[i].Length == MaxWordSize)
            {
                // create a new max word
                MaxWordIndices Word = new MaxWordIndices();
                Word.Index = i;
                Word.Words = FindWordsInMaxWord(i);

                // does the word list meet the min/max requirements
                if (Word.Words.Length >= MinWordsRequired && Word.Words.Length <= MaxWordsRequired)
                    TempMaxWords.Add(Word);
            }
        }

        MaxWords = TempMaxWords.ToArray();
    }

    // get a random max length word and the words that fit into it
    public MaxWord GetRandomWord()
    {
        MaxWord Word = new MaxWord();

        int MaxWordIndex = Random.Range(0, MaxWords.Length - 1);

        // turn the index into a string
        int Index = MaxWords[MaxWordIndex].Index;
        Word.Word = Words[Index];

        // turn the fit word indices into strings
        int Length = MaxWords[MaxWordIndex].Words.Length;
        Word.FitWords = new string[Length];
        for (int i = 0; i < Length; i++)
        {
            Index = MaxWords[MaxWordIndex].Words[i];
            Word.FitWords[i] = Words[Index];
        }

        return Word;
    }

    public void Init() 
    {
        // load the data file
        LoadFile();

        // look for the max words
        FindMaxWords();
    }
	
	void Update () 
    {	
	}
}
