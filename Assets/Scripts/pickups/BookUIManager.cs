using UnityEngine;
using UnityEngine.EventSystems;
using UnityTutorial.PlayerControl; // aby sa našiel PlayerController

public class BookUIManager : MonoBehaviour
{
    [Header("References")]
    public GameObject bookUIPanel;
    public PlayerController playerController;

    private bool isOpen = false;

    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current.iKey.wasPressedThisFrame)
        {
            ToggleBookUI();
        }
    }

    void ToggleBookUI()
    {
        isOpen = !isOpen;

        if (bookUIPanel != null)
            bookUIPanel.SetActive(isOpen);

        if (isOpen)
        {
            if (playerController != null)
                playerController.canMove = false;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            EventSystem.current.sendNavigationEvents = true;
        }
        else
        {
            if (playerController != null)
                playerController.canMove = true;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
