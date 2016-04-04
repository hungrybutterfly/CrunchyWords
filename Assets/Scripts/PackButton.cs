using UnityEngine;
using System.Collections;

public class PackButton : MonoBehaviour 
{
    public string m_Pack;

	public void Clicked()
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.StartGame(m_Pack);
    }
}
