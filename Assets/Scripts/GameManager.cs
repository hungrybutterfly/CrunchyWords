using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

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

    GameObject m_JumbleCostText;
    GameObject m_LockCostText;
    GameObject m_HintCostText;

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
    // used to add on a new score over time
    [HideInInspector] public int m_TargetScore;
    int m_BestPossibleScore;                // to determin if player for a perfect score
    int m_WordsRightCombo;
    Text m_ComboText;
    Color m_ComboTextColour;
    int m_BestChain;

    int m_JumblesUsed;
    int m_HintsUsed;
    int m_LocksUsed;    

    // ceremony timer for showing the WIN text
    int m_ShowWinTimer;

    // used to provide a hint
    Word m_SelectedWord;
    bool m_HintReady;
    HintButton m_HintButton;

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

    // a list of new words found
    List<string> m_NewWordsFound;

    // the time when Start() is called
    float m_StartTime;

    // this button is hidden initially and revealed if the player breaks a chain of 20 or more
    GameObject m_UndoButton;
    int m_UndoChainSession;
    int m_UndoChain;

    PauseManager m_Pause;

    private void SetASize(RectTransform _trans, Vector2 _newSize)
    {
        Vector2 oldSize = _trans.rect.size;
        Vector2 deltaSize = _newSize - oldSize;
        _trans.offsetMin = _trans.offsetMin - new Vector2(deltaSize.x * _trans.pivot.x, deltaSize.y * _trans.pivot.y);
        _trans.offsetMax = _trans.offsetMax + new Vector2(deltaSize.x * (1f - _trans.pivot.x), deltaSize.y * (1f - _trans.pivot.y));
    }

	void Start () 
    {
        SessionManager.MetricsLogEvent("HowToPlay");

        SoundManager Sound = GameObject.Find("SessionManager").GetComponent<SoundManager>();
        Sound.SetMusicVolume(0.5f);

        m_StartTime = Time.time;

        m_DictionaryObject = GameObject.Find("DictionaryManager").GetComponent<DictionaryManager>();
        m_MaxLetters = m_DictionaryObject.m_MaxWordSize;

        m_LetterSpacing = 640 / m_MaxLetters;
        m_LetterWidth = m_LetterSpacing - 5;

        m_LettersUsedIndex = 0;

        m_NewWordsFound = new List<string>();

        // create the letter slots
        GameObject LetterSlot = (GameObject)Resources.Load("Prefabs/LetterSlot", typeof(GameObject));
        GameObject[] Slots1 = new GameObject[m_MaxLetters];
        GameObject[] Slots2 = new GameObject[m_MaxLetters];
        for (int i = 0; i < m_MaxLetters; i++)
        {
            GameObject NewSlot = Instantiate(LetterSlot, GetLetterIdlePosition(i), Quaternion.identity) as GameObject;
            NewSlot.transform.SetParent(m_LetterParent, false);
            Slots2[i] = NewSlot;

            NewSlot = Instantiate(LetterSlot, GetLetterUsedPosition(i), Quaternion.identity) as GameObject;
            NewSlot.transform.SetParent(m_LetterParent, false);
            Slots1[i] = NewSlot;
        }

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
            float y = 857 - ((i % m_WordRows) * (WordHeight + 2)) - (WordHeight / 2);
            m_WordList[i] = Instantiate(m_WordPrefab, new Vector3(x, y, 0), Quaternion.identity) as Word;
            SetASize(m_WordList[i].GetComponent<RectTransform>(), new Vector2(WordWidth - 2, WordHeight));
            m_WordList[i].transform.SetParent(m_WordParent, false);
            m_WordList[i].m_ID = i;
        }

        //Get a reference to the Coins text.
        m_ScoreText = GameObject.Find("CoinCounter").GetComponent<Text>();

        //Hide the win text for now
        m_ShowWinTimer = 0;

        // no word selected
        m_SelectedWord = null;
        m_HintButton = GameObject.Find("Hint").GetComponent<HintButton>();

        // reset the lock feature
        m_Locked = false;
        m_LockImage = GameObject.Find("LockImage");
        m_LockImage.SetActive(false);

        //get the pause manager
        m_Pause = gameObject.GetComponent<PauseManager>();
        m_Pause.SetIsEnabled(false);

        // get some objects
        m_ComboText = GameObject.Find("Combo").GetComponent<Text>();
        m_ComboTextColour = m_ComboText.GetComponent<Text>().color;
