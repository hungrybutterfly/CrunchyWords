using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LetterButton : Button 
{
    enum eState
    {
        Idle,
        Used
    }

    [HideInInspector] public int m_Index;
    [HideInInspector] public int m_LetterIndex;

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
        m_State = _NewState;

        switch (m_State)
        {
            case eState.Idle:
                transform.localPosition = m_Manager.GetLetterIdlePosition(m_Index);
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

    public bool IsUsed()
    {
        if (m_State == eState.Used)
            return true;

        return false;
    }

    public void SetUnused()
    {
        SetState(eState.Idle);
    }

    void OnClick()
    {
        m_Manager.LetterClicked(m_Index);
    }
}
