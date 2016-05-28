using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ZoneContent : MonoBehaviour 
{
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
        for (int i = 0; i < Data.m_Zones.Length; i++)
        {
            GameObject ButtonObject = Instantiate(ButtonPrefab, new Vector3(-20, -i * Spacing - (Spacing * 0.5f), 0), Quaternion.identity) as GameObject;
            ButtonObject.transform.SetParent(transform, false);

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

            int NumLevels = Data.m_Zones[i].m_Levels.Length;

            bool Active = true;
            if (i != 0)
            {
                // see if there's any incomplete levels in the previous zone
                int j = 0;
                for (;j < NumLevels;j++)
                {
                    if (Session.m_SaveData.FindLevelComplete(i - 1, j) == -1)
                        break;
                }

                // are not all levels complete?
                if (j < NumLevels)
                {
                    // disable the button
                    TheButton.interactable = false;
                    Active = false;

                    // make it grey
                    TheButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                }
            }

            if (Active)
            {
                // tally a best score for the zone
                int TotalBestScore = 0;
                for (int j = 0; j < NumLevels; j++)
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
            }

            if (Session.m_WatchAd)
            {
                Session.m_WatchAd = false;

                // has all ads been paid for
                if (Session.m_SaveData.sd_RemoveALLAds == 0)
                {
                    Session.m_AdvertStatic = false;
                    Session.ChangeScene("Advert", LoadSceneMode.Additive);
                }
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

        GameObject String = CeremonyObject.transform.Find("Text").gameObject;
        for (int i = 0; i < 4;i++)
        {
            String.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            String.SetActive(true);
            yield return new WaitForSeconds(0.2f);
        }
            //        SessionManager.PlaySound("Fanfare_Wrong");

        yield return new WaitForSeconds(1.75f);

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