//        m_FinishButton = GameObject.Find("Finish").GetComponent<Button>();

        // grab pointers to costs of things
        m_JumbleCostText = GameObject.Find("Jumble Cost");
        m_LockCostText = GameObject.Find("Lock Cost");
        m_HintCostText = GameObject.Find("Hint Cost");

        // set the initial cost of things
        m_ShuffleCost = m_StartShuffleCost;
        m_CheckWordCost = m_StartCheckWordCost;
        m_LockCost = m_StartLockCost;
        m_HintCost = m_StartHintCost;
        m_RevealWordsCost = m_StartRevealWordsCost;
        UpdateCosts();

        m_JumblesUsed = 0;
        m_HintsUsed = 0;
        m_LocksUsed = 0;

        // get the undo button and hide it
        m_UndoButton = GameObject.Find("Undo");
        m_UndoButton.SetActive(false);

        // start the first new word off
        NewWord();

        SceneSettings Scene = GameObject.Find("SceneSettings").GetComponent<SceneSettings>();
        Color PanelColour = Scene.GetPanelColour();

        // recolour things to the zone
        Image Panel = GameObject.Find("TitlePanel").GetComponent<Image>();
        Panel.color = PanelColour;
        Panel = GameObject.Find("TopPanel").GetComponent<Image>();
        Panel.color = PanelColour;
        Panel = GameObject.Find("Panel").GetComponent<Image>();
        Panel.color = PanelColour;
        Panel = m_Pause.m_Root.gameObject.transform.Find("OptionsPanel").GetComponent<Image>();
        PanelColour.r *= 0.85f;
        PanelColour.g *= 0.85f;
        PanelColour.b *= 0.85f;
        for (int i = 0; i < m_MaxLetters; i++)
        {
            Panel = Slots1[i].GetComponent<Image>();
            Panel.color = PanelColour;
            Panel = Slots2[i].GetComponent<Image>();
            Panel.color = PanelColour;
        }
    }

    void OnDestroy()
    {
        SoundManager Sound = GameObject.Find("SessionManager").GetComponent<SoundManager>();
        Sound.SetMusicVolume(1.0f);
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
                    m_ComboText.GetComponent<Text>().color = m_ComboTextColour;
                else
                    m_ComboText.GetComponent<Text>().color = new Color(1, 0, 0);
            }
        }
	}

    void UpdateCosts()
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        // set the total coins text
        Text Value = m_JumbleCostText.GetComponentInChildren<Text>();
        Value.text = m_ShuffleCost.ToString();
        if (Session.m_SaveData.sd_TotalScore >= m_ShuffleCost)
            m_JumbleCostText.gameObject.SetActive(true);
        else
            m_JumbleCostText.gameObject.SetActive(false);

