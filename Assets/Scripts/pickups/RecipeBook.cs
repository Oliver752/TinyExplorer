using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RecipeBook : MonoBehaviour
{
    [System.Serializable]
    public class Recipe
    {
        public string recipeName;
        public bool unlocked;
        public Text uiText; // priradíš Text v UI, ktorý sa odfajkne
    }

    public Recipe[] recipes;
    public GameObject bookUIPanel; // panel knihy
    private bool isOpen = false;

    void Start()
    {
        bookUIPanel.SetActive(false);
        UpdateUI();
    }

    void Update()
    {
        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            if (!bookUIPanel.activeSelf && recipes.Length > 0)
            {
                OpenBook();
            }
            else
            {
                CloseBook();
            }
        }
    }

    public void UnlockRecipe(string recipeName)
    {
        foreach (var r in recipes)
        {
            if (r.recipeName == recipeName)
            {
                r.unlocked = true;
                UpdateUI();
                break;
            }
        }
    }

    private void UpdateUI()
    {
        foreach (var r in recipes)
        {
            if (r.uiText != null)
                r.uiText.text = r.recipeName + (r.unlocked ? " ✅" : " ❌");
        }
    }

    public void OpenBook()
    {
        bookUIPanel.SetActive(true);
        isOpen = true;
    }

    public void CloseBook()
    {
        bookUIPanel.SetActive(false);
        isOpen = false;
    }
}
