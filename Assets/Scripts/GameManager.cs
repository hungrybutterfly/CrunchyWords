using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour {

    public MyButton Button;
    public Transform LetterParent;
    [HideInInspector] public int MaxLetters;

    public Word Word;
    public Transform WordParent;
    public int WordColumns;
    public int WordRows;
    [HideInInspector] public int MaxWords;

    public DictionaryManager Dictionary;

    int LettersUsedIndex;                   // current letter being used
    MyButton[] LetterList;
    int[] LetterUsedList;                   // indexes into the LetterList of used letters

    float LetterSpacing;
    float LetterWidth;

    Word[] WordList;

    // current max length word being used
    MaxWord CurrentWord;

    void SetASize(this RectTransform trans, Vector2 newSize)
    {
        Vector2 oldSize = trans.rect.size;
        Vector2 deltaSize = newSize - oldSize;
        trans.offsetMin = trans.offsetMin - new Vector2(deltaSize.x * trans.pivot.x, deltaSize.y * trans.pivot.y);
        trans.offsetMax = trans.offsetMax + new Vector2(deltaSize.x * (1f - trans.pivot.x), deltaSize.y * (1f - trans.pivot.y));
    }

	void Start () 
    {
        Dictionary.Init();
        MaxLetters = Dictionary.MaxWordSize;

        LetterSpacing = 640 / MaxLetters;
        LetterWidth = LetterSpacing - 5;

        LettersUsedIndex = 0;

        // create the letter buttons
        LetterList = new MyButton[MaxLetters];
        LetterUsedList = new int[MaxLetters];
        for (int i = 0; i < MaxLetters; i++)
        {
            LetterList[i] = Instantiate(Button, new Vector3(0, 0, 0), Quaternion.identity) as MyButton;
            LetterList[i].GetComponent<RectTransform>().sizeDelta = new Vector2(LetterWidth, 150);
            LetterList[i].transform.SetParent(LetterParent, false);
            LetterList[i].SetIndex(i);
        }

        // create the word panels
        MaxWords = WordColumns * WordRows;
        WordList = new Word[MaxWords];
        float WordWidth = 640 / WordColumns;
        float WordHeight = 400 / WordRows;
        for (int i = 0; i < MaxWords; i++)
        {
            float x = (i / WordRows) * WordWidth + (WordWidth / 2);
            float y = 955 - ((i % WordRows) * WordHeight) - (WordHeight / 2);
            WordList[i] = Instantiate(Word, new Vector3(x, y, 0), Quaternion.identity) as Word;
            SetASize(WordList[i].GetComponent<RectTransform>(), new Vector2(WordWidth, WordHeight));
            WordList[i].transform.SetParent(WordParent, false);
        }

        // start the first new word off
        NewWord();
    }
	
	void Update () 
    {
	}

    // get the scren position for a given letter when idle
    public Vector3 GetLetterIdlePosition(int Index)
    {
        return new Vector3(Index * LetterSpacing + LetterSpacing / 2, 300, 0f);
    }

    // get the scren position for a given letter when used
    public Vector3 GetLetterUsedPosition(int Index)
    {
        return new Vector3(Index * LetterSpacing + LetterSpacing / 2, 465, 0f);
    }

    // add a letter to the word
    void AddLetter(int TheButton)
    {
        LetterList[TheButton].SetUsed(LettersUsedIndex);
        LetterUsedList[LettersUsedIndex] = TheButton;
        LettersUsedIndex++;
    }

    // remove the last letter from the word
    public void RemoveLastLetter()
    {
        // are there any letters to remove
        if (LettersUsedIndex != 0)
        {
            LetterList[LetterUsedList[LettersUsedIndex - 1]].SetUnused();
            LettersUsedIndex--;
        }
    }

    // clear all letters from the word
    void ClearUsedLetters()
    {
        for (int i = 0; i < LettersUsedIndex; i++)
        {
            LetterList[LetterUsedList[i]].SetUnused();
        }
        LettersUsedIndex = 0;
    }

    void NewWord()
    {
        ClearUsedLetters();

        CurrentWord = Dictionary.GetRandomWord();

        // update the letters
        for(int i = 0;i < Dictionary.MaxWordSize;i++)
        {
            LetterList[i].GetComponentInChildren<Text>().text = CurrentWord.Word.Substring(i, 1);
        }

        // update the words
        for (int i = 0; i < MaxWords; i++)
        {
            if (i < CurrentWord.FitWords.Length)
                WordList[i].SetIdle(CurrentWord.FitWords[i].Substring(0, 1));
            else
                WordList[i].SetHidden();
        }
    }

    public void SubmitWord()
    {
        // add all the letters to make the word
        string Word = "";
        for (int i = 0; i < LettersUsedIndex; i++)
        {
            Word += CurrentWord.Word.Substring(LetterUsedList[i], 1);
        }

        // look for the word in the possible words
        for (int i = 0; i < CurrentWord.FitWords.Length;i++)
        {
            if (!WordList[i].IsFound() && Word == CurrentWord.FitWords[i])
            {
                // mark the word as found
                WordList[i].SetFound(Word);
                break;
            }
        }

        ClearUsedLetters();
    }

    public void LetterClicked(int TheButton)
    {
        // if the letter clicked isn't used add it to the word otherwise submit the word
        if (!LetterList[TheButton].IsUsed())
            AddLetter(TheButton);
        else
            RemoveLastLetter();
    }

    public void BackupClicked()
    {
        RemoveLastLetter();
    }

    public void JumbleClicked()
    {
    }

    public void ClearClicked()
    {
        ClearUsedLetters();
    }

    public void NewClicked()
    {
        NewWord();
    }
}
