using UnityEngine;
using UnityEngine.InputSystem;
using UnityTutorial.PlayerControl;

public class BookController : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject bookUIPanel;
    public PlayerController playerController;

    private bool isOpen = false;

    void Update()
    {
        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            ToggleBook();
        }
    }

    public void ToggleBook()
    {
        isOpen = !isOpen;

        if (bookUIPanel != null)
            bookUIPanel.SetActive(isOpen);

        if (isOpen)
        {
            if (playerController != null)
                playerController.canMove = false; // zastaví pohyb a kameru

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            if (playerController != null)
                playerController.canMove = true; // opäť povolí pohyb

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
