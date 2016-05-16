using UnityEngine;
using System.Collections;

public class Loading : MonoBehaviour 
{
	void Start () 
    {
        SessionManager.MetricsLogEvent("Loading");
    }
}
