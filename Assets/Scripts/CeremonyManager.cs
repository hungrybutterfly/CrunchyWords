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
    }
	
	void Update () 
    {
        m_Timer -= Time.deltaTime;
        if (m_Timer <= 0)
            SetIsEnabled(false);
	}

    public void CorrectWord(int _WordScore, int _Multiplier)
    {
        StartCoroutine(PlayCorrectWord(_WordScore, _Multiplier));
    }

    IEnumerator PlayCorrectWord(int _WordScore, int _Multiplier)
    {
        m_Type = eCeremonyType.WordGood;

        // create and attach the ceremony
        GameObject Prefab = (GameObject)Resources.Load("Prefabs/Ceremonies/CeremonySubmitGood", typeof(GameObject));
        GameObject Root = GameObject.Find("Ceremony Root");
        GameObject CeremonyObject = Instantiate(Prefab) as GameObject;
        CeremonyObject.transform.SetParent(Root.transform, false);

        SessionManager.PlaySound("Fanfare_Right");

/*        // reveal the tick
                Image CeremonyImage = CeremonyObject.transform.Find("Image").gameObject.GetComponent<Image>();
                CeremonyImage.gameObject.SetActive(true);

                yield return new WaitForSeconds(1.0f);

                // hide the tick
                CeremonyImage.gameObject.SetActive(false);
        */
        // set the text and make it active
        Transform t = CeremonyObject.transform.Find("Text");
        Text CeremonyText = t.gameObject.GetComponent<Text>();
        CeremonyText.text = _WordScore.ToString() + " x " + _Multiplier.ToString();
        CeremonyText.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.0f);

        // hide the text
        CeremonyText.gameObject.SetActive(false);

        // delete the object
        Destroy(CeremonyObject.gameObject);

        GameManager Game = GameObject.Find("GameManager").GetComponent<GameManager>();
        Game.CompleteWordCorrect();
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
        m_Type = eCeremonyType.Win;

        // create and attach the ceremony
        GameObject Prefab = (GameObject)Resources.Load("Prefabs/Ceremonies/CeremonyWin", typeof(GameObject));
        GameObject Root = GameObject.Find("Ceremony Root");
        GameObject CeremonyObject = Instantiate(Prefab) as GameObject;
        CeremonyObject.transform.SetParent(Root.transform, false);

        if (_Perfect)
        {
            Text CeremonyText = CeremonyObject.transform.Find("Text").gameObject.GetComponent<Text>();
            CeremonyText.text = "PERFECT!";

            SessionManager.PlaySound("Level_Complete_More");
        }
        else
        {
            SessionManager.PlaySound("Level_Complete");
        }

        GameManager Game = GameObject.Find("GameManager").GetComponent<GameManager>();
        Button TheButton = CeremonyObject.GetComponent<Button>();
        TheButton.onClick.AddListener(() => { Game.EndClicked(); });
    }
}
