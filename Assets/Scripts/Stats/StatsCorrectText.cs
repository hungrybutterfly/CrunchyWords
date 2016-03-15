﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatsCorrectText : MonoBehaviour
{
    void Start()
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        int puzzles = Session.m_SaveData.sd_IncorrectSubmits;
        Text statsText = GetComponent<Text>();
        statsText.text = (puzzles + " Correct Submits");
    }
}
