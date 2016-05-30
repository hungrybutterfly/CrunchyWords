using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AndroidBackButton : MonoBehaviour 
{
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Button button = GetComponent<Button>();
            button.onClick.Invoke();
        }
    }
}
