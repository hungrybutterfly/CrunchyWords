using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Results : MonoBehaviour {

    public Sprite NoStar;

    // turn this on to have stars taken away with wrong words
    static bool m_WrongWords = false;

    enum eParts
    {
        YouChained,
        Chain,
        PossibleWords,
        Possible,
        From,
        Word,
        ToScore,
        Points,
        EqualCoins,
        EqualCoinsImage,
        EqualCoinsValue,
        YourCurrent,
        IsWords,
        UnbrokenChain,
        BestEver,
        IsWords2,
        BestUnbrokenChain,
/*        YouUsed,
        Jumble,
        JumbleCount,
        Hint,
        HintCount,
        Lock,
        LockCount,*/
        Total
    };

    string[] PartNames =
    {
        "You Chained",
        "Chain",
        "Possible words",
        "Possible",
        "from",
        "Word",
        "to score",
        "Points",
        "equal coins",
        "equal coins image",
        "equal coins value",
        "Your current",
        "is words",
        "UnbrokenChain",
        "best ever",
        "is words2",
        "BestUnbrokenChain",
/*        "You used",
        "Jumble",
        "JumbleCount",
        "Hint",
        "HintCount",
        "Lock",
        "LockCount",*/
    };

    GameObject[] m_Parts;

    bool m_bCeremonyStarted;

    bool m_bCeremonyActive;

    float m_fNormalDelay;
    float m_fLongDelay;
    float m_fTinyDelay;

    GameObject m_Next, m_Skip;

	void Start () 
    {
        SessionManager.MetricsLogEvent("Results");

        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        Text Score = GameObject.Find("Chain").GetComponent<Text>();
        string Number = Session.FormatNumberString(Session.m_BestChain.ToString());
        Score.text = Number;

        Text From = GameObject.Find("Possible").GetComponent<Text>();
        Number = Session.FormatNumberString(Session.m_WordsAvailable.ToString());
        From.text = Number;

        // just so I can start running on the review screen
        if (Session.m_LastWord != null)
        {
            Text Word = GameObject.Find("Word").GetComponent<Text>();
            Word.text = Session.m_LastWord.Word;
        }

        Text Points = GameObject.Find("Points").GetComponent<Text>();
        Number = Session.FormatNumberString(Session.m_LastScore.ToString());
        Points.text = Number;

        Text EqualCoins = GameObject.Find("equal coins value").GetComponent<Text>();
        int Multiplier = 1;
        if (Session.m_SaveData.sd_DoubleCoins == 1)
            Multiplier = 2;
        if (Session.m_SaveData.sd_X10Coins == 1)
            Multiplier = 10;
        int Coins = (int)(((float)Session.m_LastScore * Multiplier) / Session.m_ScoreToCoins);
        Number = Coins.ToString() + ")";
        EqualCoins.text = Number;

        Text UnbrokenChain = GameObject.Find("UnbrokenChain").GetComponent<Text>();
        UnbrokenChain.text = Session.m_SaveData.sd_CurrentChain.ToString();

        Text BestUnbrokenChain = GameObject.Find("BestUnbrokenChain").GetComponent<Text>();
        BestUnbrokenChain.text = Session.m_SaveData.sd_BestChain.ToString();

/*        Text Jumble = GameObject.Find("JumbleCount").GetComponent<Text>();
        Jumble.text = Session.m_JumblesUsed.ToString();

        Text Hint = GameObject.Find("HintCount").GetComponent<Text>();
        Hint.text = Session.m_HintsUsed.ToString();

        Text Lock = GameObject.Find("LockCount").GetComponent<Text>();
        Lock.text = Session.m_LocksUsed.ToString();
        */
        m_Next = GameObject.Find("Next");
        m_Skip = GameObject.Find("Skip");

        // grab all the parts and hide them
        m_Parts = new GameObject[(int)eParts.Total];
        for (int i = 0; i < (int)eParts.Total; i++)
        {
            string Name = PartNames[i];
            m_Parts[i] = (GameObject.Find(Name));
            m_Parts[i].SetActive(false);
        }
        m_Next.SetActive(false);
        m_Skip.SetActive(false);

        m_bCeremonyStarted = false;
        m_bCeremonyActive = true;
        m_fNormalDelay = 0.5f;
        m_fLongDelay = 1.0f;
        m_fTinyDelay = 0.01f;
    }

    IEnumerator PlayCeremony()
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        if (m_bCeremonyActive) yield return new WaitForSeconds(m_fNormalDelay);

        m_Parts[(int) eParts.YouChained].SetActive(true);
        m_Parts[(int) eParts.Chain].SetActive(true);
        if (m_bCeremonyActive) yield return new WaitForSeconds(m_fNormalDelay);

        m_Parts[(int) eParts.PossibleWords].SetActive(true);
        m_Parts[(int) eParts.Possible].SetActive(true);
        if (m_bCeremonyActive) yield return new WaitForSeconds(m_fNormalDelay);

        m_Parts[(int) eParts.From].SetActive(true);
        m_Parts[(int) eParts.Word].SetActive(true);
        if (m_bCeremonyActive) yield return new WaitForSeconds(m_fNormalDelay);

        m_Parts[(int) eParts.ToScore].SetActive(true);
        m_Parts[(int) eParts.Points].SetActive(true);
        if (m_bCeremonyActive) yield return new WaitForSeconds(m_fNormalDelay);

        m_Parts[(int) eParts.EqualCoins].SetActive(true);
        m_Parts[(int) eParts.EqualCoinsImage].SetActive(true);
        m_Parts[(int) eParts.EqualCoinsValue].SetActive(true);
        if (m_bCeremonyActive) yield return new WaitForSeconds(m_fLongDelay);

        int NumFrames = 60;
        int Multiplier = 1;
        if (Session.m_SaveData.sd_DoubleCoins == 1)
            Multiplier = 2;
        if (Session.m_SaveData.sd_X10Coins == 1)
            Multiplier = 10;
        int CoinsToAdd = (int)(((float)Session.m_LastScore * Multiplier) / Session.m_ScoreToCoins);
        int CoinsTarget = Session.m_SaveData.sd_TotalScore + CoinsToAdd;
        float Coins = Session.m_SaveData.sd_TotalScore;
        float CoinsDelta = (CoinsTarget - Coins) / NumFrames;
        for (int i = 0; i < NumFrames; i++)
        {
            if (m_bCeremonyActive) yield return new WaitForSeconds(m_fTinyDelay);
            Coins += CoinsDelta;

            // update coins left
            Text String = m_Parts[(int)eParts.EqualCoinsValue].GetComponent<Text>();
            String.text = ((int) (CoinsTarget - Coins)).ToString() + ")";

            // force the update of the total coins counter
            Session.m_SaveData.sd_TotalScore = (int)Coins;
            Session.m_SaveData.AddCoins(0);
        }

        // final coin tally
        Session.m_SaveData.sd_TotalScore = CoinsTarget;
        Session.m_SaveData.AddCoins(0);
        Session.Save();
        if (m_bCeremonyActive) yield return new WaitForSeconds(m_fNormalDelay);

        m_Parts[(int) eParts.EqualCoins].SetActive(false);
        m_Parts[(int) eParts.EqualCoinsImage].SetActive(false);
        m_Parts[(int) eParts.EqualCoinsValue].SetActive(false);
        if (m_bCeremonyActive) yield return new WaitForSeconds(m_fNormalDelay);

        m_Parts[(int) eParts.YourCurrent].SetActive(true);
        if (m_bCeremonyActive) yield return new WaitForSeconds(m_fNormalDelay);

        m_Parts[(int) eParts.IsWords].SetActive(true);
        m_Parts[(int) eParts.UnbrokenChain].SetActive(true);
        if (m_bCeremonyActive) yield return new WaitForSeconds(m_fNormalDelay);

        m_Parts[(int) eParts.BestEver].SetActive(true);
        if (m_bCeremonyActive) yield return new WaitForSeconds(m_fNormalDelay);

        m_Parts[(int) eParts.IsWords2].SetActive(true);
        m_Parts[(int) eParts.BestUnbrokenChain].SetActive(true);
        if (m_bCeremonyActive) yield return new WaitForSeconds(m_fNormalDelay);

/*        m_Parts[(int) eParts.YouUsed].SetActive(true);
        if (m_bCeremonyActive) yield return new WaitForSeconds(m_fNormalDelay);

        m_Parts[(int) eParts.Jumble].SetActive(true);
        m_Parts[(int) eParts.JumbleCount].SetActive(true);
        if (m_bCeremonyActive) yield return new WaitForSeconds(m_fNormalDelay);

        m_Parts[(int) eParts.Hint].SetActive(true);
        m_Parts[(int) eParts.HintCount].SetActive(true);
        if (m_bCeremonyActive) yield return new WaitForSeconds(m_fNormalDelay);

        m_Parts[(int) eParts.Lock].SetActive(true);
        m_Parts[(int) eParts.LockCount].SetActive(true);
        if (m_bCeremonyActive) yield return new WaitForSeconds(m_fNormalDelay);*/

        m_Next.SetActive(true);

        // hide the SkipAd button if necessary
        if ((!Session.m_ZoneComplete && (Session.m_SaveData.sd_RemoveStaticAds == 0 && Session.m_SaveData.sd_RemoveALLAds == 0)) ||
            (Session.m_ZoneComplete && Session.m_SaveData.sd_RemoveALLAds == 0))
            m_Skip.SetActive(true);

        m_bCeremonyActive = false;
    }

    public void Update()
    {
        if (!m_bCeremonyStarted)
        {
            m_bCeremonyStarted = true;
            StartCoroutine(PlayCeremony());
        }

        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        // hide the SkipAd button if necessary
        if ((!Session.m_ZoneComplete && (Session.m_SaveData.sd_RemoveStaticAds != 0 || Session.m_SaveData.sd_RemoveALLAds != 0)) ||
            (Session.m_ZoneComplete && Session.m_SaveData.sd_RemoveALLAds != 0))
        {
			if (m_Skip) 
				m_Skip.SetActive (false);
			if (m_Next) {
				if (m_Next.GetComponent<Text> ()) {
					m_Next.GetComponent<Text> ().text = "NEXT";
				}
			}
        }
    }
	
	public void SkipAdClicked() 
    {
        if (m_bCeremonyActive)
        {
            SessionManager.MetricsLogEvent("ResultsSkipAdFinish");
            m_bCeremonyActive = false;
        }
        else
        {
            SessionManager.MetricsLogEvent("ResultsSkipAd");

            SessionManager.PlaySound("Option_Select");

            SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
            Session.ChangeScene("Shop", LoadSceneMode.Additive);
        }
    }

    public void SkipCeremonyClicked()
    {
        if (m_bCeremonyActive)
        {
            SessionManager.MetricsLogEvent("ResultsSkipCeremony");
            m_bCeremonyActive = false;
        }
    }

    public void NextClicked()
    {
        SessionManager.MetricsLogEvent("ResultsNext");

        SessionManager.PlaySound("Option_Select");

        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        SessionManager.MetricsLogEventWithParameters("SaveData", new Dictionary<string, string>() 
        { 
            { "BestChain", Session.m_SaveData.sd_BestChain.ToString() }, 
            { "CorrectSubmits", Session.m_SaveData.sd_CorrectSubmits.ToString() }, 
            { "IncorrectSubmits", Session.m_SaveData.sd_IncorrectSubmits.ToString() }, 
            { "LevelsComplete", Session.m_SaveData.sd_LevelsComplete.Count.ToString() }, 
            { "TotalScore", Session.m_SaveData.sd_TotalScore.ToString() }, 
        });

        // move to the next level
        Session.m_SaveData.sd_CurrentLevel++;
        Session.Save();

        if (!Session.m_ZoneComplete)
        {
			if (Session.m_SaveData.sd_RemoveStaticAds == 0 && Session.m_SaveData.sd_RemoveALLAds == 0 && Session.m_AdvertManager.IsOnline ())
            {
                Session.m_AdvertStatic = true;
                Session.m_AdvertCount = 1;
                Session.m_AdvertNextScene = "Level";
                Session.ChangeScene("Advert");
            }
            else
            {
                Session.ChangeScene("Level");
            }
        }
        else
        {
            LevelData Data = GameObject.Find("SessionManager").GetComponent<LevelData>();
            // was this the last zone
            if (Session.m_CurrentZone == Data.m_Zones.Length - 1)
            {
                // has all ads been paid for
				if (Session.m_SaveData.sd_RemoveALLAds == 0 && Session.m_AdvertManager.IsOnline ())
                {
                    Session.m_AdvertStatic = false;
					Session.m_AdvertSkippable = false;
                    Session.m_AdvertCount = 1;
                    Session.m_AdvertNextScene = "AllComplete";
                    Session.ChangeScene("Advert");
                }
                else
                {
                    Session.ChangeScene("AllComplete");
                }
            }
            else
            {
                // has all ads been paid for
				if (Session.m_SaveData.sd_RemoveALLAds == 0 && Session.m_AdvertManager.IsOnline ())
                {
                    Session.m_AdvertStatic = false;
					Session.m_AdvertSkippable = false;
                    Session.m_AdvertCount = 1;
                    Session.m_AdvertNextScene = "Zone";
                    Session.ChangeScene("Advert");
                }
                else
                {
                    Session.ChangeScene("Zone");
                }
            }
        }
    }

    public void StatsClicked()
    {
        SessionManager.MetricsLogEvent("ResultsStats");

        SessionManager.PlaySound("Option_Select");

        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.ChangeScene("Stats");
    }
}
