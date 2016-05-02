using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Advert : MonoBehaviour 
{
    float m_TimeLeft;

    public Sprite[] m_Adverts;

	void Start () 
    {
        m_TimeLeft = 20.0f;

        // pick a random image to display
        int Index = Random.Range(0, m_Adverts.Length);
        GameObject.Find("Image").GetComponent<Image>().overrideSprite = m_Adverts[Index];
	}
	
	void Update () 
    {
/*        Text TimeLeft = GameObject.Find("TimeLeft").GetComponent<Text>();
        TimeLeft.text = ((int)m_TimeLeft).ToString() + "s";

        m_TimeLeft -= Time.deltaTime;
        if (m_TimeLeft <= 0)
        {
            SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
            Session.ChangeScene("Level");
        }*/
	}
}
