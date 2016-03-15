using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatsIncorrectText : MonoBehaviour
{
    void Start()
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        int puzzles = Session.m_SaveData.sd_CorrectSubmits;
        Text statsText = GetComponent<Text>();
        statsText.text = (puzzles + " Incorrect Submits");
    }
}

