using UnityEngine;
using UnityEngine.EventSystems;

public class DeselectUIOnStart : MonoBehaviour
{
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
}
