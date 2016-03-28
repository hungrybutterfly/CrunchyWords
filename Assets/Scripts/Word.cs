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
    };

    public int m_ID;

    eState m_State;

    bool m_IsSelected;

    bool m_HintUsed;

    void Awake()
    {
        m_IsSelected = false;
        m_HintUsed = false;
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
        }

        if (m_IsSelected)
            GetComponent<Image>().color = new Color(1, 0, 0);
    }

    public void SetHidden()
    {
        SetState(eState.Hidden);
    }

    public void SetIdle(string _Word)
    {
        SetState(eState.Idle);

        GetComponentInChildren<Text>().text = _Word;
    }

    public void SetIdle()
    {
        SetState(eState.Idle);
    }

    public void SetPossible()
    {
        SetState(eState.Possible);
    }

    public void SetFound(string _Word)
    {
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

    public void UseHint(string _Word)
    {
        GetComponentInChildren<Text>().text = _Word;
        m_HintUsed = true;
    }

    public bool IsHintUsed()
    {
        return m_HintUsed;
    }
}
