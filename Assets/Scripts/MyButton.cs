using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MyButton : Button 
{
    enum eState
    {
        Idle,
        Used
    }

    [HideInInspector] public int Index;

    public int UsedIndex;
    eState State;

    GameManager Manager;

    void Awake()
    {
        //Get a component reference to the GameManager.
        Manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Start()
    {
        onClick.AddListener(() => OnClick());
	}
	
	void Update () 
    {
	}

    void SetState(eState NewState)
    {
        State = NewState;

        switch (State)
        {
            case eState.Idle:
                transform.localPosition = Manager.GetLetterIdlePosition(Index);
                break;

            case eState.Used:
                transform.localPosition = Manager.GetLetterUsedPosition(UsedIndex);
                break;
        }
    }

    public void SetIndex(int NewIndex)
    {
        Index = NewIndex;
        SetState(eState.Idle);
    }

    public void SetUsed(int Index)
    {
        UsedIndex = Index;
        SetState(eState.Used);
    }

    public bool IsUsed()
    {
        if (State == eState.Used)
            return true;

        return false;
    }

    public void SetUnused()
    {
        SetState(eState.Idle);
    }

    void OnClick()
    {
        Manager.LetterClicked(Index);
    }
}
