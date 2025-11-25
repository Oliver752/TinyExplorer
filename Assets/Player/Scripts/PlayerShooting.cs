using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{
    public Camera cam;
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public Animator playerAnimator;  // Reference to PlayerController's Animator

    [Header("Shooting Settings")]
    public float fireCooldown = 2f;  // Total cooldown between shots
    private bool canShoot = true;

    [Header("Animation Timing")]
    public string fireballTriggerName = "Fireball"; // Animator trigger
    public float fireballSpawnDelay = 1.2f;         // Delay before fireball spawns (seconds)

    void Start()
    {
        if (playerAnimator == null)
        {
            playerAnimator = GetComponentInChildren<Animator>();
            if (playerAnimator == null)
                Debug.LogWarning("Player Animator not found on this GameObject or its children!");
        }
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && canShoot)
        {
            StartCoroutine(ShootFireballRoutine());
        }
    }

    private IEnumerator ShootFireballRoutine()
    {
        canShoot = false;

        // Trigger the fireball animation
        if (playerAnimator != null)
            playerAnimator.SetTrigger(fireballTriggerName);

        // Wait for the animation to reach the fireball spawn point
        yield return new WaitForSeconds(fireballSpawnDelay);

        // Spawn the fireball
        Shoot();

        // Wait for the remaining cooldown
        yield return new WaitForSeconds(fireCooldown - fireballSpawnDelay);

        canShoot = true;
    }

    private void Shoot()
    {
        // Ray from the center of the screen
        Ray ray = cam.ScreenPointToRay(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(100f);

        Vector3 dir = (targetPoint - shootPoint.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);

        Instantiate(bulletPrefab, shootPoint.position, rot);
    }
}
