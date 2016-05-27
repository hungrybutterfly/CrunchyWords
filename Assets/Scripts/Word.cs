using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Word : MonoBehaviour 
{
    enum eState
    {
        Hidden,
        Idle,
        Possible,
        Found,
        Ended,
        Nudge,
    };

    public int m_ID;

    eState m_State;

    bool m_IsSelected;

    bool[] m_HintUsed;

    public string m_Word;

    float m_NudgeTimer;
    eState m_NudgeOldState;

    void Awake()
    {
        m_IsSelected = false;
        SetState(eState.Idle);
    }

    void Update()
    {
        switch (m_State)
        {
        case eState.Nudge:
            m_NudgeTimer += Time.deltaTime;
            if (((int)(m_NudgeTimer * 60)) % 30 < 20)
            {
                Color Colour = GameObject.Find("SessionManager").GetComponent<SessionManager>().m_HintColour;
                Colour.r = 1 - ((1 - Colour.r) * 0.5f);
                Colour.g = 1 - ((1 - Colour.g) * 0.5f);
                Colour.b = 1 - ((1 - Colour.b) * 0.5f);
                GetComponent<Image>().color = Colour;
            }
            else
                GetComponent<Image>().color = new Color(1, 1, 1, 1);
            break;
        }
    }

    void SetState(eState _NewState)
    {
        // leave the old state
        switch (m_State)
        {
            case eState.Hidden:
                gameObject.SetActive(true);
                break;

            case eState.Nudge:
                GetComponent<Image>().color = new Color(1, 1, 1);
                break;
        }

        m_State = _NewState;

        // enter the new state
        switch (m_State)
        {
            case eState.Hidden:
                gameObject.SetActive(false);
                break;

            case eState.Idle:
                GetComponent<Image>().color = new Color(1, 1, 1);
                break;

            case eState.Possible:
                GetComponent<Image>().color = new Color(1, 1, 0.5f);
                break;

            case eState.Found:
                GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                break;

            case eState.Ended:
                GetComponent<Image>().color = new Color(1, 0, 0, 0.5f);
                break;

            case eState.Nudge:
                m_NudgeTimer = 0;
                break;
        }

        if (m_IsSelected)
        {
            Color Colour = GameObject.Find("SessionManager").GetComponent<SessionManager>().m_HintColour;
            GetComponent<Image>().color = Colour;
        }
    }

    public void SetHidden()
    {
        SetState(eState.Hidden);
    }

    public void SetIdle()
    {
        SetState(eState.Idle);
    }

    public void SetPossible()
    {
        SetState(eState.Possible);
    }

    public void SetNudge(bool Nudge)
    {
        if (Nudge)
        {
            if ((m_State == eState.Idle || m_State == eState.Possible) && !IsHintUsed())
            {
                m_NudgeOldState = m_State;
                SetState(eState.Nudge);
            }
        }
        else
        {
            if (m_State == eState.Nudge)
            {
                SetState(m_NudgeOldState);
            }
        }
    }

    public void SetFound(string _Word, bool Ended)
    {
        if (Ended)
            SetState(eState.Ended);
        else
            SetState(eState.Found);

		GetComponentInChildren<Text>().text = _Word;
    }

    public bool IsFound()
    {
        if (m_State == eState.Found)
            return true;

        return false;
    }

    public bool IsHidden()
    {
        if (m_State == eState.Hidden)
            return true;

        return false;
    }

    public void Clicked()
    {
        GameManager Game = GameObject.Find("GameManager").GetComponent<GameManager>();
        Game.WordClicked(this);
    }

    public void Selected(bool IsSelected)
    {
        m_IsSelected = IsSelected;
        SetState(m_State);
    }

    public void SetLetter(int Index, string Letter)
    {
        string OldWord = GetComponentInChildren<Text>().text;
        string Word = OldWord.Substring(0, Index) + Letter + OldWord.Substring(Index + 1, m_Word.Length - (Index + 1));
        GetComponentInChildren<Text>().text = Word;
    }

    void BuildString()
    {
        // build the new word based on how many hints have been used
        string String = "";
        for (int j = 0; j < m_Word.Length; j++)
        {
            if (m_HintUsed[j])
                String += (m_Word.Substring(j, 1));
            else
                String += " _";
        }

        GetComponentInChildren<Text>().text = String;
    }

    public void SetWord(string _Word)
    {
        m_Word = _Word;

        // allocate hint space
        m_HintUsed = new bool[_Word.Length];
        for (int i = 0; i < _Word.Length; i++)
            m_HintUsed[i] = false;
        // the first letter is always revealed
        m_HintUsed[0] = true;

        BuildString();
    }

    public void UseHint()
    {
        // look for an unused hint slot
        int Index = 0;
        do
        {
            Index = (int)(Random.value * m_Word.Length);
        } while (m_HintUsed[Index]);

        // make that slot as used
        m_HintUsed[Index] = true;

        BuildString();
    }

    public bool IsHintUsed()
    {
        if (IsFound())
            return true;

        // count how many hints have been used
        int Count = 0;
        for (int j = 0; j < m_HintUsed.Length; j++)
        {
            if (m_HintUsed[j])
                Count++;
        }

        if (Count == m_Word.Length - 1)
            return true;

        return false;
    }
}
