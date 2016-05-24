using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TotalCoins : MonoBehaviour 
{
    Color m_Colour;

    int m_FlashTimer;

    Vector3 m_Scale;

	void Start () 
    {
        m_FlashTimer = 0;
        m_Colour = GetComponentInChildren<Text>().color;
        m_Scale = gameObject.transform.localScale;

        UpdateCoins();
	}

    public void StartFlash()
    {
        m_FlashTimer = 120;
    }

    void Update()
    {
        if (m_FlashTimer > 0)
        {
            m_FlashTimer--;
            if (m_FlashTimer % 20 < 14)
            {
                //GetComponentInChildren<Text>().color = m_Colour;
                //GetComponentInChildren<Image>().color = new Color(1, 1, 1);
                gameObject.transform.localScale = m_Scale;
            }
            else
            { 
                //GetComponentInChildren<Text>().color = new Color(1, 0, 0);
                //GetComponentInChildren<Image>().color = new Color(1, 0, 0);
                gameObject.transform.localScale = m_Scale * 1.5f;
            }
        }
    }
	
	public void UpdateCoins () 
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        int Coins = Session.m_SaveData.sd_TotalScore;

        Text NumberText = GetComponentInChildren<Text>();
        string Number = Session.FormatNumberString(Coins.ToString());
        NumberText.text = Number;
    }
}
