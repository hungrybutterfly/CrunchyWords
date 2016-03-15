using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SessionManager : MonoBehaviour
{
    //Session Manager (this)
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
        }
    }

	void Start () 
    {
        m_FirstTimeInit = 0;
        Load();
    }
	
	void Update () 
    {
        if (m_FirstTimeInit == 2)
        {
            m_DictionaryObject = (GameObject)Instantiate(Resources.Load("Prefabs/DictionaryManager"));
            m_DictionaryManager = m_DictionaryObject.GetComponent<DictionaryManager>();
            m_DictionaryManager.Init();
            m_DictionaryObject.name = "DictionaryManager";
            DontDestroyOnLoad(m_DictionaryObject);
            ChangeScene("Cover");            
        }
        m_FirstTimeInit++;
    }

    public void ChangeScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
        bf.Serialize(file, m_SaveData);
        file.Close();
        //Debug.Log("SAVED -> "+ Application.persistentDataPath + "/playerInfo.dat");
    }

    public void Load()
    {
        if(File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            //Load - file exists
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            m_SaveData = (PlayerData)bf.Deserialize(file);
            file.Close();
            //Debug.Log("LOADED -> " + Application.persistentDataPath + "/playerInfo.dat");
        }
        else
        {
            //Create from new
            m_SaveData = new PlayerData();            
            //Debug.Log("CREATING -> " + Application.persistentDataPath + "/playerInfo.dat");
            FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");
            file.Close();
        }
    }
}


