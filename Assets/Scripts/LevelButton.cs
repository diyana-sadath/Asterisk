using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public GameObject difficultyDropdown;
    private bool isDropdownVisible = false;

    void Start()
    {
        // Make sure dropdown is hidden at start
        if (difficultyDropdown != null)
        {
            difficultyDropdown.SetActive(false);
        }
    }

    public void OnClick()
    {
        if (difficultyDropdown != null)
        {
            // Toggle visibility
            isDropdownVisible = !isDropdownVisible;
            difficultyDropdown.SetActive(isDropdownVisible);
        }
    }
}