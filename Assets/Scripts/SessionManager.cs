using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SessionManager : MonoBehaviour
{
	//Session Manager (this)
    [HideInInspector]
    public static SessionManager m_Instance;

	//Dictionary Object
	[HideInInspector]
	public GameObject m_DictionaryObject;

	//Dictionary Manager
	[HideInInspector]
	public DictionaryManager m_DictionaryManager;

	//First time init int (for loading screen)
	private int m_FirstTimeInit;

    //Save Data
    [HideInInspector]
    public SavePlayerData m_SaveData;
    public JSONPlayerData m_JSONSaveData;

    //Review data
    [HideInInspector]
    public int m_LastWordsRight;
    [HideInInspector]
    public int m_LastWordsWrong;
    [HideInInspector]
    public int m_LastScore;
    [HideInInspector]
    public int[] m_WordFoundCounts;

    //Last word infromation
    [HideInInspector]
    public bool m_UseLastWord;
    [HideInInspector]
    public MaxWord m_LastWord;
    [HideInInspector]
    public int m_WordsCompleted;
    [HideInInspector]
    public int m_WordsAvailable;

    [HideInInspector]
    public string m_SaveFileName = "/playerInfo7.dat";

    // current level info
    [HideInInspector]
    public int m_CurrentZone = 0;
    [HideInInspector]
    public int m_CurrentLevel = 0;

    // version string
    public string m_Version;
    // is this a version for external consumption
    public bool m_ExternalVersion;
    // debug flag for ignoring the loading of the dictionary in scenes that don't need it
    public bool m_IgnoreDictionary = false;
    public int m_StartingCoins = 100;

	void LoadDictionary ()
	{
		m_DictionaryObject = (GameObject)Instantiate (Resources.Load ("Prefabs/DictionaryManager"));
		m_DictionaryManager = m_DictionaryObject.GetComponent<DictionaryManager> ();
		m_DictionaryManager.Init ();
		m_DictionaryObject.name = "DictionaryManager";
		DontDestroyOnLoad (m_DictionaryObject);

		// now it's safe to load the player's data
		Load ();
	}

	void Awake ()
	{
		// Session Manager needs to be created and never destroyed so
		// this will make sure we only ever have one instance of Session Manager
		if (m_Instance)
			DestroyImmediate (gameObject);
		else 
        {
			DontDestroyOnLoad (gameObject);
			m_Instance = this;

			// if we're not on the loading screen then immediately load the dictionary
            if (SceneManager.GetActiveScene().name != "Start" && !m_IgnoreDictionary)
				LoadDictionary ();

			m_FirstTimeInit = 0;
			m_UseLastWord = false;
		}
	}

	void Start ()
	{
	}

	void Update ()
	{
		// if we're on the loading screen wait 2 game frames, load the dictionary then transition to the cover
		if (SceneManager.GetActiveScene ().name == "Start") 
        {
			if (m_FirstTimeInit == 2) 
            {
				LoadDictionary ();

				ChangeScene ("Cover");
			}
			m_FirstTimeInit++;
		}
	}

	public void ChangeScene (string SceneName)
	{
		SceneManager.LoadScene (SceneName);
	}

	public void Save ()
	{
        //Convert
        m_SaveData.ConvertToJSON();
        m_JSONSaveData.jssd_Dictionary = m_SaveData.sd_Dictionary;
        //Save
        BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Open (Application.persistentDataPath + m_SaveFileName, FileMode.Open);
		bf.Serialize (file, m_JSONSaveData);
		file.Close ();
        Debug.Log("SAVED -> " + Application.persistentDataPath + m_SaveFileName);
	}

	public void CreateNewSaveData ()
	{
        //Create from new
        m_SaveData = new SavePlayerData ();
        m_JSONSaveData = new JSONPlayerData();
        Debug.Log("CREATING -> " + Application.persistentDataPath + m_SaveFileName);
        FileStream file = File.Create(Application.persistentDataPath + m_SaveFileName);
		file.Close ();

        //Reset
        m_SaveData.InitSaveData();

		Save ();
	}

	public void Load ()
	{
        if (File.Exists(Application.persistentDataPath + m_SaveFileName))
        {
			//Load - file exists
			BinaryFormatter bf = new BinaryFormatter ();
            FileStream file = File.Open(Application.persistentDataPath + m_SaveFileName, FileMode.Open);
			m_JSONSaveData = (JSONPlayerData)bf.Deserialize (file);
			file.Close ();
            //Convert   
            m_SaveData = new SavePlayerData();

            m_SaveData.sd_Dictionary = m_JSONSaveData.jssd_Dictionary;
            //m_SaveData.sd_SaveString = m_JSONSaveData.jssd_SaveString;

            m_SaveData.LoadDataFromJSON();
            // is the version number different
            if (m_SaveData.sd_Version != SavePlayerData.sd_CurrentVersion) 
            {
				// do something here to upgrade the data
				// for now I'll just create a new one
				CreateNewSaveData ();
			}
            Debug.Log("LOADED -> " + Application.persistentDataPath + m_SaveFileName);
		} 
        else 
        {
			CreateNewSaveData ();            
		}
    }

    // add commas to a large number
    public string FormatNumberString(string _In)
    {
        string Out = "";

        int Counter = 0;
        for(int i = _In.Length - 1;i >= 0;i--)
        {
            if (Counter == 3)
            {
                Counter = 0;
                Out = "," + Out;
            }
            Counter++;

            Out = _In.Substring(i, 1) + Out;
        }

        return Out;
    }
}


