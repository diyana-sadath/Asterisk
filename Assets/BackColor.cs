using UnityEngine;

public class SetCameraColor : MonoBehaviour
{
    void Start()
    {
        Camera.main.clearFlags = CameraClearFlags.SolidColor;
        Camera.main.backgroundColor = Color.black;
    }
}
