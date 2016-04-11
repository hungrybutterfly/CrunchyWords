using UnityEngine;
using System.Collections;
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
	public PlayerData m_SaveData;

	//Review data
    [HideInInspector]
    public int m_LastWordsRight;
    [HideInInspector]
    public int m_LastWordsWrong;
    [HideInInspector]
    public int m_LastScore;

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
    public string m_SaveFileName = "/playerInfo3.dat";

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
			if (SceneManager.GetActiveScene ().name != "Start")
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
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Open (Application.persistentDataPath + m_SaveFileName, FileMode.Open);
		bf.Serialize (file, m_SaveData);
		file.Close ();
        Debug.Log("SAVED -> " + Application.persistentDataPath + m_SaveFileName);
	}

	public void CreateNewSaveData ()
	{
		//Create from new
		m_SaveData = new PlayerData ();
        Debug.Log("CREATING -> " + Application.persistentDataPath + m_SaveFileName);
        FileStream file = File.Create(Application.persistentDataPath + m_SaveFileName);
		file.Close ();

        // clear the scores
        m_SaveData.sd_PuzzlesSolved = 0;
        m_SaveData.sd_CorrectSubmits = 0;
        m_SaveData.sd_IncorrectSubmits = 0;

		// create and clear the arrays
        DictionaryManager Dictionary = GameObject.Find("DictionaryManager").GetComponent<DictionaryManager>();
        int Size = Dictionary.m_Words.Length;
		m_SaveData.sd_WordFound = new bool[Size];
		for (int i = 0; i < Size; i++)
			m_SaveData.sd_WordFound [i] = false;
		Size = 26;
		m_SaveData.sd_WordFoundCounts = new int[Size];
		for (int i = 0; i < Size; i++)
			m_SaveData.sd_WordFoundCounts [i] = 0;

        m_SaveData.sd_TotalScore = 0;
        m_SaveData.sd_RandomSeed = 12345;
        m_SaveData.sd_CurrentLevel = 0;

		Save ();
	}

	public void Load ()
	{
        if (File.Exists(Application.persistentDataPath + m_SaveFileName))
        {
			//Load - file exists
			BinaryFormatter bf = new BinaryFormatter ();
            FileStream file = File.Open(Application.persistentDataPath + m_SaveFileName, FileMode.Open);
			m_SaveData = (PlayerData)bf.Deserialize (file);
			file.Close ();
			// is the version number different
			if (m_SaveData.sd_Version != PlayerData.sd_CurrentVersion) 
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
}


