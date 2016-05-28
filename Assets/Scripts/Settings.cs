using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using MiniJSON;

[Serializable]
public class Settings
{
    // update this when the contents change. You'll also need to add an upgrade process in the SessionManager
    public const int m_CurrentVersion = 1;

    //JSON Save related
    public Dictionary<string, object> m_Dictionary;

    //Save variables
    public int m_Version = m_CurrentVersion;
    public int m_HowToSeen = 0;
    public int m_SFXEnabled;
    public int m_MusicEnabled;

    //Reset and Initialise
    public void Init()
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        m_HowToSeen = 0;
        m_SFXEnabled = 1;
        m_MusicEnabled = 0;
    }

    //Take all the savedata and convert it into a Dictionary (Ready for saving)
    public void ConvertToJSON()
    {
        //Serialise the data
        m_Dictionary = new Dictionary<string, object>();
        //Save Data
        m_Dictionary.Add("m_Version", m_Version);
        m_Dictionary.Add("m_HowToSeen", m_HowToSeen);
        m_Dictionary.Add("m_SFXEnabled", m_SFXEnabled);
        m_Dictionary.Add("m_MusicEnabled", m_MusicEnabled);
    }

    //Take the serialised dictionary, convert it and restore the values (After Loading)
    public void LoadDataFromJSON()
    {
        Init();

        //Now find it and load each key
        if (m_Dictionary.ContainsKey("m_HowToSeen")) { m_HowToSeen = (int)m_Dictionary["m_HowToSeen"]; }
        if (m_Dictionary.ContainsKey("m_SFXEnabled")) { m_SFXEnabled = (int)m_Dictionary["m_SFXEnabled"]; }
        if (m_Dictionary.ContainsKey("m_MusicEnabled")) { m_MusicEnabled = (int)m_Dictionary["m_MusicEnabled"]; }
    }
}