using UnityEngine;

public class ShowAfterIntro : MonoBehaviour
{
    public IntroSequenceController intro;

    // If for some reason `intro` is null, we can fall back to a fixed delay.
    public float fallbackDelay = 15f;   // seconds (set to your intro length if needed)

    private CanvasGroup canvasGroup;
    private float timer = 0f;
    private bool shown = false;

    void Awake()
    {
        // Make sure we have a CanvasGroup – add one if missing
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    void Start()
    {
        // Hide UI at start (but keep GameObject active!)
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    void Update()
    {
        if (shown) return;

        bool shouldShow = false;

        // Preferred: use the IntroFinished flag
        if (intro != null)
        {
            if (intro.IntroFinished)
                shouldShow = true;
        }
        else
        {
            // Fallback: use a simple timer if no IntroSequenceController reference
            timer += Time.deltaTime;
            if (timer >= fallbackDelay)
                shouldShow = true;
        }

        if (shouldShow)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            shown = true;
            enabled = false; // stop checking
        }
    }
}
