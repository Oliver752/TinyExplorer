using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BookPickup : MonoBehaviour
{
    public float pickupRange = 2f;
    public GameObject pressETipUI;
    public GameObject book;
    public Text infoText; // Optional UI text to show messages

    private Transform player;
    private bool pickedUp = false;

    void Start()
    {
        if (pressETipUI != null)
            pressETipUI.SetActive(false);

        TryFindPlayer();

        // Optional: clear the info text at the start
        if (infoText != null)
            infoText.text = "";
    }

    void TryFindPlayer()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
            Debug.Log("Player FOUND!");
        }
    }

    void Update()
    {
        if (!pickedUp)
        {
            // If we still didn't find player, keep trying
            if (player == null)
            {
                TryFindPlayer();
                return;
            }

            float distance = Vector3.Distance(transform.position, player.position);
            if (distance <= pickupRange)
            {
                if (pressETipUI != null)
                    pressETipUI.SetActive(true);

                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    Pickup();
                }
            }
            else
            {
                if (pressETipUI != null)
                    pressETipUI.SetActive(false);
            }
        }

        // Handle book opening attempt
        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            if (!pickedUp)
            {
                if (infoText != null)
                {
                    infoText.text = "First find the green glowing book!";
                }
                Debug.Log("Player tried to open the book but hasn't picked it up yet.");
            }
            else
            {
                // Your existing book open logic can go here
                Debug.Log("Book opened!");
            }
        }
    }

    void Pickup()
{
    pickedUp = true;
    if (pressETipUI != null)
        pressETipUI.SetActive(false);

    if (book != null)
        book.SetActive(true);
    else
        Debug.LogError("BOOK object not assigned!");

    // Tell the BookUIManager that the book has been picked up
    BookUIManager uiManager = FindObjectOfType<BookUIManager>();
    if (uiManager != null)
        uiManager.hasPickedUpBook = true;

    Destroy(gameObject);
}

}
