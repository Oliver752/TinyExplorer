using UnityEngine;
using System.Collections.Generic;

public class BookIngredientManager : MonoBehaviour
{
    [System.Serializable]
    public class Ingredient
    {
        public string ingredientName;
        public bool unlocked;
        public UIIngredientSlot uiSlot; // assigned automatically
    }

    [Header("Ingredient List")]
    public List<Ingredient> ingredients = new List<Ingredient>();

    [Header("UI")]
    public Transform slotParent;            // Parent object where slots will spawn
    public UIIngredientSlot slotPrefab;     // Prefab of the ingredient row

    void Start()
    {
        GenerateUI();
    }

    void GenerateUI()
    {
        foreach (Transform child in slotParent)
            Destroy(child.gameObject);

        foreach (var ing in ingredients)
        {
            UIIngredientSlot slot = Instantiate(slotPrefab, slotParent);
            ing.uiSlot = slot;
            slot.ingredientText.text = ing.ingredientName;
            slot.SetUnlocked(ing.unlocked);
        }
    }

    public void UnlockIngredient(string ingredientName)
    {
        var ing = ingredients.Find(i => i.ingredientName == ingredientName);
        if (ing != null)
        {
            ing.unlocked = true;
            ing.uiSlot.SetUnlocked(true);
        }
        else
        {
            Debug.LogWarning($"Ingredient '{ingredientName}' not found in BookIngredientManager list.");
        }
    }
}
