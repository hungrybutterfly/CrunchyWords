using UnityEngine;
using UnityEngine.SceneManagement;
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
    [HideInInspector]
    public JSONPlayerData m_JSONSaveData;
    [HideInInspector]
    public string m_SaveFileName2 = "/playerInfo8.dat";

    //Settings Data
    [HideInInspector]
    public Settings m_Settings;
    [HideInInspector]
    public JSONPlayerData m_JSONSettings;
    [HideInInspector]
    public string m_SettingsFileName2 = "/Settings1.dat";

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
    public int m_BestChain;
    [HideInInspector]
    public bool m_ZoneComplete = false;
    [HideInInspector]
    public int m_JumblesUsed;
    [HideInInspector]
    public int m_HintsUsed;
    [HideInInspector]
    public int m_LocksUsed;    

    // current level info
    [HideInInspector]
    public int m_CurrentZone = 0;
    [HideInInspector]
    public int m_CurrentLevel = 0;

    //Adverts
    [HideInInspector]
    public AdvertManager m_AdvertManager = null;
	// what kind of advert to show when launching ads
	public bool m_AdvertStatic;
    public int m_AdvertCount;
    // which scene to go to after an advert. null means stay where you are
    public string m_AdvertNextScene;

    //Flurry
    public bool m_AllowFlurry = false;

    //Audio
    public bool m_AllowAudio = false;

	//In App Purchasing
	[HideInInspector]
	public IAPurchaser m_IAPManager = null;

    // version string
    public string m_Version;
    // is this a version for external consumption
    public bool m_ExternalVersion;
    // debug flag for ignoring the loading of the dictionary in scenes that don't need it
    public bool m_IgnoreDictionary = false;
    public int m_StartingCoins = 100;    

    // generate the dictionary at runtime
    public bool m_GenerateMaxWords = true;

    // conversion rate from score to coins
    public float m_ScoreToCoins = 10.0f;

    [HideInInspector]
    public Color m_HintColour = new Color(0.5f, 1, 1);

    void LoadDictionary()
    {
        m_DictionaryObject = (GameObject)Instantiate(Resources.Load("Prefabs/DictionaryManager"));
        m_DictionaryManager = m_DictionaryObject.GetComponent<DictionaryManager>();
        m_DictionaryManager.Init();
        m_DictionaryObject.name = "DictionaryManager";
        DontDestroyOnLoad(m_DictionaryObject);

        // now it's safe to load the player's data
        Load();
    }

    void Awake()
    {
        // Session Manager needs to be created and never destroyed so
        // this will make sure we only ever have one instance of Session Manager
        if (m_Instance)
            DestroyImmediate(gameObject);
        else
        {
            DontDestroyOnLoad(gameObject);
            m_Instance = this;

            // if we're not on the loading screen then immediately load the dictionary
            if (SceneManager.GetActiveScene().name != "Start" && SceneManager.GetActiveScene().name != "HowToPlay" && !m_IgnoreDictionary)
                LoadDictionary();

            m_FirstTimeInit = 0;
            m_UseLastWord = false;

            //Create Advert Manager
            if (!m_AdvertManager)
            {
                m_AdvertManager = GetComponent<AdvertManager>();
                m_AdvertManager.RequestAd();
            }

            //Create Flurry Analyics
            if (m_AllowFlurry)
            {
                //Flurry (IOS, ANDROID, CRASH)
                KHD.FlurryAnalytics.Instance.StartSession("RTFXKFFH7FKH545GTCQ5", "FGF2V8MGPGPHV5BSPWRM", true);
                KHD.FlurryAnalytics.Instance.SetAppVersion(m_Version);
                KHD.FlurryAnalytics.Instance.SetUserId(m_AdvertManager.GetDeviceID());
            }

			//Create IAP
			if (!m_IAPManager) 
			{
				m_IAPManager = GetComponent<IAPurchaser> ();
			}
        }
    }

    void Start()
    {
        LoadSettings();
    }

    void Update()
    {
        if (m_FirstTimeInit == 5)
        {
            LoadDictionary();
        }
        m_FirstTimeInit++;
    }

    public void ChangeScene(string SceneName, LoadSceneMode Mode = LoadSceneMode.Single)
    {
        SceneManager.LoadScene(SceneName, Mode);
    }

    public void ReturnScene(string SceneName)
    {
        SceneManager.UnloadScene(SceneName);
    }

    public void Save()
    {
        //Convert
        m_SaveData.ConvertToJSON();
        m_JSONSaveData.jssd_Dictionary = m_SaveData.sd_Dictionary;
        //Save
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + m_SaveFileName2, FileMode.Open);
        bf.Serialize(file, m_JSONSaveData);
        file.Close();
        Debug.Log("SAVED -> " + Application.persistentDataPath + m_SaveFileName2);
    }

    public void CreateNewSaveData()
    {
        //Create from new
        m_SaveData = new SavePlayerData();
        m_JSONSaveData = new JSONPlayerData();
        Debug.Log("CREATING -> " + Application.persistentDataPath + m_SaveFileName2);
        FileStream file = File.Create(Application.persistentDataPath + m_SaveFileName2);
        file.Close();

        //Reset
        m_SaveData.InitSaveData();

        Save();
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + m_SaveFileName2))
        {
            //Load - file exists
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + m_SaveFileName2, FileMode.Open);
            m_JSONSaveData = (JSONPlayerData)bf.Deserialize(file);
            file.Close();
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
                CreateNewSaveData();
            }
            Debug.Log("LOADED -> " + Application.persistentDataPath + m_SaveFileName2);
        }
        else
        {
            CreateNewSaveData();
        }
    }


    public void SaveSettings()
    {
        //Convert
        m_Settings.ConvertToJSON();
        m_JSONSettings.jssd_Dictionary = m_Settings.m_Dictionary;
        //Save
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + m_SettingsFileName2, FileMode.Open);
        bf.Serialize(file, m_JSONSettings);
        file.Close();
        Debug.Log("SAVED Settings -> " + Application.persistentDataPath + m_SettingsFileName2);
    }

    public void CreateNewSettings()
    {
        //Create from new
        m_Settings = new Settings();
        m_JSONSettings = new JSONPlayerData();
        Debug.Log("CREATING Settings -> " + Application.persistentDataPath + m_SettingsFileName2);
        FileStream file = File.Create(Application.persistentDataPath + m_SettingsFileName2);
        file.Close();

        //Reset
        m_Settings.Init();

        SaveSettings();
    }

    public void LoadSettings()
    {
        if (File.Exists(Application.persistentDataPath + m_SettingsFileName2))
        {
            //Load - file exists
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + m_SettingsFileName2, FileMode.Open);
            m_JSONSettings = (JSONPlayerData)bf.Deserialize(file);
            file.Close();
            //Convert   
            m_Settings = new Settings();

            m_Settings.m_Dictionary = m_JSONSettings.jssd_Dictionary;

            m_Settings.LoadDataFromJSON();
            Debug.Log("LOADED Settings -> " + Application.persistentDataPath + m_SettingsFileName2);

            GetComponent<SoundManager>().UpdateSFX(m_Settings.m_SFXEnabled);
            GetComponent<SoundManager>().UpdateMusic(m_Settings.m_MusicEnabled);
        }
        else
        {
            CreateNewSettings();
        }
    }

    // add commas to a large number
    public string FormatNumberString(string _In)
    {
        string Out = "";

        int Counter = 0;
        for (int i = _In.Length - 1; i >= 0; i--)
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

    public void ToggleSFX()
    {
        m_Settings.m_SFXEnabled = 1 - m_Settings.m_SFXEnabled;
        GetComponent<SoundManager>().UpdateSFX(m_Settings.m_SFXEnabled);
        Save();
    }

    public void ToggleMusic()
    {
        m_Settings.m_MusicEnabled = 1 - m_Settings.m_MusicEnabled;
        GetComponent<SoundManager>().UpdateMusic(m_Settings.m_MusicEnabled);
        Save();
    }

    static public void MetricsLogEvent(string _EventName)
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        if (Session.m_AllowFlurry)
        {
            KHD.FlurryAnalytics.Instance.LogEvent(_EventName);
        }
    }

    static public void MetricsLogEventWithParameters(string _EventName, Dictionary<string, string> _Params)
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        if (Session.m_AllowFlurry)
        {
            KHD.FlurryAnalytics.Instance.LogEventWithParameters(_EventName, _Params);
        }
    }

    static public void PlaySound(string _Name)
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        if (Session.m_AllowAudio)
        {
            SoundManager Manager = GameObject.Find("SessionManager").GetComponent<SoundManager>();
            Manager.PlaySound(_Name);
        }
    }
}


