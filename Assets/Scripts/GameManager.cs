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

    // starting cost in coins
    public int m_StartShuffleCost;
    public int m_StartCheckWordCost;
    public int m_StartLockCost;
    public int m_StartHintCost;
    public int m_StartRevealWordsCost;

    int m_ShuffleCost;
    int m_CheckWordCost;
    int m_LockCost;
    int m_HintCost;
    [HideInInspector]
    public int m_RevealWordsCost;

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
    int m_BestPossibleScore;                // to determin if player for a perfect score
    int m_WordsRightCombo;
    Text m_ComboText;
    int m_BestChain;

    // ceremony timer for showing the WIN text
    int m_ShowWinTimer;

    // used to provide a hint
    Word m_SelectedWord;
    bool m_HintReady;

    // button that is enabled when player meets the word quota
    Button m_FinishButton;
    int m_FinishFlashTimer;

    // this will turn on bad word reenforcement in the scoring
    static bool m_BadScore = true;
    int m_BadScoreTimer;                        // timer to flash the score

    // pick a random word when hint is used rather than the selected word
    public bool m_RandomHint = false;

    // variables to be used after a ceremony is complete
    Word m_CorrectWord;
    string m_WordFound;

    // is the chain locked (so it won't break with a bad word)
    bool m_Locked;
    GameObject m_LockImage;

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
            float y = 864 - ((i % m_WordRows) * WordHeight) - (WordHeight / 2);
            m_WordList[i] = Instantiate(m_WordPrefab, new Vector3(x, y, 0), Quaternion.identity) as Word;
            SetASize(m_WordList[i].GetComponent<RectTransform>(), new Vector2(WordWidth, WordHeight));
            m_WordList[i].transform.SetParent(m_WordParent, false);
            m_WordList[i].m_ID = i;
        }

        //Get a reference to the Coins text.
        m_ScoreText = GameObject.Find("CoinCounter").GetComponentInChildren<Text>();

        //Hide the win text for now
        m_ShowWinTimer = 0;

        // no word selected
        m_SelectedWord = null;
        m_HintReady = false;

        // reset the lock feature
        m_Locked = false;
        m_LockImage = GameObject.Find("LockImage");
        m_LockImage.SetActive(false);

        // get some objects
        m_ComboText = GameObject.Find("Combo").GetComponent<Text>();
