using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class MyButton : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        Button TheButton = gameObject.GetComponent<Button>();
        if (TheButton.enabled)
        {
            TheButton.onClick.Invoke();
        }
    }
}
