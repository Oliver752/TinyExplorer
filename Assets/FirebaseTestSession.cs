using UnityEngine;

public class FirebaseTestSession : MonoBehaviour
{
    private bool started;

    void Update()
    {
        // Press S to start a session
        if (!started && Input.GetKeyDown(KeyCode.S))
        {
            if (FirebaseGameAnalytics.Instance != null)
            {
                FirebaseGameAnalytics.Instance.StartSession();
                started = true;
                Debug.Log("[FirebaseTestSession] Session start requested.");
            }
            else
            {
                Debug.LogWarning("[FirebaseTestSession] No FirebaseGameAnalytics.Instance found.");
            }
        }

        // Press E to end the session
        if (started && Input.GetKeyDown(KeyCode.E))
        {
            if (FirebaseGameAnalytics.Instance != null)
            {
                FirebaseGameAnalytics.Instance.EndSession(true);
                started = false;
                Debug.Log("[FirebaseTestSession] Session end requested.");
            }
        }
    }
}
