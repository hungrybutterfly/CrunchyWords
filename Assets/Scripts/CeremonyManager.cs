using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum eCeremonyType
{
    CheckGood,
    CheckBad,
    WordGood,
    WordBad,
    Win,
    All3Found,
    All4Found,
    All5Found,
    All6Found,
    AlreadyFound,

    Total
};

public class CeremonyManager : MonoBehaviour 
{
    public GameObject m_Root;

    float m_Timer;

    eCeremonyType m_Type;

    static string[] m_Strings =
    {
        "GOOD!",
        "BAD!",
        "",
        "",
        "",
        "All 3 letter words found!",
        "All 4 letter words found!",
        "All 5 letter words found!",
        "All 6 letter words found!",
        "Already Found!"
    };

    public void SetIsEnabled(bool _Active)
    {
        m_Root.gameObject.SetActive(_Active);
    }

    public void Init(eCeremonyType _Type) 
    {
        m_Timer = 1.0f;
        if (_Type >= eCeremonyType.All3Found && _Type <= eCeremonyType.All6Found)
            m_Timer = 2.0f;
        m_Type = _Type;

        SetIsEnabled(true);

        GameObject Object = GameObject.Find("Ceremony Text");
        Text String = Object.GetComponent<Text>();
        String.text = m_Strings[(int) _Type];

        if (_Type == eCeremonyType.AlreadyFound)
        {
            SessionManager.PlaySound("Fanfare_Already");
        }
        else if (_Type == eCeremonyType.All3Found)
        {
            SessionManager.PlaySound("All_3_Complete");
        }
        else if (_Type == eCeremonyType.All4Found)
        {
            SessionManager.PlaySound("All_4_Complete");
        }
        else if (_Type == eCeremonyType.All5Found)
        {
            SessionManager.PlaySound("All_5_Complete");
        }
        else if (_Type == eCeremonyType.All6Found)
        {
            SessionManager.PlaySound("All_6_Complete");
        }

        GameObject Blocker = GameObject.Find("Ceremony Panel Blocker");

        // disable raycasting so player can click
        bool Block = true;
        if (_Type >= eCeremonyType.All3Found && _Type <= eCeremonyType.All6Found)
            Block = false;
        Blocker.GetComponent<Image>().raycastTarget = Block;
    }
	
	void Update () 
    {
        m_Timer -= Time.deltaTime;
        if (m_Timer <= 0)
            SetIsEnabled(false);
	}

    public void CorrectWord(int _WordLength, int _Multiplier)
    {
        StartCoroutine(PlayCorrectWord(_WordLength, _Multiplier));
    }

    IEnumerator PlayCorrectWord(int _WordLength, int _Multiplier)
    {
        m_Type = eCeremonyType.WordGood;

        // create and attach the ceremony
        GameObject Prefab = (GameObject)Resources.Load("Prefabs/Ceremonies/CeremonySubmitGood", typeof(GameObject));
        GameObject Root = GameObject.Find("Ceremony Root");
        GameObject CeremonyObject = Instantiate(Prefab) as GameObject;
        CeremonyObject.transform.SetParent(Root.transform, false);

        SessionManager.PlaySound("Fanfare_Right");

        // set the text and make it active
        Transform t = CeremonyObject.transform.Find("Text");
        Text CeremonyText = t.gameObject.GetComponent<Text>();
        CeremonyText.text = "0";
        CeremonyText.gameObject.SetActive(true);

        GameManager Game = GameObject.Find("GameManager").GetComponent<GameManager>();

        // tally up the letters
        int TotalScore = 0;
        for (int i = 0; i < _WordLength; i++)
        {
            yield return new WaitForSeconds(0.075f);

            TotalScore += Game.CorrectWordNextLetter(i);
            CeremonyText.text = TotalScore.ToString();
        }

        // display the multiplier
        yield return new WaitForSeconds(0.25f);
        CeremonyText.text = TotalScore.ToString() + " x " + _Multiplier.ToString();

        yield return new WaitForSeconds(0.5f);

        Game.CompleteWordCorrect();

        // disable raycasting on the big panel so player can start clicking again
        CeremonyObject.GetComponent<Image>().raycastTarget = false;

        // fade out text and add on score
        int FadeDelay = 30;
        float Score = Game.m_TotalScore;
        float ScoreDelta = ((float) (Game.m_TargetScore - Game.m_TotalScore)) / FadeDelay;
        for (int i = 0; i < FadeDelay; i++)
        {
            yield return new WaitForSeconds(0.01f);

            // add on score
            Score += ScoreDelta;
            Game.m_TotalScore = (int)Score;
            Game.UpdateScore();

            // fade out text
            Color colour = CeremonyText.color;
            colour.a = 1 - ((float)i / FadeDelay);
            CeremonyText.color = colour;
        }

        // make sure final score is correct
        Game.m_TotalScore = Game.m_TargetScore;
        Game.UpdateScore();

        // hide the text
        CeremonyText.gameObject.SetActive(false);

        // delete the object
        Destroy(CeremonyObject.gameObject);

    }

