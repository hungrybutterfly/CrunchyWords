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

    // cost in coins
    public int m_ShuffleCost;
    public int m_CheckWordCost;
    public int m_RevealWordsCost;
    public int m_HintCost;

    int m_LettersUsedIndex;                   // current letter being used
    LetterButton[] m_LetterList;
    int[] m_LetterUsedList;                   // indexes into the m_LetterList of used letters

    float m_LetterSpacing;
    float m_LetterWidth;

    Word[] m_WordList;

    // current max length word being used
    MaxWord m_CurrentWord;

    // used for keeping track of right/wrong words submitted
    int m_WordsWrong;
    int m_WordsRight;
    Text m_ScoreText;

    // used for scoring
    [HideInInspector] public int m_TotalScore;
    int m_WordsRightCombo;
    Text m_ComboText;

    // ceremony timer for showing the WIN text
    int m_ShowWinTimer;
    GameObject m_WinObject;

    // used to provide a hint
    Word m_SelectedWord;

    // button that is enabled when player meets the word quota
    Button m_FinishButton;
    int m_FinishFlashTimer;

    // this will turn on bad word reenforcement in the scoring
    static bool m_BadScore = true;
    int m_BadScoreTimer;                        // timer to flash the score

    private void SetASize(RectTransform _trans, Vector2 _newSize)
    {
        Vector2 oldSize = _trans.rect.size;
        Vector2 deltaSize = _newSize - oldSize;
        _trans.offsetMin = _trans.offsetMin - new Vector2(deltaSize.x * _trans.pivot.x, deltaSize.y * _trans.pivot.y);
        _trans.offsetMax = _trans.offsetMax + new Vector2(deltaSize.x * (1f - _trans.pivot.x), deltaSize.y * (1f - _trans.pivot.y));
    }

	void Start () 
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

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
            m_WordList[i].m_ID = i;
        }

        //Get a reference to the Scores text.
        m_ScoreText = GameObject.Find("Scores").GetComponent<Text>();

        //Hide the win text for now
        m_WinObject = GameObject.Find("End Text");
        m_WinObject.SetActive(false);
        m_ShowWinTimer = 0;

        // no word selected
        m_SelectedWord = null;

        // get some objects
        m_ComboText = GameObject.Find("Combo").GetComponent<Text>();
