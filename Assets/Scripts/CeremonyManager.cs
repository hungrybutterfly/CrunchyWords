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
    };

    public void SetIsEnabled(bool _Active)
    {
        m_Root.gameObject.SetActive(_Active);
    }

    public void Init(eCeremonyType _Type) 
    {
        m_Timer = 1.0f;
        m_Type = _Type;

        SetIsEnabled(true);

        GameObject Object = GameObject.Find("Ceremony Text");
        Text String = Object.GetComponent<Text>();
        String.text = m_Strings[(int) _Type];
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
        CeremonyText.text = _WordLength.ToString() + " x " + _Multiplier.ToString();
        CeremonyText.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.0f);

        // hide the text
        CeremonyText.gameObject.SetActive(false);

        // delete the object
        Destroy(CeremonyObject.gameObject);

        GameManager Game = GameObject.Find("GameManager").GetComponent<GameManager>();
        Game.CompleteWordCorrect();
    }

    public void IncorrectWord()
    {
        StartCoroutine(PlayIncorrectWord());
    }

    IEnumerator PlayIncorrectWord()
    {
        m_Type = eCeremonyType.WordGood;

        // create and attach the ceremony
        GameObject Prefab = (GameObject)Resources.Load("Prefabs/Ceremonies/CeremonySubmitBad", typeof(GameObject));
        GameObject Root = GameObject.Find("Ceremony Root");
        GameObject CeremonyObject = Instantiate(Prefab) as GameObject;
        CeremonyObject.transform.SetParent(Root.transform, false);

        // reveal the cross
        Image CeremonyImage = CeremonyObject.transform.Find("Image").gameObject.GetComponent<Image>();
        CeremonyImage.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.0f);

        // hide the cross
        CeremonyImage.gameObject.SetActive(false);

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
        }

        GameManager Game = GameObject.Find("GameManager").GetComponent<GameManager>();
        Button TheButton = CeremonyObject.GetComponent<Button>();
        TheButton.onClick.AddListener(() => { Game.EndClicked(); });
    }
}
