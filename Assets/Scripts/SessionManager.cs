using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SessionManager : MonoBehaviour
{
    public static SessionManager m_Instance;
    [HideInInspector]
    public GameObject m_DictionaryObject;
    [HideInInspector]
    public DictionaryManager m_DictionaryManager;

    int m_FirstTimeInit;

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
}