//        m_FinishButton = GameObject.Find("Finish").GetComponent<Button>();

        // set the total coins text
        Text Coins = GameObject.Find("Total Coins Text").GetComponent<Text>();
        Coins.text = Session.m_SaveData.sd_TotalScore.ToString();

        // set the total coins text
        GameObject Object = GameObject.Find("Jumble Cost");
        Text Value = Object.GetComponentInChildren<Text>();
        Value.text = m_ShuffleCost.ToString();

        Object = GameObject.Find("Check Cost");
        Value = Object.GetComponentInChildren<Text>();
        Value.text = m_CheckWordCost.ToString();

        Object = GameObject.Find("Hint Cost");
        Value = Object.GetComponentInChildren<Text>();
        Value.text = m_HintCost.ToString();

        // start the first new word off
        NewWord();
    }
	
	void Update () 
    {
        // was a timer started showing the WIN text
        if (m_ShowWinTimer != 0)
        {
            m_ShowWinTimer--;
        }
        else
        {
            // flash the finish button if it's just appeared
            if (m_FinishFlashTimer != 0)
            {
                m_FinishFlashTimer--;
                if ((m_FinishFlashTimer & 8) < 4)
                    m_FinishButton.GetComponent<Image>().color = new Color(1, 1, 0);
                else
                    m_FinishButton.GetComponent<Image>().color = new Color(1, 0, 0);
            }

            // flash the m_ComboText if it's been reset
            if (m_BadScoreTimer != 0)
            {
                m_BadScoreTimer--;
                if ((m_BadScoreTimer & 8) < 4)
                    m_ComboText.GetComponent<Text>().color = new Color(1, 1, 1);
                else
                    m_ComboText.GetComponent<Text>().color = new Color(1, 0, 0);
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
    public void NewWord()
    {
        ClearUsedLetters();

        // do we want to use the last word from the previous game
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        if (Session.m_UseLastWord)
        {
            m_CurrentWord = Session.m_LastWord;
            Session.m_UseLastWord = false;
        }
        else
            m_CurrentWord = m_DictionaryObject.GetRandomWord();

        // update the letters
        for (int i = 0; i < m_DictionaryObject.m_MaxWordSize; i++)
        {
			m_LetterList[i].GetComponentInChildren<Text>().text = (m_CurrentWord.Word.Substring(i, 1));
            m_LetterList[i].SetLetterIndex(i);
        }

        // update the words
        for (int i = 0; i < m_MaxWords; i++)
        {
            if (i < m_CurrentWord.FitWords.Length)
            {
                m_WordList[i].SetWord(m_CurrentWord.FitWords[i]);
                m_WordList[i].SetIdle();
            }
            else
                m_WordList[i].SetHidden();
        }

        // jumble it up
        JumbleLetters();

        // clear the score
        m_WordsWrong = 0;
        m_WordsRight = 0;
        m_TotalScore = 0;
        m_WordsRightCombo = 1;
        UpdateScore();

        ResetHint();

        // reset the finish button
        if (m_FinishButton)
        {
            m_FinishButton.GetComponent<Image>().color = new Color(1, 1, 1);
            m_FinishButton.gameObject.SetActive(false);
            m_FinishFlashTimer = 0;
        }
    }

    private void UpdateScore()
    {
        m_ScoreText.text = m_TotalScore.ToString();

        m_ComboText.text = "Combo x" + m_WordsRightCombo.ToString();
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

    private void SubmitWord(bool Check)
    {
		// don't allow a no-letter submission
		if (m_LettersUsedIndex == 0)
			return;
	
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        // add all the letters to make the word
        string Word = "";
        for (int i = 0; i < m_LettersUsedIndex; i++)
        {
            Word += m_CurrentWord.Word.Substring(m_LetterList[m_LetterUsedList[i]].m_LetterIndex, 1);
        }

        // look for the word in the possible words
        bool Right = false;
        bool AlreadyFound = false;
        for (int i = 0; i < m_CurrentWord.FitWords.Length; i++)
        {
            if (Word == m_CurrentWord.FitWords[i])
            {
                // has this word not been found yet
                if (!m_WordList[i].IsFound())
                {
                    // mark the word as found
                    m_WordList[i].SetFound(Word, false);
                    Right = true;

                    // has this word been found in the lifetime stats
                    int Index = m_CurrentWord.FitWordsIndex[i];
                    if (!Session.m_SaveData.sd_WordFound[Index])
                    {
                        // mark the word as found
                        Session.m_SaveData.sd_WordFound[Index] = true;

                        // increase the number of words for that letter
                        int Letter = System.Convert.ToInt32(m_CurrentWord.FitWords[i][0]) - 65;
                        Session.m_SaveData.sd_WordFoundCounts[Letter]++;
                    }
                    break;
                }
                else
                {
                    AlreadyFound = true;
                }
            }
        }

        if (!AlreadyFound)
        {
            // Update the score
            if (Right)
            {
                m_WordsRight++;
                ++Session.m_SaveData.sd_CorrectSubmits;

                // update the score (add word length * combo)
                m_TotalScore += Word.Length * m_WordsRightCombo;

                // increase the combo
                m_WordsRightCombo++;
            }
            else
            {
                if (!Check)
                {
                    m_WordsWrong++;
                    ++Session.m_SaveData.sd_IncorrectSubmits;

                    // reset the combo
                    m_WordsRightCombo = 1;

                    // flash the score for 2 seconds
                    if (m_BadScore)
                        m_BadScoreTimer = 120;
                }
            }
            UpdateScore();
        }

        if (!Check || Right)
            ClearUsedLetters();

        // did the player find all the words
        if (m_WordsRight == m_CurrentWord.FitWords.Length)
        {
            Win();
        }

        //Save the data
        Session.Save();

        // have enough words been revealed to complete the level
        if (m_FinishButton && !m_FinishButton.gameObject.activeSelf && m_WordsRight >= m_CurrentWord.FitWords.Length / 2)
        {
            // enable the finish button
            m_FinishButton.gameObject.SetActive(true);
            m_FinishFlashTimer = 120;
        }

        ResetHint();
    }

    void Win()
    {
        // show the win text
        m_WinObject.SetActive(true);

        // start a count down before going to the result screen
        m_ShowWinTimer = 30;

        //Add to the save data
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        ++Session.m_SaveData.sd_PuzzlesSolved;
        Session.m_SaveData.sd_TotalScore += m_TotalScore;

        // remember these stats to go to the review
        Session.m_LastWordsRight = m_WordsRight;
        Session.m_LastWordsWrong = m_WordsWrong;
        Session.m_LastScore = m_TotalScore;
        Session.m_LastWord = m_CurrentWord;
        Session.m_WordsCompleted = m_WordsRight;
        Session.m_WordsAvailable = m_CurrentWord.FitWords.Length;

        Session.Save();

        // reveal any unfound words
        for (int i = 0; i < m_CurrentWord.FitWords.Length; i++)
        {
            // has this word not been found yet
            if (!m_WordList[i].IsFound())
            {
                // mark the word as found
                m_WordList[i].SetFound(m_CurrentWord.FitWords[i], true);
            }
        }
    }

    void ResetHint()
    {
        // reset the hints button
        Button Hint = GameObject.Find("Hint").GetComponent<Button>();
        Hint.GetComponent<Image>().color = new Color(1, 1, 1);

        // unselect the old word
        if (m_SelectedWord)
        {
            m_SelectedWord.Selected(false);
            m_SelectedWord = null;
        }
    }

    void Hint()
    {
        // has a word been selected
        if (m_SelectedWord)
        {
            // does this word have any hints available
            int WordIndex = m_SelectedWord.m_ID;
            if (!m_SelectedWord.IsHintUsed())
                m_SelectedWord.UseHint();

            ResetHint();
        }
    }

    void SelectWord(Word WordObject)
    {
        // make sure this word isn't found already
        if (!WordObject.IsFound() && !WordObject.IsHintUsed())
        {
            Button Hint = GameObject.Find("Hint").GetComponent<Button>();

            // unselect the old word
            if (m_SelectedWord)
            {
                m_SelectedWord.Selected(false);

                // highlight the hint button
                Hint.GetComponent<Image>().color = new Color(1, 1, 1);
            }

            // is this a new word (or no word)
            if (m_SelectedWord == WordObject)
                WordObject = null;

            m_SelectedWord = WordObject;

            // select the new word
            if (m_SelectedWord)
            {
                m_SelectedWord.Selected(true);

                // unhighlight the hint button
                Hint.GetComponent<Image>().color = new Color(1, 0, 0);
            }
        }
    }

    bool SpendCoins(int Amount)
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        // does the player have enough coins
        if (Session.m_SaveData.sd_TotalScore >= Amount)
        {
            // remove the coins
            Session.m_SaveData.sd_TotalScore -= Amount;

            // update the text
            Text Coins = GameObject.Find("Total Coins Text").GetComponent<Text>();
            Coins.text = Session.m_SaveData.sd_TotalScore.ToString();

            return true;
        }

        return false;
    }

    public void Finish()
    {
        // attempt to spend coins
        if (SpendCoins(m_RevealWordsCost))
        {
            Win();
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
        // attempt to spend coins
        if (SpendCoins(m_ShuffleCost))
        {
            ClearUsedLetters();
            JumbleLetters();
        }
    }

    public void SubmitClicked()
    {
        SubmitWord(false);
    }

    public void CheckClicked()
    {
        // does the player have any letters used
        if (m_LettersUsedIndex != 0)
        {
            // attempt to spend coins
            if (SpendCoins(m_CheckWordCost))
            {
                SubmitWord(true);
            }
        }
    }

    public void HintClicked()
    {
        // attempt to spend coins
        if (SpendCoins(m_HintCost))
        {
            Hint();
        }
    }

    public void PauseClicked()
    {
        PauseManager Pause = GameObject.Find("GameManager").GetComponent<PauseManager>();
        Pause.SetIsEnabled(true);
    }

    public void WordClicked(Word WordObject)
    {
        SelectWord(WordObject);
    }

    public void EndClicked()
    {
        if (m_ShowWinTimer == 0)
        {
            // timer has finished so go to the result screen
            SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
            Session.ChangeScene("Results");
        }
    }

}
