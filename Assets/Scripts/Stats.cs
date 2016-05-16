using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Stats : MonoBehaviour 
{
    public Text m_StatsLetterPrefab;

    Text[] m_LetterStats;

    void Start()
    {
        SessionManager.MetricsLogEvent("Stats");

        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        DictionaryManager Dictionary = GameObject.Find("DictionaryManager").GetComponent<DictionaryManager>();

        GameObject Parent = GameObject.Find("Root");

        m_LetterStats = new Text[26];
        for (int i = 0; i < 26; i++)
        {
            float x = (320 - 150) + (i / 13) * 300;
            float y = 880 - ((i % 13) * 40 + 175);
            m_LetterStats[i] = Instantiate(m_StatsLetterPrefab, new Vector3(x, y, 0), Quaternion.identity) as Text;
            m_LetterStats[i].GetComponent<RectTransform>().sizeDelta = new Vector2(200, 50);
            m_LetterStats[i].transform.SetParent(Parent.GetComponent<Transform>(), false);
			int FoundCount = Session.m_WordFoundCounts[i];
            m_LetterStats[i].text = System.Convert.ToChar(i + 65) + " = " + FoundCount + " / " + Dictionary.m_FinalLetterWords[i];
        }
    }

    public void BackClicked()
    {
        SessionManager.PlaySound("Option_Back");

        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.ChangeScene("Results");
    }

}
