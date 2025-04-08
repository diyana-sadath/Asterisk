using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadGameplay()
    {
        SceneManager.LoadScene("GameplayScreen");
    }
}
