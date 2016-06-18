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
        Text ZoneTitle = GameObject.Find("TitleBar").GetComponentInChildren<Text>();
        ZoneTitle.text = Data.m_Zones[ZoneIndex].m_Name;

        DictionaryManager Dictionary = GameObject.Find("DictionaryManager").GetComponent<DictionaryManager>();

        GameObject Root = GameObject.Find("Root");

        // create a button for each zone
        float Spacing = 101;
        GameObject ButtonPrefab = (GameObject)Resources.Load("Prefabs/LevelSelector", typeof(GameObject));
        GameObject ZoneScore = (GameObject)Resources.Load("Prefabs/ZoneScore", typeof(GameObject));
        for (int i = 0; i < Zone.m_Levels.Length; i++)
        {
            GameObject ButtonObject = Instantiate(ButtonPrefab, new Vector3(280, 755 -i * Spacing - (Spacing * 0.5f), 0), Quaternion.identity) as GameObject;
            ButtonObject.transform.SetParent(Root.transform, false);

            // set the number of the button
            Text NumberText = ButtonObject.transform.Find("Text2").GetComponent<Text>();
            NumberText.text = (ZoneIndex + 1).ToString() + "-" + (i + 1).ToString();

            // set the button text
            Button TheButton = ButtonObject.GetComponentInChildren<Button>();
            Text ButtonText = ButtonObject.transform.Find("Text").GetComponent<Text>();
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

                int BestScore = Session.m_SaveData.GetLevelCompleteScore(Index);
                // create and set a score
                GameObject ScoreObject = Instantiate(ZoneScore, new Vector3(630, 705 -i * Spacing - (Spacing * 0.5f) - 15, 0), Quaternion.identity) as GameObject;
                ScoreObject.transform.SetParent(Root.transform, false);
                NumberText = ScoreObject.GetComponentInChildren<Text>();
                string Number = Session.FormatNumberString(BestScore.ToString());
                NumberText.text = "Score " + Number;
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
                    TheButton.enabled = false;

                    // make it grey
                    TheButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                }
            }
        }

        // make the Content panel big enough for the zone buttons
        Vector2 Size = GetComponent<RectTransform>().sizeDelta;
        Size.y = Spacing * (Zone.m_Levels.Length);
        GetComponent<RectTransform>().sizeDelta = Size;
    }

    public void BackClicked()
    {
        SessionManager.MetricsLogEvent("LevelContentBack");
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.ChangeScene("Zone");
        SessionManager.PlaySound("Option_Back");
    }

    public void ShopClicked()
    {
        SessionManager.MetricsLogEvent("LevelContentShop");
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.ChangeScene("Shop", LoadSceneMode.Additive);
        SessionManager.PlaySound("Option_Select");
    }
}
