using UnityEngine;
using UnityEngine.InputSystem;

public class BookPickup : MonoBehaviour
{
    public float pickupRange = 2f;
    public GameObject pressETipUI;
    public GameObject book;

    private Transform player;
    private bool pickedUp = false;

    void Start()
    {
        if (pressETipUI != null)
            pressETipUI.SetActive(false);

        TryFindPlayer();
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
        if (pickedUp) return;

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

    void Pickup()
    {
        pickedUp = true;
        if (pressETipUI != null)
            pressETipUI.SetActive(false);

        if (book != null)
            book.SetActive(true);
        else
            Debug.LogError("BOOK object not assigned!");

        Destroy(gameObject);
    }
}