//        m_FinishButton = GameObject.Find("Finish").GetComponent<Button>();

        // set the initial cost of things
        m_ShuffleCost = m_StartShuffleCost;
        m_CheckWordCost = m_StartCheckWordCost;
        m_LockCost = m_StartLockCost;
        m_HintCost = m_StartHintCost;
        m_RevealWordsCost = m_StartRevealWordsCost;
        UpdateCosts();

        // start the first new word off
        NewWord();
    }
	
	void Update() 
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

    void UpdateCosts()
    {
        // set the total coins text
        GameObject Object = GameObject.Find("Jumble Cost");
        Text Value = Object.GetComponentInChildren<Text>();
        Value.text = m_ShuffleCost.ToString();

/*        Object = GameObject.Find("Check Cost");
        Value = Object.GetComponentInChildren<Text>();
        Value.text = m_CheckWordCost.ToString();*/

        Object = GameObject.Find("Lock Cost");
        Value = Object.GetComponentInChildren<Text>();
        Value.text = m_LockCost.ToString();

        Object = GameObject.Find("Hint Cost");
        Value = Object.GetComponentInChildren<Text>();
        Value.text = m_HintCost.ToString();
    }

    // get the scren position for a given letter when idle
    public Vector3 GetLetterIdlePosition(int _Index)
    {
        return new Vector3(_Index * m_LetterSpacing + m_LetterSpacing / 2, 199, 0f);
    }

    // get the scren position for a given letter when used
    public Vector3 GetLetterUsedPosition(int _Index)
    {
        return new Vector3(_Index * m_LetterSpacing + m_LetterSpacing / 2, 366, 0f);
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

    private void UpdateLettersNotReady()
    {
        for (int i = 0; i < m_LettersUsedIndex; i++)
        {
            if (m_LettersUsedIndex < 3)
                m_LetterList[m_LetterUsedList[i]].SetNotReady(true);
            else
                m_LetterList[m_LetterUsedList[i]].SetNotReady(false);
        }
    }

    // add a letter to the word
    private void AddLetter(int _TheButton)
    {
        m_LetterList[_TheButton].SetUsed(m_LettersUsedIndex);
        m_LetterUsedList[m_LettersUsedIndex] = _TheButton;
        m_LettersUsedIndex++;

        UpdateLettersNotReady();
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

        UpdateLettersNotReady();
        UpdatePossibleWords();
    }

    // clear all letters from the word
    public void ClearUsedLetters()
    {
        for (int i = 0; i < m_LettersUsedIndex; i++)
        {
            m_LetterList[m_LetterUsedList[i]].SetUnused();
        }
        m_LettersUsedIndex = 0;

        UpdateLettersNotReady();
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
            m_CurrentWord = m_DictionaryObject.GetLevelWord();

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
        m_BestChain = 0;
        UpdateScore();

        // work out the best possible score. todo
        m_BestPossibleScore = 1000;

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
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        string Number = Session.FormatNumberString(m_TotalScore.ToString());
        m_ScoreText.text = Number;

        m_ComboText.text = "Chain x" + m_WordsRightCombo.ToString();
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

    private bool IsValidWord()
    {
		// don't allow a no-letter submission
		if (m_LettersUsedIndex == 0)
			return false;
	
        // add all the letters to make the word
        string Word = "";
        for (int i = 0; i < m_LettersUsedIndex; i++)
        {
            Word += m_CurrentWord.Word.Substring(m_LetterList[m_LetterUsedList[i]].m_LetterIndex, 1);
        }

        // look for the word in the possible words
        for (int i = 0; i < m_CurrentWord.FitWords.Length; i++)
        {
            if (Word == m_CurrentWord.FitWords[i])
            {
                return true;
            }
        }

        return false;
    }

    private void SubmitWord()
    {
		// don't allow a no-letter submission
		if (m_LettersUsedIndex < 3)
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
        int WordIndex = 0;
        for (; WordIndex < m_CurrentWord.FitWords.Length; WordIndex++)
        {
            if (Word == m_CurrentWord.FitWords[WordIndex])
            {
                // has this word not been found yet
                if (!m_WordList[WordIndex].IsFound())
                {
                    // store the values for the end of the ceremony
                    m_CorrectWord = m_WordList[WordIndex];
                    m_WordFound = Word;
                    Right = true;

                    // has this word been found in the lifetime stats
                    if (!Session.m_SaveData.IsWordAlreadyFound(m_CurrentWord.FitWords[WordIndex]))
                    {
                        // mark the word as found
                        Session.m_SaveData.sd_WordFound.Add(m_CurrentWord.FitWords[WordIndex]);

                        // increase the number of words for that letter
                        int Letter = System.Convert.ToInt32(m_CurrentWord.FitWords[WordIndex][0]) - 65;
                        Session.m_WordFoundCounts[Letter]++;
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
                Session.m_SaveData.IncreaseChain();

                // update the score (add word length * combo)
                m_TotalScore += Word.Length * m_WordsRightCombo;

                // start the CorrectWord ceremony
                CeremonyManager Ceremony = GetComponent<CeremonyManager>();
                Ceremony.CorrectWord(Word.Length, m_WordsRightCombo);

                // increase the combo
                m_WordsRightCombo++;
                if (m_BestChain < m_WordsRightCombo - 1)
                    m_BestChain = m_WordsRightCombo - 1;
            }
            else
            {
                m_WordsWrong++;
                ++Session.m_SaveData.sd_IncorrectSubmits;

                if (!m_Locked)
                {
                    Session.m_SaveData.BreakChain();

                    // reset the combo
                    m_WordsRightCombo = 1;

                    // flash the score for 2 seconds
                    if (m_BadScore)
                        m_BadScoreTimer = 120;
                }

                // start the IncorrectWord ceremony
                CeremonyManager Ceremony = GetComponent<CeremonyManager>();
                Ceremony.IncorrectWord();
            }
            UpdateScore();
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

        ResetLock();
    }

    void BeginCeremony(eCeremonyType _Type)
    {
        CeremonyManager Ceremony = GameObject.Find("GameManager").GetComponent<CeremonyManager>();
        Ceremony.Init(_Type);
    }

    void Win()
    {
        // show the win text
        CeremonyManager Ceremony = GameObject.Find("GameManager").GetComponent<CeremonyManager>();
        bool Perfect = false;
        if (m_TotalScore == m_BestPossibleScore)
            Perfect = true;
        Ceremony.Win(Perfect);


        // start a count down before going to the result screen
        m_ShowWinTimer = 30;

        // find the best scoring word
        int BestWordScore = 0;
        string BestWord = "";
        for (int i = 0; i < m_CurrentWord.FitWords.Length; i++)
        {
            // has this word been found yet
            if (m_WordList[i].IsFound())
            {
                // is this a better score
                int Score = m_CurrentWord.FitWords[i].Length;
                if (Score > BestWordScore)
                {
                    BestWordScore = Score;
                    BestWord = m_CurrentWord.FitWords[i];
                }
            }
        }

        //Add to the save data
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.m_SaveData.LevelComplete(Session.m_CurrentZone, Session.m_CurrentLevel, BestWord, m_TotalScore);
        Session.Save();

        // remember these stats to go to the review
        Session.m_LastWordsRight = m_WordsRight;
        Session.m_LastWordsWrong = m_WordsWrong;
        Session.m_LastScore = m_TotalScore;
        Session.m_LastWord = m_CurrentWord;
        Session.m_WordsCompleted = m_WordsRight;
        Session.m_WordsAvailable = m_CurrentWord.FitWords.Length;
        Session.m_BestChain = m_BestChain;

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
        if (m_RandomHint)
        {
            // pick a random word with a hint still available
            int Index = 0;
            do
            {
                Index = Random.Range(0, m_CurrentWord.FitWords.Length);
            } while (m_WordList[Index].IsHintUsed());

            m_WordList[Index].UseHint();

            ResetHint();
        }
        // has a word been selected
        else if (m_SelectedWord)
        {
            // does this word have any hints available
            if (!m_SelectedWord.IsHintUsed())
                m_SelectedWord.UseHint();

            ResetHint();
        }
    }

    bool HintAvailable()
    {
        if (m_RandomHint)
        {
            // are there any words still to give out a hint
            int i = 0;
            for (; i < m_CurrentWord.FitWords.Length; i++)
            {
                if (!m_WordList[i].IsHintUsed())
                    return true;
            }
        }
        else if (!m_SelectedWord.IsHintUsed())
            return true;

        return false;
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

    bool SpendCoins(int Amount, int BaseAmount, out int AmountOut)
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        AmountOut = Amount;

        // does the player have enough coins
        if (Session.m_SaveData.sd_TotalScore >= Amount)
        {
            // remove the coins
            Session.m_SaveData.AddCoins(-Amount);

            // increase the cost until it's x10 original value
            AmountOut += BaseAmount;
            if (AmountOut > BaseAmount * 10)
                AmountOut = BaseAmount * 10;
            UpdateCosts();

            return true;
        }

        return false;
    }

    void UpdateHintButton()
    {
        // change the hint button to be red if it's selected, white if not
        Button Hint = GameObject.Find("Hint").GetComponent<Button>();
        ColorBlock cb = Hint.colors;
        if (m_HintReady)
            cb.normalColor = new Color(1, 0, 0);
        else
            cb.normalColor = new Color(1, 1, 1);
        cb.highlightedColor = cb.normalColor;
        Hint.colors = cb;
    }

    public void Finish()
    {
        // attempt to spend coins
        if (SpendCoins(m_RevealWordsCost, m_StartRevealWordsCost, out m_RevealWordsCost))
        {
            Win();
        }
    }

    private void Lock()
    {
        m_Locked = true;
        m_LockImage.SetActive(true);
    }

    private void ResetLock()
    {
        m_Locked = false;
        m_LockImage.SetActive(false);
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
        if (SpendCoins(m_ShuffleCost, m_StartShuffleCost, out m_ShuffleCost))
        {
            ClearUsedLetters();
            JumbleLetters();
        }
    }

    public void SubmitClicked()
    {
        SubmitWord();
    }

    public void CheckClicked()
    {
        // does the player have any letters used
        if (m_LettersUsedIndex >= 3)
        {
            // attempt to spend coins
            if (SpendCoins(m_CheckWordCost, m_StartCheckWordCost, out m_CheckWordCost))
            {
                if (IsValidWord())
                    BeginCeremony(eCeremonyType.CheckGood);
                else
                    BeginCeremony(eCeremonyType.CheckBad);
            }
        }
    }

    public void LockClicked()
    {
        // attempt to spend coins
        if (SpendCoins(m_LockCost, m_StartLockCost, out m_LockCost))
        {
            Lock();
        }
    }

    public void HintClicked()
    {
        if (!m_SelectedWord && !m_RandomHint)
        {
            m_HintReady = !m_HintReady;
            UpdateHintButton();
        }
        else
        {
            if (HintAvailable())
            {
                // attempt to spend coins
                if (SpendCoins(m_HintCost, m_StartHintCost, out m_HintCost))
                {
                    Hint();
                }
            }
        }
    }

    public void PauseClicked()
    {
        PauseManager Pause = GameObject.Find("GameManager").GetComponent<PauseManager>();
        Pause.SetIsEnabled(!Pause.m_PauseEnabled);
    }

    public void WordClicked(Word WordObject)
    {
        // can't select words when random hint is used
        if (m_RandomHint)
            return;

        SelectWord(WordObject);

        if (m_HintReady)
        {
            m_HintReady = false;
            UpdateHintButton();

            if (HintAvailable())
            {
                // attempt to spend coins
                if (SpendCoins(m_HintCost, m_StartHintCost, out m_HintCost))
                {
                    Hint();
                }
            }
        }
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

    public void CompleteWordCorrect()
    {
        // mark the word as correct
        m_CorrectWord.SetFound(m_WordFound, false);

        // return the letters to idle
        ClearUsedLetters();

        // did the player find all the words
        if (m_WordsRight == m_CurrentWord.FitWords.Length)
        {
            Win();
        }
        else
        {
            // did the player find all the words of the same size
            int Length = m_WordFound.Length;
            int i = 0;
            for (;i < m_CurrentWord.FitWords.Length;i++)
            {
                if (m_CurrentWord.FitWords[i].Length == Length && !m_WordList[i].IsFound())
                {
                    break;
                }
            }

            // all words found?
            if (i == m_CurrentWord.FitWords.Length)
            {
                // start the ceremony
                CeremonyManager Ceremony = GetComponent<CeremonyManager>();
                Ceremony.Init(eCeremonyType.All3Found + (Length - 3));
            }
        }
    }
}
