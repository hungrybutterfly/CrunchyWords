using UnityEngine;
using System.Collections;

public class ChooseIcon : MonoBehaviour 
{
    void Start()
    {
        SessionManager.MetricsLogEvent("ChooseIcon");
    }

    public void IconClicked(string Number)
    {
        string Log = "Icon" + Number;
        SessionManager.MetricsLogEvent(Log);
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        Session.ChangeScene("HowToPlay");
    }
}
