using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 60f;  // koľko zásahov
    private float currentHealth;

    private Animator animator;
    private NavMeshAgent agent;
    private SpiderAI ai;

    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;

        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        ai = GetComponent<SpiderAI>();
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return; // už je mŕtvy

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        // stop AI
        if (agent != null) agent.isStopped = true;
        if (ai != null) ai.enabled = false;

        // spusti animáciu
        if (animator != null)
            animator.SetTrigger("Die");

        // vypneme kolízie aby hráč neostal zaseknutý
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // pavúk ostane ležať navždy → žiaden Destroy
    }
}
