using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ZoneContent : MonoBehaviour 
{
	void Start () 
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

	    // populate the ScrollView with buttons according to the level data
        LevelData Data = GameObject.Find("LevelData").GetComponent<LevelData>();

        // create a button for each zone
        float Spacing = 125;
        GameObject ButtonPrefab = (GameObject)Resources.Load("Prefabs/ZoneSelector", typeof(GameObject));
        for (int i = 0; i < Data.m_Zones.Length; i++)
        {
            GameObject ButtonObject = Instantiate(ButtonPrefab, new Vector3(30, -i * Spacing - (Spacing * 0.5f), 0), Quaternion.identity) as GameObject;
            ButtonObject.transform.SetParent(transform, false);

            // set the number of the button
            Text NumberText = ButtonObject.GetComponent<Text>();
            NumberText.text = (i + 1).ToString();

            // set the button text
            Button TheButton = ButtonObject.GetComponentInChildren<Button>();
            Text ButtonText = TheButton.GetComponentInChildren<Text>();
            ButtonText.text = Data.m_Zones[i].m_Name;

            // setup the button type and index
            ZoneSelector Button = TheButton.GetComponent<ZoneSelector>();
            Button.Init(false, i);

            if (i != 0)
            {
                // see if there's any incomplete levels in the previous zone
                int NumLevels = Data.m_Zones[i].m_Levels.Length;
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
                }
            }
        }

        // make the Content panel big enough for the zone buttons
        Vector2 Size = GetComponent<RectTransform>().sizeDelta;
        Size.y = Spacing * Data.m_Zones.Length;
        GetComponent<RectTransform>().sizeDelta = Size;
    }
}
