using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemySaveID))]
[RequireComponent(typeof(EnemyHealth))]
public class EnemySaveState : MonoBehaviour
{
    private EnemySaveID saveId;
    private EnemyHealth health;
    private NavMeshAgent agent;

    private void Awake()
    {
        saveId = GetComponent<EnemySaveID>();
        health = GetComponent<EnemyHealth>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        // Only restore if we actually have a save
        if (!SaveSystem.HasSave()) return;

        string id = saveId.id;

        // If this enemy has saved data, apply it
        if (!SaveSystem.HasEnemyData(id)) return;

        bool alive = SaveSystem.GetEnemyAlive(id);
        if (!alive)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 pos = SaveSystem.GetEnemyPosition(id, transform.position);
        Quaternion rot = SaveSystem.GetEnemyRotation(id, transform.rotation);
        float hp = SaveSystem.GetEnemyHealth(id, health.maxHealth);

        // NavMeshAgent-safe reposition
        if (agent != null && agent.isOnNavMesh)
        {
            agent.Warp(pos);
            transform.rotation = rot;
        }
        else
        {
            transform.position = pos;
            transform.rotation = rot;
        }

        health.SetHealthSilently(hp);
    }

    public void SaveNow()
    {
        if (saveId == null) return;

        SaveSystem.SaveEnemy(
            saveId.id,
            transform.position,
            transform.rotation,
            health.GetCurrentHealth(),
            health.IsDead == false
        );
    }
}
