using UnityEngine;
using UnityEngine.InputSystem;

public class BookPickup : MonoBehaviour
{
    public float pickupRange = 0.3f;
    public GameObject pressETipUI;
    public GameObject book; // skript Book bude držať recepty a stav

    private Transform player;
    private bool pickedUp = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (pressETipUI != null)
            pressETipUI.SetActive(false);
    }

    void Update()
    {
        if (pickedUp || player == null) return;

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
        book.SetActive(true); // aktivuje Page Curl panel
    else
        Debug.LogError("Book nie je priradený!");

    Destroy(gameObject); // kniha v scéne zmizne
}
}
