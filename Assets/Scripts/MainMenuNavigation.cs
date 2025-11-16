using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuNavigation : MonoBehaviour
{
    [SerializeField] private Button[] buttons;   // drag all menu buttons here, top-to-bottom
    private int currentIndex = 0;
    private Color highlightColor = new Color(1f, 0.5f, 0f, 1f); // Example highlight color (orange)

    void OnEnable()
    {
        // make sure we have an EventSystem in the scene
        if (FindObjectOfType<EventSystem>() == null)
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

        // highlight the first button
        Highlight(currentIndex);
    }

    void Update()
    {
        // keyboard navigation
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentIndex = (currentIndex + 1) % buttons.Length;
            Highlight(currentIndex);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentIndex = (currentIndex - 1 + buttons.Length) % buttons.Length;
            Highlight(currentIndex);
        }

        // activate highlighted button
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space))
        {
            buttons[currentIndex].onClick.Invoke();
        }
    }

    // call this from each button’s OnClick() if you want mouse to update highlight too
    public void SetHighlight(Button b)
    {
        int idx = System.Array.IndexOf(buttons, b);
        if (idx >= 0) { currentIndex = idx; Highlight(idx); }
    }

    private void Highlight(int index)
    {
        // deactivate all buttons
        for (int i = 0; i < buttons.Length; i++)
        {
            ColorBlock colors = buttons[i].colors;
            colors.normalColor = Color.black; // Set normal color to black
            buttons[i].colors = colors;
            buttons[i].interactable = false;
        }

        // activate the chosen button (triggers ColorTint transition -> Highlighted color)
        ColorBlock highlightColors = buttons[index].colors;
        highlightColors.normalColor = highlightColor; // Set normal color to highlight color
        buttons[index].colors = highlightColors;
        buttons[index].interactable = true;
        buttons[index].Select();
    }
}