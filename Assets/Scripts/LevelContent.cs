using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelContent : MonoBehaviour
{
    void Start()
    {
        SessionManager.MetricsLogEvent("LevelContent");

        // populate the ScrollView with buttons according to the level data
        LevelData Data = GameObject.Find("SessionManager").GetComponent<LevelData>();

        // get the current zone data
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        int ZoneIndex = Session.m_CurrentZone;
        LevelData.ZoneData Zone = Data.m_Zones[ZoneIndex];

        // set the zone title
        Text ZoneTitle = GameObject.Find("ZoneTitle").GetComponent<Text>();
        ZoneTitle.text = Data.m_Zones[ZoneIndex].m_Name;

        // set the zone bg colour
        Color Colour;
        MyMisc.HexToColour(Data.m_Zones[ZoneIndex].m_Colour, out Colour);
        GameObject.Find("Main Camera").GetComponent<Camera>().backgroundColor = Colour;

        // set the zone panel colour
        Colour.r *= 0.85f;
        Colour.g *= 0.85f;
        Colour.b *= 0.85f;
        GameObject.Find("ZonePanel").GetComponent<Image>().color = Colour;

        DictionaryManager Dictionary = GameObject.Find("DictionaryManager").GetComponent<DictionaryManager>();

        GameObject Root = GameObject.Find("Root");

        // create a button for each zone
        float Spacing = 110;
        GameObject ButtonPrefab = (GameObject)Resources.Load("Prefabs/ZoneSelector", typeof(GameObject));
        for (int i = 0; i < Zone.m_Levels.Length; i++)
        {
            GameObject ButtonObject = Instantiate(ButtonPrefab, new Vector3(200, 725 -i * Spacing - (Spacing * 0.5f), 0), Quaternion.identity) as GameObject;
            ButtonObject.transform.SetParent(Root.transform, false);

            // set the number of the button
            Text NumberText = ButtonObject.GetComponent<Text>();
            //NumberText.text = (ZoneIndex + 1).ToString() + "-" + (i + 1).ToString();

            // set the button text
            Button TheButton = ButtonObject.GetComponentInChildren<Button>();
            Text ButtonText = TheButton.GetComponentInChildren<Text>();
            string String = "_ _ _ _ _ _";
            // is the level complete
            int Index = Session.m_SaveData.FindLevelComplete(ZoneIndex, i);
            if (Index != -1)
            {
                String = "";
                string Word = Session.m_SaveData.GetLevelCompleteWord(Index);
                for (int j = 0;j < Dictionary.m_MaxWordSize;j++)
                {
                    if (j < Word.Length)
                        String += (Word.Substring(j, 1));
                    else
                        String += " _";
                }
            }
            ButtonText.text = String;

            // setup the button type and index
            ZoneSelector Button = TheButton.GetComponent<ZoneSelector>();
            Button.Init(true, i);

            if (i != 0)
            {
                // is the previous level incomplete
                if (Session.m_SaveData.FindLevelComplete(ZoneIndex, i - 1) == -1)
                {
                    // disable the button
                    TheButton.interactable = false;
                }
            }
        }

        // make the Content panel big enough for the zone buttons
        Vector2 Size = GetComponent<RectTransform>().sizeDelta;
        Size.y = Spacing * (Zone.m_Levels.Length);
        GetComponent<RectTransform>().sizeDelta = Size;

        if (Session.m_WatchAd)
        {
            Session.m_WatchAd = false;

            // has static ads been paid for
            if (Session.m_SaveData.sd_RemoveStaticAds == 0 && Session.m_SaveData.sd_RemoveALLAds == 0)
            {
                Session.m_AdvertStatic = true;
                Session.ChangeScene("Advert", LoadSceneMode.Additive);
            }
        }
    }

    public void BackClicked()
    {
        SessionManager.MetricsLogEvent("LevelContentBack");
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.ChangeScene("Zone");
        SessionManager.PlaySound("Option_Back");
    }
}
