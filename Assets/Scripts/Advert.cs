using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Advertisements;

public class Advert : MonoBehaviour
{
    float m_TimeLeft;

    public Sprite[] m_Adverts;

    bool m_WaitingOnFullscreenAd = false;

    void Start()
    {
        m_TimeLeft = 20.0f;

        // pick a random image to display
        int Index = Random.Range(0, m_Adverts.Length);
        GameObject.Find("Image").GetComponent<Image>().overrideSprite = m_Adverts[Index];

        m_WaitingOnFullscreenAd = true;
    }

    void Update()
    {
        /*        Text TimeLeft = GameObject.Find("TimeLeft").GetComponent<Text>();
                TimeLeft.text = ((int)m_TimeLeft).ToString() + "s";

                m_TimeLeft -= Time.deltaTime;
                if (m_TimeLeft <= 0)
                {
                    SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
                    Session.ChangeScene("Level");
                }*/

        /*if (m_WaitingOnFullscreenAd)
        {
            if (Advertisement.IsReady())
            {
                Advertisement.Show();
                m_WaitingOnFullscreenAd = false;
            }
        }*/
    }

    public void Clicked()
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.ChangeScene("Level");
    }
}
