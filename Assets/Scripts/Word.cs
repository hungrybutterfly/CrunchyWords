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
    };

    public int m_ID;

    eState m_State;

    bool m_IsSelected;

    bool[] m_HintUsed;

    public string m_Word;

    void Awake()
    {
        m_IsSelected = false;
        SetState(eState.Idle);
    }

    void SetState(eState _NewState)
    {
        // leave the old state
        switch (m_State)
        {
            case eState.Hidden:
                gameObject.SetActive(true);
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
        }

        if (m_IsSelected)
            GetComponent<Image>().color = new Color(1, 0, 0);
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

    void BuildString()
    {
        // build the new word based on how many hints have been used
        string String = "";
        for (int j = 0; j < m_Word.Length; j++)
        {
            if (m_HintUsed[j])
                String += (m_Word.Substring(j, 1));
            else
                String += " -";
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
