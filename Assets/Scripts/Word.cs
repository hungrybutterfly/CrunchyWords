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

    eState State;
    GameManager Manager;

    void Awake()
    {
        //Get a component reference to the GameManager.
        Manager = GameObject.Find("GameManager").GetComponent<GameManager>();

        SetState(eState.Idle);
    }

    void SetState(eState NewState)
    {
        // leave the old state
        switch (State)
        {
            case eState.Hidden:
                gameObject.SetActive(true);
                break;
        }

        State = NewState;

        // enter the new state
        switch (State)
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
    }

    public void SetHidden()
    {
        SetState(eState.Hidden);
    }

    public void SetIdle(string Word)
    {
        SetState(eState.Idle);

        GetComponentInChildren<Text>().text = Word;
    }

    public void SetFound(string Word)
    {
        SetState(eState.Found);

        GetComponentInChildren<Text>().text = Word;
    }

    public bool IsFound()
    {
        if (State == eState.Found)
            return true;

        return false;
    }
}
