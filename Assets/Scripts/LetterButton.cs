using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LetterButton : Button 
{
    enum eState
    {
        Idle,
        UsedNotReady,
        Used
    }

    [HideInInspector] public int m_Index;
    [HideInInspector] public int m_LetterIndex;
    [HideInInspector] public int m_Value;
    [HideInInspector] public string m_Letter;

    public int m_UsedIndex;
    eState m_State;

    GameManager m_Manager;

    protected override void Awake()
    {
        //Get a component reference to the GameManager.
        m_Manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    protected override void Start()
    {
        onClick.AddListener(() => OnClick());
	}
	
	void Update () 
    {
	}

    void SetState(eState _NewState)
    {
        switch (m_State)
        {
            case eState.UsedNotReady:
                GetComponent<Image>().color = new Color(1, 1, 1);                
                break;
        }

        m_State = _NewState;

        switch (m_State)
        {
            case eState.Idle:
                transform.localPosition = m_Manager.GetLetterIdlePosition(m_Index);
                break;

            case eState.UsedNotReady:
                transform.localPosition = m_Manager.GetLetterUsedPosition(m_UsedIndex);
                GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                break;

            case eState.Used:
                transform.localPosition = m_Manager.GetLetterUsedPosition(m_UsedIndex);
                break;
        }
    }

    public void SetIndex(int _NewIndex)
    {
        m_Index = _NewIndex;
        SetState(eState.Idle);
    }

    public void SetLetterIndex(int _NewIndex)
    {
        m_LetterIndex = _NewIndex;
    }

    public void SetUsed(int _Index)
    {
        m_UsedIndex = _Index;
        SetState(eState.Used);
    }

    public void SetNotReady(bool NotReady)
    {
        if (NotReady && m_State == eState.Used)
            SetState(eState.UsedNotReady);
        else if (!NotReady && m_State == eState.UsedNotReady)
            SetState(eState.Used);
    }

    public bool IsUsed()
    {
        if (m_State == eState.UsedNotReady || m_State == eState.Used)
            return true;

        return false;
    }

    public void SetUnused()
    {
        SetState(eState.Idle);
    }

    public void SetVisible(bool Visible)
    {
        gameObject.SetActive(Visible);
    }

    public void SetLetter(string _Letter)
    {
        LetterScores Scores = GameObject.Find("GameManager").GetComponent<LetterScores>();

        m_Letter = _Letter;

        Text[] Texts = GetComponentsInChildren<Text>();
        for (int i = 0; i < Texts.Length;i++)
        {
            if (Texts[i].name == "Text")
                Texts[i].text = _Letter;
            if (Texts[i].name == "Value")
            {
                char c = _Letter[0];
                int Index = c;
                m_Value = Scores.m_Values[Index - 65];
                Texts[i].text = m_Value.ToString();
            }
        }
    }

    void OnClick()
    {
        m_Manager.LetterClicked(m_Index);
    }
}
