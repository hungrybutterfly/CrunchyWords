using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour {

    public LetterButton m_LetterPrefab;
    public Transform m_LetterParent;
    [HideInInspector] public int m_MaxLetters;

    public Word m_WordPrefab;
    public Transform m_WordParent;
    public int m_WordColumns;
    public int m_WordRows;
    [HideInInspector] public int m_MaxWords;

    [HideInInspector] public DictionaryManager m_DictionaryObject;

    int m_LettersUsedIndex;                   // current letter being used
    LetterButton[] m_LetterList;
    int[] m_LetterUsedList;                   // indexes into the m_LetterList of used letters

    float m_LetterSpacing;
    float m_LetterWidth;

    Word[] m_WordList;

    // current max length word being used
    MaxWord m_CurrentWord;

    // used for keeping track of right/wrong words submitted
    int m_ScoreWrong;
    int m_ScoreRight;
    Text m_ScoreText;

    // ceremony timer for showing the WIN text
    int m_ShowWinTimer;
    GameObject m_WinObject;

    private void SetASize(RectTransform _trans, Vector2 _newSize)
    {
        Vector2 oldSize = _trans.rect.size;
        Vector2 deltaSize = _newSize - oldSize;
        _trans.offsetMin = _trans.offsetMin - new Vector2(deltaSize.x * _trans.pivot.x, deltaSize.y * _trans.pivot.y);
        _trans.offsetMax = _trans.offsetMax + new Vector2(deltaSize.x * (1f - _trans.pivot.x), deltaSize.y * (1f - _trans.pivot.y));
    }

	void Start () 
    {
        m_DictionaryObject = GameObject.Find("DictionaryManager").GetComponent<DictionaryManager>();
        m_MaxLetters = m_DictionaryObject.m_MaxWordSize;

        m_LetterSpacing = 640 / m_MaxLetters;
        m_LetterWidth = m_LetterSpacing - 5;

        m_LettersUsedIndex = 0;

        // create the letter buttons
        m_LetterList = new LetterButton[m_MaxLetters];
        m_LetterUsedList = new int[m_MaxLetters];
        for (int i = 0; i < m_MaxLetters; i++)
        {
            m_LetterList[i] = Instantiate(m_LetterPrefab, new Vector3(0, 0, 0), Quaternion.identity) as LetterButton;
            m_LetterList[i].GetComponent<RectTransform>().sizeDelta = new Vector2(m_LetterWidth, 150);
            m_LetterList[i].transform.SetParent(m_LetterParent, false);
            m_LetterList[i].SetIndex(i);
        }

        // create the word panels
        m_MaxWords = m_WordColumns * m_WordRows;
        m_WordList = new Word[m_MaxWords];
        float WordWidth = 640 / m_WordColumns;
        float WordHeight = 400 / m_WordRows;
        for (int i = 0; i < m_MaxWords; i++)
        {
            float x = (i / m_WordRows) * WordWidth + (WordWidth / 2);
            float y = 955 - ((i % m_WordRows) * WordHeight) - (WordHeight / 2);
            m_WordList[i] = Instantiate(m_WordPrefab, new Vector3(x, y, 0), Quaternion.identity) as Word;
            SetASize(m_WordList[i].GetComponent<RectTransform>(), new Vector2(WordWidth, WordHeight));
            m_WordList[i].transform.SetParent(m_WordParent, false);
        }

        //Get a reference to the Scores text.
        m_ScoreText = GameObject.Find("Scores").GetComponent<Text>();

        //Hide the win text for now
        m_WinObject = GameObject.Find("Result");
        m_WinObject.SetActive(false);
        m_ShowWinTimer = 0;

        // start the first new word off
        NewWord();
    }
	
	void Update () 
    {
        // was a timer started showing the WIN text
        if (m_ShowWinTimer != 0)
        {
            m_ShowWinTimer--;
            if (m_ShowWinTimer == 0)
            {
                // timer has finished so go to the result screen
                SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
                Session.ChangeScene("Results");
            }
        }
	}

    // get the scren position for a given letter when idle
    public Vector3 GetLetterIdlePosition(int _Index)
    {
        return new Vector3(_Index * m_LetterSpacing + m_LetterSpacing / 2, 300, 0f);
    }

    // get the scren position for a given letter when used
    public Vector3 GetLetterUsedPosition(int _Index)
    {
        return new Vector3(_Index * m_LetterSpacing + m_LetterSpacing / 2, 465, 0f);
    }

    // change the state of the Words to show which are potential candidates for the currently used letters
    private void UpdatePossibleWords()
    {
        if (m_CurrentWord == null)
            return;

        // go through all the non-found words words
        for (int i = 0; i < m_CurrentWord.FitWords.Length; i++)
        {
            if (!m_WordList[i].IsFound())
            {
                if (m_LettersUsedIndex == 0)
                    m_WordList[i].SetIdle();
                else
                {
                    // get the first used letter
                    int Index = m_LetterUsedList[0];
                    int LetterIndex = m_LetterList[Index].m_LetterIndex;
                    string FirstLetter = m_CurrentWord.Word.Substring(LetterIndex, 1);

                    // is the first letter of the word the same as the first letter the player has used
                    // and is the possible word length less than the known word
                    if (FirstLetter == m_CurrentWord.FitWords[i].Substring(0, 1) && m_LettersUsedIndex <= m_CurrentWord.FitWords[i].Length)
                    {
                        m_WordList[i].SetPossible();
                    }
                    else
                    {
                        m_WordList[i].SetIdle();
                    }
                }
            }
        }
    }

    // add a letter to the word
    private void AddLetter(int _TheButton)
    {
        m_LetterList[_TheButton].SetUsed(m_LettersUsedIndex);
        m_LetterUsedList[m_LettersUsedIndex] = _TheButton;
        m_LettersUsedIndex++;

        UpdatePossibleWords();
    }

    // remove the last letter from the word
    private void RemoveLastLetter()
    {
        // are there any letters to remove
        if (m_LettersUsedIndex != 0)
        {
            m_LetterList[m_LetterUsedList[m_LettersUsedIndex - 1]].SetUnused();
            m_LettersUsedIndex--;
        }

        UpdatePossibleWords();
    }

    // clear all letters from the word
    private void ClearUsedLetters()
    {
        for (int i = 0; i < m_LettersUsedIndex; i++)
        {
            m_LetterList[m_LetterUsedList[i]].SetUnused();
        }
        m_LettersUsedIndex = 0;

        UpdatePossibleWords();
    }

    // pick out a new random max length words and set up the letters ready for play
    private void NewWord()
    {
        ClearUsedLetters();

        m_CurrentWord = m_DictionaryObject.GetRandomWord();

        // update the letters
        for (int i = 0; i < m_DictionaryObject.m_MaxWordSize; i++)
        {
            m_LetterList[i].GetComponentInChildren<Text>().text = m_CurrentWord.Word.Substring(i, 1);
            m_LetterList[i].SetLetterIndex(i);
        }

        // update the words
        for (int i = 0; i < m_MaxWords; i++)
        {
            if (i < m_CurrentWord.FitWords.Length)
            {
                string String = m_CurrentWord.FitWords[i].Substring(0, 1);
                for (int j = 0; j < m_CurrentWord.FitWords[i].Length - 1; j++)
                    String += "_ ";
                m_WordList[i].SetIdle(String);
            }
            else
                m_WordList[i].SetHidden();
        }

        // jumble it up
        JumbleLetters();

        // clear the score
        m_ScoreWrong = 0;
        m_ScoreRight = 0;
        UpdateScore();
    }

    private void UpdateScore()
    {
        m_ScoreText.text = m_ScoreRight.ToString() + "/" + m_ScoreWrong.ToString();
    }

    private void JumbleLetters()
    {
        // make a temp copy of the existing letters
        LetterButton[] TempLetters = new LetterButton[m_MaxLetters];
        for (int i = 0; i < m_MaxLetters; i++)
        {
            TempLetters[i] = m_LetterList[i];
            m_LetterList[i] = null;
        }

        // reassign the letters back in a jumbled order
        for (int i = 0; i < m_MaxLetters; i++)
        {
            // find a random, unused letter slot
            int Index = 0;
            do
            {
                Index = (int)(Random.value * m_MaxLetters);
            } while (m_LetterList[Index]);

            // assign and update the position
            m_LetterList[Index] = TempLetters[i];
            m_LetterList[Index].SetIndex(Index);
        }
    }

    private void SubmitWord()
    {
        // add all the letters to make the word
        string Word = "";
        for (int i = 0; i < m_LettersUsedIndex; i++)
        {
            Word += m_CurrentWord.Word.Substring(m_LetterList[m_LetterUsedList[i]].m_LetterIndex, 1);
        }

        // look for the word in the possible words
        bool Right = false;
        for (int i = 0; i < m_CurrentWord.FitWords.Length;i++)
        {
            if (!m_WordList[i].IsFound() && Word == m_CurrentWord.FitWords[i])
            {
                // mark the word as found
                m_WordList[i].SetFound(Word);
                Right = true;
                break;
            }
        }

        // update the score
        if (Right)
            m_ScoreRight++;
        else
            m_ScoreWrong++;
        UpdateScore();

        ClearUsedLetters();

        // did the player find all the words
        if (m_ScoreRight == m_CurrentWord.FitWords.Length)
        {
            // show the win text
            m_WinObject.SetActive(true);

            // start a count down before going to the result screen
            m_ShowWinTimer = 120;
        }
    }

    // these 'Clicked' functions are called when the appropriate button is clicked
    public void LetterClicked(int _TheButton)
    {
        // if the letter clicked isn't used add it to the word otherwise submit the word
        if (!m_LetterList[_TheButton].IsUsed())
            AddLetter(_TheButton);
        else
            RemoveLastLetter();
    }

    public void BackupClicked()
    {
        RemoveLastLetter();
    }

    public void JumbleClicked()
    {
        ClearUsedLetters();
        JumbleLetters();
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
