using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ZoneContent : MonoBehaviour 
{
    GameObject[] m_ZoneButtons;

    int m_HighestZone;

	void Start () 
    {
        SessionManager.MetricsLogEvent("ZoneContent");

        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

	    // populate the ScrollView with buttons according to the level data
        LevelData Data = GameObject.Find("SessionManager").GetComponent<LevelData>();

        // create a button for each zone
        float Spacing = 125;
        GameObject ButtonPrefab = (GameObject)Resources.Load("Prefabs/ZoneSelector", typeof(GameObject));
        GameObject ZoneScore = (GameObject)Resources.Load("Prefabs/ZoneScore", typeof(GameObject));
        int NumLevels;
        int j;
        m_ZoneButtons = new GameObject[Data.m_Zones.Length];
        for (int i = 0; i < Data.m_Zones.Length; i++)
        {
            GameObject ButtonObject = Instantiate(ButtonPrefab, new Vector3(-20, -i * Spacing - (Spacing * 0.5f), 0), Quaternion.identity) as GameObject;
            ButtonObject.transform.SetParent(transform, false);
            m_ZoneButtons[i] = ButtonObject;

            // set the number of the button
            Text NumberText = ButtonObject.GetComponent<Text>();
            //NumberText.text = (i + 1).ToString();

            // set the button text
            Button TheButton = ButtonObject.GetComponentInChildren<Button>();
            Text ButtonText = TheButton.GetComponentInChildren<Text>();
            ButtonText.text = Data.m_Zones[i].m_Name;

            // set the button colour
            Color Colour;
            MyMisc.HexToColour(Data.m_Zones[i].m_Colour, out Colour);
            TheButton.GetComponent<Image>().color = Colour;

            // setup the button type and index
            ZoneSelector Button = TheButton.GetComponent<ZoneSelector>();
            Button.Init(false, i);

            NumLevels = Data.m_Zones[i].m_Levels.Length;

            bool Active = true;
            if (i != 0)
            {
                // see if there's any incomplete levels in the previous zone
                j = 0;
                for (;j < NumLevels;j++)
                {
                    if (Session.m_SaveData.FindLevelComplete(i - 1, j) == -1)
                        break;
                }

                // are not all levels complete?
                if (j < NumLevels)
                {
                    // disable the button
                    TheButton.enabled = false;
                    Active = false;

                    // make it grey
                    TheButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                }
            }

            if (Active)
            {
                // tally a best score for the zone
                int TotalBestScore = 0;
                for (j = 0; j < NumLevels; j++)
                {
                    int Index = Session.m_SaveData.FindLevelComplete(i, j);
                    if (Index != -1)
                    {
                        TotalBestScore += Session.m_SaveData.GetLevelCompleteScore(Index);
                    }
                }

                // create and set a score
                GameObject ScoreObject = Instantiate(ZoneScore, new Vector3(480, -i * Spacing - (Spacing * 0.5f) - 15, 0), Quaternion.identity) as GameObject;
                ScoreObject.transform.SetParent(transform, false);
                NumberText = ScoreObject.GetComponentInChildren<Text>();
                string Number = Session.FormatNumberString(TotalBestScore.ToString());
                NumberText.text = "Score " + Number;

                m_HighestZone = i;
            }
        }

        // make the Content panel big enough for the zone buttons
        Vector2 Size = GetComponent<RectTransform>().sizeDelta;
        Size.y = Spacing * Data.m_Zones.Length;
        GetComponent<RectTransform>().sizeDelta = Size;

        if (Session.m_ZoneComplete)
        {
            Session.m_ZoneComplete = false;

            // kick of the zome complete ceremony
            StartCoroutine(PlayZoneComplete());
        }
    }

    public void BackClicked()
    {
        SessionManager.MetricsLogEvent("ZoneContentBack");
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.ChangeScene("Cover");
        SessionManager.PlaySound("Option_Back");
    }

    IEnumerator PlayZoneComplete()
    {
        // create and attach the ceremony
        GameObject Prefab = (GameObject)Resources.Load("Prefabs/Ceremonies/CeremonyZoneComplete", typeof(GameObject));
        GameObject Root = GameObject.Find("Root");
        GameObject CeremonyObject = Instantiate(Prefab) as GameObject;
        CeremonyObject.transform.SetParent(Root.transform, false);

        bool AdActive = false;
        do
        {
            AdActive = false;
            Scene Advert = SceneManager.GetSceneByName("Advert");
            if (Advert != null && Advert.IsValid())
            {
                AdActive = true;
                yield return new WaitForSeconds(0.1f);
            }
        } while (AdActive);

        GameObject String = CeremonyObject.transform.Find("Text").gameObject;
        for (int i = 0; i < 8;i++)
        {
            m_ZoneButtons[m_HighestZone].gameObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            m_ZoneButtons[m_HighestZone].gameObject.SetActive(true);
            yield return new WaitForSeconds(0.2f);
        }

        // delete the object
        Destroy(CeremonyObject.gameObject);
    }

    public void ShopClicked()
    {
        SessionManager.MetricsLogEvent("ZoneContentShop");
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.ChangeScene("Shop", LoadSceneMode.Additive);
        SessionManager.PlaySound("Option_Select");
    }
}
