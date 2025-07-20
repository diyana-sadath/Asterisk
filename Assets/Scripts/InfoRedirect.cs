using UnityEngine;

public class InfoRedirect : MonoBehaviour
{
    // URL to open
    public string url = "https://asterisk-eight.vercel.app/";

    // Call this method to open the URL
    public void OpenInfoPage()
    {
        Application.OpenURL(url);
    }
}
