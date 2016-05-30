using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
    Lock,
    MoreCoins,

    Total
};

public class CeremonyManager : MonoBehaviour 
{
    public GameObject m_Root;

    float m_Timer;

    eCeremonyType m_Type;

    bool m_CeremonyActive;
    public bool m_CerermonyInterrupted;

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
        "Already Found!",
        "Lock",
    };

    void Start()
    {
        SetIsEnabled(false);
    }

    public void SetIsEnabled(bool _Active)
    {
        m_CeremonyActive = _Active;
        m_CerermonyInterrupted = false;
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

        if (_Type == eCeremonyType.AlreadyFound)
            Object.transform.localScale = new Vector3(1.5f, 1.5f, 1);
        else
            Object.transform.localScale = new Vector3(1, 1, 1);

        GameObject Blocker = GameObject.Find("Ceremony Panel Blocker");

        // disable raycasting so player can click
        bool Block = true;
        if (_Type >= eCeremonyType.All3Found && _Type <= eCeremonyType.All6Found)
            Block = false;
        Blocker.GetComponent<Image>().raycastTarget = Block;
    }
	
	void Update () 
    {
        if (m_Timer > 0)
        {
            m_Timer -= Time.deltaTime;
            if (m_Timer <= 0)
            {
                m_Timer = 0;
                SetIsEnabled(false);
            }
        }
	}

    public void CorrectWord(int _WordLength, int _Multiplier)
    {
        StartCoroutine(PlayCorrectWord(_WordLength, _Multiplier));
    }

    IEnumerator PlayCorrectWord(int _WordLength, int _Multiplier)
    {
        m_Type = eCeremonyType.WordGood;
        m_CeremonyActive = true;
        m_CerermonyInterrupted = false;

        m_Root.gameObject.SetActive(true);
        GameObject Blocker = GameObject.Find("Ceremony Panel Blocker");
        Blocker.GetComponent<Image>().raycastTarget = true;
        Text Object = GameObject.Find("Ceremony Text").GetComponent<Text>();
        Object.text = "";

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
        CeremonyText.text = TotalScore.ToString() + " x " + _Multiplier.ToString() + " Chain";

        yield return new WaitForSeconds(0.5f);

        Game.CompleteWordCorrect(m_CerermonyInterrupted);

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

        m_Root.gameObject.SetActive(false);
        m_CeremonyActive = false;
    }

    public void IncorrectWord(int _WordsRightCombo)
    {
        StartCoroutine(PlayIncorrectWord(_WordsRightCombo));
    }

    IEnumerator PlayIncorrectWord(int _WordsRightCombo)
    {
        m_Type = eCeremonyType.WordGood;
        m_CeremonyActive = true;
        m_CerermonyInterrupted = false;

        // create and attach the ceremony
        GameObject Prefab = (GameObject)Resources.Load("Prefabs/Ceremonies/CeremonySubmitBad", typeof(GameObject));
        GameObject Root = GameObject.Find("Ceremony Root");
        GameObject CeremonyObject = Instantiate(Prefab) as GameObject;
        CeremonyObject.transform.SetParent(Root.transform, false);

        SessionManager.PlaySound("Fanfare_Wrong");

        // reveal the cross
        Image CeremonyImage = CeremonyObject.transform.Find("Image").gameObject.GetComponent<Image>();
        CeremonyImage.gameObject.SetActive(true);

        if (_WordsRightCombo == 1)
            yield return new WaitForSeconds(1.0f);
        else
            yield return new WaitForSeconds(0.5f);

        // hide the cross
        CeremonyImage.gameObject.SetActive(false);

        if (_WordsRightCombo > 1)
        {
            // reveal the text
            Text CeremonyText = CeremonyObject.transform.Find("Text").gameObject.GetComponent<Text>();
            CeremonyText.text = "x" + _WordsRightCombo.ToString() + " CHAIN\nBROKEN!";
            CeremonyText.gameObject.SetActive(true);

            yield return new WaitForSeconds(1.0f);

            // hide the text
            CeremonyText.gameObject.SetActive(false);
        }

        // delete the object
        Destroy(CeremonyObject.gameObject);

        m_CeremonyActive = false;
    }

    public void Win(bool _Perfect, string _Word)
    {
        StartCoroutine(PlayWin(_Perfect, _Word));
    }

    IEnumerator PlayWin(bool _Perfect, string _Word)
    {
        m_Type = eCeremonyType.Win;
        m_CeremonyActive = true;
        m_CerermonyInterrupted = false;

        // create and attach the ceremony
        GameObject Prefab = (GameObject)Resources.Load("Prefabs/Ceremonies/CeremonyWin", typeof(GameObject));
        GameObject Root = GameObject.Find("Ceremony Root");
        GameObject CeremonyObject = Instantiate(Prefab) as GameObject;
        CeremonyObject.transform.SetParent(Root.transform, false);

        SceneSettings Scene = GameObject.Find("SceneSettings").GetComponent<SceneSettings>();
        Color PanelColour = Scene.GetPanelColour();

        // recolour things to the zone
        Image Panel = CeremonyObject.transform.Find("Image").GetComponent<Image>();
        Panel.color = PanelColour;

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
        Text In = CeremonyObject.transform.Find("In").gameObject.GetComponent<Text>();
        In.gameObject.SetActive(false);
        Text Word = CeremonyObject.transform.Find("Word").gameObject.GetComponent<Text>();
        Word.gameObject.SetActive(false);
        Word.text = _Word + "!";

        Text Perfect = CeremonyObject.transform.Find("Perfect").gameObject.GetComponent<Text>();
        Perfect.gameObject.SetActive(false);

        Button Next = CeremonyObject.transform.Find("Button").gameObject.GetComponent<Button>();
        Next.gameObject.SetActive(false);

        float Delay = 0.1f;

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
        In.gameObject.SetActive(true);
        yield return new WaitForSeconds(Delay);
        Word.gameObject.SetActive(true);
        yield return new WaitForSeconds(Delay);

        if (_Perfect)
        {
            Perfect.gameObject.SetActive(true);
            yield return new WaitForSeconds(Delay);
        }

        GameManager Game = GameObject.Find("GameManager").GetComponent<GameManager>();
        Next.onClick.AddListener(() => { Game.EndClicked(); });
        Next.gameObject.SetActive(true);

        m_CeremonyActive = false;
    }

    public void Lock()
    {
        StartCoroutine(PlayLock());
    }

    IEnumerator PlayLock()
    {
        m_Type = eCeremonyType.Lock;

        // create and attach the ceremony
        GameObject Prefab = (GameObject)Resources.Load("Prefabs/Ceremonies/CeremonyLock", typeof(GameObject));
        GameObject Root = GameObject.Find("Ceremony Root");
        GameObject CeremonyObject = Instantiate(Prefab) as GameObject;
        CeremonyObject.transform.SetParent(Root.transform, false);

        SessionManager.PlaySound("Fanfare_Wrong");

        GameObject Image = CeremonyObject.transform.Find("Image").gameObject;
        GameObject Text1 = CeremonyObject.transform.Find("Text").gameObject;
        Text1.SetActive(false);

        Image.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        Image.SetActive(false);
        Text1.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        Text1.SetActive(false);

         // delete the object
        Destroy(CeremonyObject.gameObject);
    }

    public void SaveChain(int _CurrentChain)
    {
        StartCoroutine(PlaySaveChain(_CurrentChain));
    }

    IEnumerator PlaySaveChain(int _CurrentChain)
    {
        m_Type = eCeremonyType.Lock;

        // create and attach the ceremony
        GameObject Prefab = (GameObject)Resources.Load("Prefabs/Ceremonies/CeremonySaveChain", typeof(GameObject));
        GameObject Root = GameObject.Find("Ceremony Root");
        GameObject CeremonyObject = Instantiate(Prefab) as GameObject;
        CeremonyObject.transform.SetParent(Root.transform, false);

        GameObject Image = CeremonyObject.transform.Find("Image").gameObject;
        GameObject Text1 = CeremonyObject.transform.Find("Text").gameObject;
        Text1.SetActive(false);
        string NewString = "Save your Chain\nwith UNDO!";
        Text1.GetComponent<Text>().text = NewString;

        // show cross
        Image.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        Image.SetActive(false);

        // flash text
        for(int i = 0;i < 3;i++)
        {
            Text1.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            Text1.SetActive(false);
            yield return new WaitForSeconds(0.1f);
        }

        // show text
        Text1.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        Text1.SetActive(false);

        // delete the object
        Destroy(CeremonyObject.gameObject);
    }


    public void MoreCoins()
    {
        StartCoroutine(PlayMoreCoins());
    }

    IEnumerator PlayMoreCoins()
    {
        m_Type = eCeremonyType.MoreCoins;

        // create and attach the ceremony
        GameObject Prefab = (GameObject)Resources.Load("Prefabs/Ceremonies/CeremonyMoreCoins", typeof(GameObject));
        GameObject Root = GameObject.Find("Ceremony Root");
        GameObject CeremonyObject = Instantiate(Prefab) as GameObject;
        CeremonyObject.transform.SetParent(Root.transform, false);

        yield return new WaitForSeconds(1.0f);

        // delete the object
        Destroy(CeremonyObject.gameObject);

        // send the player to the shop
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.ChangeScene("Shop", LoadSceneMode.Additive);
    }

    public void BlockerClicked()
    {
        if (m_CeremonyActive)
        {
            if (m_Type == eCeremonyType.WordGood)
            {
                GameManager Game = GameObject.Find("GameManager").GetComponent<GameManager>();
                Game.ClearUsedLetters();
                m_Root.gameObject.SetActive(false);
                m_CerermonyInterrupted = true;
            }
        }
    }
}
