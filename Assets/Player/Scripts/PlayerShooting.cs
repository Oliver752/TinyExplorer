using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    public Camera cam;
    public GameObject bulletPrefab;
    public Transform shootPoint;

    public float fireRate = 0.2f;
    private float fireTimer = 0f;

    void Update()
    {
        fireTimer -= Time.deltaTime;

        if (Mouse.current.leftButton.wasPressedThisFrame && fireTimer <= 0f)
        {
            Shoot();
            fireTimer = fireRate;
        }
    }

    void Shoot()
    {
        // Ray z kamery smerom dopredu (stred obrazovky)
        Ray ray = cam.ScreenPointToRay(new Vector2(
            Screen.width * 0.5f,
            Screen.height * 0.5f));

        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100f);
        }


        Vector3 dir = (targetPoint - shootPoint.position).normalized;

        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);

        Instantiate(bulletPrefab, shootPoint.position, rot);
    }
}
