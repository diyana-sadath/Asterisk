using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    public Animator animator;
    private string sceneToLoad;

    public void FadeToScene(string sceneName)
    {
        sceneToLoad = sceneName;
        animator.SetBool("Fade", true);
        StartCoroutine(LoadSceneAfterFade());
    }

    IEnumerator LoadSceneAfterFade()
    {
        yield return new WaitForSeconds(0.2f); // Adjust based on animation time
        SceneManager.LoadScene(sceneToLoad);
    }

    public void OnSceneLoaded()
    {
        animator.SetBool("Fade", false);
    }
}