/*        Object = GameObject.Find("Check Cost");
        Value = Object.GetComponentInChildren<Text>();
        Value.text = m_CheckWordCost.ToString();*/

        Value = m_LockCostText.GetComponentInChildren<Text>();
        Value.text = m_LockCost.ToString();
        if (Session.m_SaveData.sd_TotalScore >= m_LockCost)
            m_LockCostText.gameObject.SetActive(true);
        else
            m_LockCostText.gameObject.SetActive(false);

        Value = m_HintCostText.GetComponentInChildren<Text>();
        Value.text = m_HintCost.ToString();
        if (Session.m_SaveData.sd_TotalScore >= m_HintCost)
            m_HintCostText.gameObject.SetActive(true);
        else
            m_HintCostText.gameObject.SetActive(false);
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
        Button Submit = GameObject.Find("Submit").GetComponent<Button>();

        if (m_LettersUsedIndex < 3)
            Submit.interactable = false;
        else
            Submit.interactable = true;
    }

    // add a letter to the word
    private void AddLetter(int _TheButton)
    {
        SessionManager.MetricsLogEventWithParameters("LetterClickedAdd", new Dictionary<string, string>() { { "Letter", m_LetterList[_TheButton].m_Letter } });

        m_LetterList[_TheButton].SetUsed(m_LettersUsedIndex);
        m_LetterUsedList[m_LettersUsedIndex] = _TheButton;
        m_LettersUsedIndex++;

        UpdateLettersNotReady();
        UpdatePossibleWords();

        SessionManager.PlaySound("Letter_Select");
    }

    // remove the last letter from the word
    private void RemoveLastLetter()
    {
        // are there any letters to remove
        if (m_LettersUsedIndex != 0)
        {
            SessionManager.MetricsLogEvent("LetterClickedRemoveSuccess");

            m_LetterList[m_LetterUsedList[m_LettersUsedIndex - 1]].SetUnused();
            m_LettersUsedIndex--;

            SessionManager.PlaySound("Letter_Remove");
        }
        else
        {
           SessionManager.MetricsLogEvent("LetterClickedRemoveFailed");
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
            m_LetterList[m_LetterUsedList[i]].SetVisible(true);
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
			m_LetterList[i].SetLetter(m_CurrentWord.Word.Substring(i, 1));
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

        m_NewWordsFound.Clear();

        ResetHint();

        // reset the finish button
        if (m_FinishButton)
        {
            m_FinishButton.GetComponent<Image>().color = new Color(1, 1, 1);
            m_FinishButton.gameObject.SetActive(false);
            m_FinishFlashTimer = 0;
        }
    }

    public void UpdateScore()
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        string Number = Session.FormatNumberString(m_TotalScore.ToString());
        m_ScoreText.text = "Score " + Number;

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
        // add all the letters to make the word
        string Word = "";
        int Score = 0;
        for (int i = 0; i < m_LettersUsedIndex; i++)
        {
            Word += m_CurrentWord.Word.Substring(m_LetterList[m_LetterUsedList[i]].m_LetterIndex, 1);
            Score += m_LetterList[m_LetterUsedList[i]].m_Value;
        }

        // don't allow a no-letter submission
        if (m_LettersUsedIndex < 3)
        {
            SessionManager.MetricsLogEventWithParameters("SubmitTooSmall", new Dictionary<string, string>() { { "Word", Word } });
            return;
        }

        // hide the UNDO button just incase it's not already
        m_UndoButton.SetActive(false);
	
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

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
                    string NewWord = m_CurrentWord.FitWords[WordIndex];
                    if (!Session.m_SaveData.IsWordAlreadyFound(NewWord) && !m_NewWordsFound.Contains(NewWord))
                    {
                        m_NewWordsFound.Add(NewWord);
                    }
                    break;
                }
                else
                {
                    AlreadyFound = true;
                }
            }
        }

        if (AlreadyFound)
        {
            SessionManager.MetricsLogEventWithParameters("SubmitAlreadyFound", new Dictionary<string, string>() { { "Word", Word } });

            BeginCeremony(eCeremonyType.AlreadyFound);
        }
        else
        {
            // Update the score
            if (Right)
            {
                SessionManager.MetricsLogEventWithParameters("SubmitGood", new Dictionary<string, string>() { { "Word", Word } });

                m_WordsRight++;
                Debug.Log(m_WordsRight.ToString());
                ++Session.m_SaveData.sd_CorrectSubmits;
                Session.m_SaveData.IncreaseChain();

                // update the score (add word Score * combo)
                m_TargetScore = m_TotalScore + Score * m_WordsRightCombo;

                // has the player not yet finished 
                if (m_WordsRight < m_CurrentWord.FitWords.Length)
                {
                    // increase the combo
                    m_WordsRightCombo++;
                    if (m_BestChain < m_WordsRightCombo - 1)
                        m_BestChain = m_WordsRightCombo - 1;
                }
                else
                {
                    if (m_BestChain < m_WordsRightCombo)
                        m_BestChain = m_WordsRightCombo;
                }

                // start the CorrectWord ceremony
                CeremonyManager Ceremony = GetComponent<CeremonyManager>();
                Ceremony.CorrectWord(Word.Length, m_WordsRightCombo);
            }
            else
            {
                m_WordsWrong++;
                ++Session.m_SaveData.sd_IncorrectSubmits;

                // start the IncorrectWord ceremony
                CeremonyManager Ceremony = GetComponent<CeremonyManager>();

                if (!m_Locked)
                {
                    SessionManager.MetricsLogEventWithParameters("SubmitBadChainBroken", new Dictionary<string, string>() { { "Word", Word } });

                    // display the UNDO button if the chain >= 20
                    if (Session.m_SaveData.sd_CurrentChain >= 20)
                    {
                        m_UndoButton.SetActive(true);
                        m_UndoChainSession = Session.m_SaveData.sd_CurrentChain;
                        m_UndoChain = m_WordsRightCombo;

                        // start the SaveChain ceremony
                        Ceremony.SaveChain(Session.m_SaveData.sd_CurrentChain);
                    }
                    else
                    {
                        Ceremony.IncorrectWord(m_WordsRightCombo);
                    }

                    Session.m_SaveData.BreakChain();

                    // reset the combo
                    m_WordsRightCombo = 1;
                    UpdateScore();

                    // flash the score for 2 seconds
                    if (m_BadScore)
                        m_BadScoreTimer = 120;
                }
                else
                {
                    SessionManager.MetricsLogEventWithParameters("SubmitBadChainLocked", new Dictionary<string, string>() { { "Word", Word } });
                    Ceremony.Lock();
                }
            }
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
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        // show the win text
        CeremonyManager Ceremony = GameObject.Find("GameManager").GetComponent<CeremonyManager>();
        bool Perfect = false;
        if (m_BestChain == m_CurrentWord.FitWords.Length)
            Perfect = true;
        Ceremony.Win(Perfect, m_CurrentWord.Word);

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
        bool AlreadyDone = Session.m_SaveData.LevelComplete(Session.m_CurrentZone, Session.m_CurrentLevel, BestWord, m_TotalScore);
        Session.Save();

        // remember these stats to go to the review
        Session.m_LastWordsRight = m_WordsRight;
        Session.m_LastWordsWrong = m_WordsWrong;
        Session.m_LastScore = m_TotalScore;
        Session.m_LastWord = m_CurrentWord;
        Session.m_WordsCompleted = m_WordsRight;
        Session.m_WordsAvailable = m_CurrentWord.FitWords.Length;
        Session.m_BestChain = m_BestChain;
        Session.m_JumblesUsed = m_JumblesUsed;
        Session.m_HintsUsed = m_HintsUsed;
        Session.m_LocksUsed = m_LocksUsed;

        LevelData Data = GameObject.Find("SessionManager").GetComponent<LevelData>();
        if (Session.m_CurrentLevel == Data.m_Zones[Session.m_CurrentZone].m_Levels.Length - 1 && !AlreadyDone)
            Session.m_ZoneComplete = true;
        else
            Session.m_ZoneComplete = false;

        int TotalTime = (int) (Time.time - m_StartTime);

        SessionManager.MetricsLogEventWithParameters("Win", new Dictionary<string, string>() 
        { 
            { "Time", TotalTime.ToString() }, 
            { "Score", m_TotalScore.ToString() }, 
            { "BestWord", BestWord.ToString() }, 
            { "BestWordScore", BestWordScore.ToString() }, 
            { "WordsRight", m_TotalScore.ToString() }, 
            { "LastWordsWrong", Session.m_LastWordsWrong.ToString() }, 
            { "WordsCompleted", Session.m_WordsCompleted.ToString() }, 
            { "WordsAvailable", Session.m_WordsAvailable.ToString() },
            { "BestChain", m_BestChain.ToString() },
        });

        // transfer the new words found to the permanent list
        for (int i = 0; i < m_NewWordsFound.Count; i++)
        {
            // mark the word as found
            Session.m_SaveData.sd_WordFound.Add(m_NewWordsFound[i]);

            // increase the number of words for that letter
            int Letter = System.Convert.ToInt32(m_NewWordsFound[i][0]) - 65;
            Session.m_WordFoundCounts[Letter]++;
        }

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

        // hide the options button
        GameObject.Find("Pause").SetActive(false);
    }

    void ResetHint()
    {
        // reset the hint button
        m_HintButton.SetReady(false);
        m_HintButton.SetNudge(false);

        // unselect the old word
        if (m_SelectedWord)
        {
            m_SelectedWord.Selected(false);
            m_SelectedWord = null;
        }

        // turn off nudge on all words
        for (int i = 0; i < m_WordList.Length; i++)
        {
            m_WordList[i].SetNudge(false);
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
            {
                SessionManager.MetricsLogEventWithParameters("Hint", new Dictionary<string, string>() { { "Word", m_SelectedWord.m_Word } });
                m_SelectedWord.UseHint();
            }

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
            // turn off nudge on all words
            for(int i = 0;i < m_WordList.Length;i++)
            {
                m_WordList[i].SetNudge(false);
            }

            // unselect the old word
            if (m_SelectedWord)
            {
                m_SelectedWord.Selected(false);

                m_HintButton.SetNudge(false);
            }

            // is this a new word (or no word)
            if (m_SelectedWord == WordObject)
                WordObject = null;

            m_SelectedWord = WordObject;

            // select the new word
            if (m_SelectedWord)
            {
                m_SelectedWord.Selected(true);

                m_HintButton.SetNudge(true);
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
            SessionManager.MetricsLogEventWithParameters("CoinsSpentSuccess", new Dictionary<string, string>() { { "Coins", AmountOut.ToString() } });

            // remove the coins
            Session.m_SaveData.AddCoins(-Amount);

            // double the next cost until it's 1000
            AmountOut *= 2;
            if (AmountOut > 1000)
                AmountOut = 1000;

            // increase the cost until it's x10 original value
/*            AmountOut += BaseAmount;
            if (AmountOut > BaseAmount * 10)
                AmountOut = BaseAmount * 10;*/
            UpdateCosts();

            TotalCoins Coins = GameObject.Find("TotalCoins").GetComponent<TotalCoins>();
            Coins.StartFlash();

            return true;
        }
        else
        {
            SessionManager.MetricsLogEventWithParameters("CoinsSpentFailed", new Dictionary<string, string>() { { "Coins", AmountOut.ToString() } });

            CeremonyManager Ceremony = GetComponent<CeremonyManager>();
            Ceremony.MoreCoins();
        }

        return false;
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
        ResetHint();

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
        SessionManager.PlaySound("Option_Select");

        // attempt to spend coins
        int ShuffleCost = m_StartShuffleCost;
        if (SpendCoins(m_ShuffleCost, m_StartShuffleCost, out m_ShuffleCost))
        {
            SessionManager.MetricsLogEventWithParameters("JumbleSuccess", new Dictionary<string, string>() { { "Cost", ShuffleCost.ToString() } });

            ClearUsedLetters();
            JumbleLetters();
            m_JumblesUsed++;
        }
        else
        {
            SessionManager.MetricsLogEventWithParameters("JumbleFailed", new Dictionary<string, string>() { { "Cost", ShuffleCost.ToString() } });
        }
    }

    public void SubmitClicked()
    {
        SubmitWord();
    }

    public void LockClicked()
    {
        if (m_Locked)
            return;

        SessionManager.PlaySound("Option_Select");

        // attempt to spend coins
        int LockCost = m_StartLockCost;
        if (SpendCoins(m_LockCost, m_StartLockCost, out m_LockCost))
        {
            SessionManager.MetricsLogEventWithParameters("LockSuccess", new Dictionary<string, string>() { { "Cost", LockCost.ToString() } });

            Lock();
            m_LocksUsed++;
        }
        else
        {
            SessionManager.MetricsLogEventWithParameters("LockFailed", new Dictionary<string, string>() { { "Cost", LockCost.ToString() } });
        }
    }

    void AttemptHint()
    {
        SessionManager.PlaySound("Option_Select");

        if (HintAvailable())
        {
            // attempt to spend coins
            if (SpendCoins(m_HintCost, m_StartHintCost, out m_HintCost))
            {
                SessionManager.MetricsLogEventWithParameters("HintUsedSuccess", new Dictionary<string, string>() { { "Cost", m_StartHintCost.ToString() } });

                Hint();
                m_HintsUsed++;
            }
            else
            {
                SessionManager.MetricsLogEventWithParameters("HintUsedFailed", new Dictionary<string, string>() { { "Cost", m_StartHintCost.ToString() } });
            }
        }
        else
        {
            SessionManager.MetricsLogEvent("HintUsedNotAvailable");
        }
    }

    public void HintClicked()
    {
        SessionManager.PlaySound("Option_Select");

        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        if (!m_SelectedWord && !m_RandomHint)
        {
            SessionManager.MetricsLogEvent("HintReady");

            if (Session.m_SaveData.sd_TotalScore < m_HintCost)
            {
                // send the player to the shop
                Session.ChangeScene("Shop", LoadSceneMode.Additive);
            }
            else
            {
                m_HintButton.SetReady(!m_HintButton.GetReady());

                // turn on nudge on all words
                for (int i = 0; i < m_WordList.Length; i++)
                {
                    m_WordList[i].SetNudge(m_HintButton.GetReady());
                }
            }
        }
        else
        {
            SessionManager.MetricsLogEventWithParameters("HintButtonUsed", new Dictionary<string, string>() { { "Word", m_SelectedWord.m_Word } });

            AttemptHint();
        }
    }

    public void PauseClicked()
    {
        Debug.Log("PauseClicked");

        SessionManager.PlaySound("Option_Select");

        Debug.Log("1");

        m_Pause.SetIsEnabled(!m_Pause.m_PauseEnabled);
        Debug.Log("3");


        if (m_Pause.m_PauseEnabled)
            SessionManager.MetricsLogEvent("Paused");
        else
            SessionManager.MetricsLogEvent("Unpaused");
        Debug.Log("4");

    }

    public void WordClicked(Word WordObject)
    {
        Debug.Log("WordClicked");

        // can't select words when random hint is used
        if (m_RandomHint)
            return;

        SessionManager.PlaySound("Option_Select");

        SelectWord(WordObject);

        if (m_HintButton.GetReady())
        {
            m_HintButton.SetReady(false);

            SessionManager.MetricsLogEventWithParameters("HintWordUsed", new Dictionary<string, string>() { { "Word", WordObject.m_Word } });

            AttemptHint();
        }
        else
        {
            SessionManager.MetricsLogEventWithParameters("WordClicked", new Dictionary<string, string>() { { "Word", WordObject.m_Word } });
        }
    }

    public void EndClicked()
    {
        if (m_ShowWinTimer == 0)
        {
            SessionManager.MetricsLogEvent("EndClicked");

            SessionManager.PlaySound("Option_Select");

            // timer has finished so go to the result screen
            SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
            Session.ChangeScene("Results");
        }
    }

    public void CompleteWordCorrect(bool _CerermonyInterrupted)
    {
        // mark the word as correct
        m_CorrectWord.SetFound(m_WordFound, false);

        // return the letters to idle
        if (!_CerermonyInterrupted)
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
                if (Length == 3)
                    SessionManager.MetricsLogEvent("All3Found");
                if (Length == 4)
                    SessionManager.MetricsLogEvent("All4Found");
                if (Length == 5)
                    SessionManager.MetricsLogEvent("All5Found");
                if (Length == 6)
                    SessionManager.MetricsLogEvent("All6Found");

                // start the ceremony
                CeremonyManager Ceremony = GetComponent<CeremonyManager>();
                Ceremony.Init(eCeremonyType.All3Found + (Length - 3));
            }
        }
    }

    public int CorrectWordNextLetter(int Index)
    {
        LetterButton Letter = m_LetterList[m_LetterUsedList[Index]];

        CeremonyManager Ceremony = GetComponent<CeremonyManager>();
        if (!Ceremony.m_CerermonyInterrupted)
            Letter.SetVisible(false);

        m_CorrectWord.SetLetter(Index, Letter.m_Letter);

        return Letter.m_Value;
    }

    public void CeremonyBlockerClicked()
    {
        GetComponent<CeremonyManager>().BlockerClicked();
    }

    public void UndoClicked()
    {
        SessionManager.MetricsLogEvent("UndoClicked");

        m_UndoButton.SetActive(false);

        // kick off a video ad
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.m_AdvertStatic = true;
        Session.ChangeScene("Advert", LoadSceneMode.Additive);

        // revert the combo
        m_WordsRightCombo = m_UndoChain;
        UpdateScore();

        // Revert the save data
        Session.m_SaveData.sd_CurrentChain = m_UndoChainSession;
        Session.Save();
    }
}