    public void IncorrectWord(int _WordsRightCombo)
    {
        StartCoroutine(PlayIncorrectWord(_WordsRightCombo));
    }

    IEnumerator PlayIncorrectWord(int _WordsRightCombo)
    {
        m_Type = eCeremonyType.WordGood;

        // create and attach the ceremony
        GameObject Prefab = (GameObject)Resources.Load("Prefabs/Ceremonies/CeremonySubmitBad", typeof(GameObject));
        GameObject Root = GameObject.Find("Ceremony Root");
        GameObject CeremonyObject = Instantiate(Prefab) as GameObject;
        CeremonyObject.transform.SetParent(Root.transform, false);

        SessionManager.PlaySound("Fanfare_Wrong");

        if (_WordsRightCombo == 1)
        {
            // reveal the cross
            Image CeremonyImage = CeremonyObject.transform.Find("Image").gameObject.GetComponent<Image>();
            CeremonyImage.gameObject.SetActive(true);
        }
        else
        {
            // reveal the text
            Text CeremonyText = CeremonyObject.transform.Find("Text").gameObject.GetComponent<Text>();
            CeremonyText.text = "X" + _WordsRightCombo.ToString() + " CHAIN BROKEN!";
            CeremonyText.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(1.0f);

        if (_WordsRightCombo == 1)
        {
            // hide the cross
            Image CeremonyImage = CeremonyObject.transform.Find("Image").gameObject.GetComponent<Image>();
            CeremonyImage.gameObject.SetActive(false);
        }
        else
        {
            // hide the text
            Text CeremonyText = CeremonyObject.transform.Find("Text").gameObject.GetComponent<Text>();
            CeremonyText.gameObject.SetActive(false);
        }

        // delete the object
        Destroy(CeremonyObject.gameObject);
    }

    public void Win(bool _Perfect)
    {
        StartCoroutine(PlayWin(_Perfect));
    }

    IEnumerator PlayWin(bool _Perfect)
    {
        m_Type = eCeremonyType.Win;

        // create and attach the ceremony
        GameObject Prefab = (GameObject)Resources.Load("Prefabs/Ceremonies/CeremonyWin", typeof(GameObject));
        GameObject Root = GameObject.Find("Ceremony Root");
        GameObject CeremonyObject = Instantiate(Prefab) as GameObject;
        CeremonyObject.transform.SetParent(Root.transform, false);

        Text You = CeremonyObject.transform.Find("You").gameObject.GetComponent<Text>();
        You.gameObject.SetActive(false);
        Text Made = CeremonyObject.transform.Find("Made").gameObject.GetComponent<Text>();
        Made.gameObject.SetActive(false);
        Text All = CeremonyObject.transform.Find("All").gameObject.GetComponent<Text>();
        All.gameObject.SetActive(false);
        Text The = CeremonyObject.transform.Find("The").gameObject.GetComponent<Text>();
        The.gameObject.SetActive(false);
        Text Words = CeremonyObject.transform.Find("Words").gameObject.GetComponent<Text>();
        Words.gameObject.SetActive(false);

        Text Perfect = CeremonyObject.transform.Find("Perfect").gameObject.GetComponent<Text>();
        Perfect.gameObject.SetActive(false);

        Button Next = CeremonyObject.transform.Find("Button").gameObject.GetComponent<Button>();
        Next.gameObject.SetActive(false);

        float Delay = 0.5f;

        yield return new WaitForSeconds(Delay);

        if (_Perfect)
        {
            SessionManager.PlaySound("Level_Complete_More");
        }
        else
        {
            SessionManager.PlaySound("Level_Complete");
        }

        You.gameObject.SetActive(true);
        yield return new WaitForSeconds(Delay);
        Made.gameObject.SetActive(true);
        yield return new WaitForSeconds(Delay);
        All.gameObject.SetActive(true);
        yield return new WaitForSeconds(Delay);
        The.gameObject.SetActive(true);
        yield return new WaitForSeconds(Delay);
        Words.gameObject.SetActive(true);
        yield return new WaitForSeconds(Delay);

        if (_Perfect)
        {
            Perfect.gameObject.SetActive(true);
            yield return new WaitForSeconds(Delay);
        }

        GameManager Game = GameObject.Find("GameManager").GetComponent<GameManager>();
        Next.onClick.AddListener(() => { Game.EndClicked(); });
        Next.gameObject.SetActive(true);
    }
}
