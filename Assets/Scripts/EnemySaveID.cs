using UnityEngine;

public class EnemySaveID : MonoBehaviour
{
    [Tooltip("Must be unique per enemy. If empty, auto-generated from scene + name + initial position.")]
    public string id;

    private void Awake()
    {
        if (!string.IsNullOrEmpty(id)) return;

        Vector3 p = transform.position;
        id = $"{gameObject.scene.name}:{gameObject.name}:{p.x:F2},{p.y:F2},{p.z:F2}";
    }
}
