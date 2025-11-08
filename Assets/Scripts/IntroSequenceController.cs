using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class DialogueEntry
{
    [Tooltip("Text to display")]
    public string text;
    [Tooltip("Time in seconds to show this dialogue (0 = during initial black screen)")]
    public float displayTime;
    [Tooltip("Duration of the fade transition (seconds)")]
    public float fadeDuration = 0.5f;
}

public class IntroSequenceController : MonoBehaviour
{
    [Header("References")]
    public Camera introCamera;
    public SplinePath path;
    public GameObject player;
    public Transform spawnPoint;
    public Camera gameplayCamera;

    [Header("UI")]
    public TextMeshProUGUI dialogueText;
    public CanvasGroup dialogueCanvasGroup;

    [Header("Full Screen Fade")]
    public CanvasGroup fullScreenFadeCanvasGroup;
    public float initialHoldDuration = 2f;
    public float initialFadeDuration = 1f;
    public float finalFadeDuration = 1f;
    public float finalHoldDuration = 0.5f;

    [Header("Post-Processing")]
    public UnityEngine.Rendering.Volume postProcessVolume;
    private UnityEngine.Rendering.Universal.DepthOfField depthOfField;

    [Header("Timing")]
    public float introDuration = 10f;
    public float lookAheadAmount = 0.05f;
    public List<DialogueEntry> dialogueEntries = new List<DialogueEntry>();

    private Coroutine introCoroutine;

    void Start()
    {
        // Setup
        if (player) player.SetActive(false);
        if (gameplayCamera) gameplayCamera.gameObject.SetActive(false);
        if (introCamera) introCamera.gameObject.SetActive(true);
        if (fullScreenFadeCanvasGroup) fullScreenFadeCanvasGroup.alpha = 1f;

        if (postProcessVolume)
            postProcessVolume.profile.TryGet<UnityEngine.Rendering.Universal.DepthOfField>(out depthOfField);

        if (introCamera && path)
        {
            introCamera.transform.position = path.GetPoint(0f);
            Vector3 lookTarget = path.GetPoint(lookAheadAmount);
            introCamera.transform.LookAt(lookTarget);
        }
    
        introCoroutine = StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro()
{
    Coroutine currentDialogueFade = null;
    
    // Phase 1: Initial Black Screen
    if (dialogueEntries.Count > 0)
    {
        dialogueText.text = dialogueEntries[0].text;
        currentDialogueFade = StartCoroutine(FadeCanvasGroup(dialogueCanvasGroup, 0f, 1f, 1f)); // Quick fade
    }
    
    yield return new WaitForSeconds(initialHoldDuration);
    
    // Fade out initial dialogue
    if (dialogueEntries.Count > 0)
    {
        if (currentDialogueFade != null) StopCoroutine(currentDialogueFade);
        yield return FadeCanvasGroup(dialogueCanvasGroup, 1f, 0f, 0.5f);
    }
    
    // Phase 2: Fade From Black
    if (fullScreenFadeCanvasGroup)
        yield return FadeCanvasGroup(fullScreenFadeCanvasGroup, 1f, 0f, initialFadeDuration);
    
    // Phase 3: Main Camera Path
    float mainElapsed = 0f;
    int dialogueIndex = 1; // Start from second entry
    string lastDialogueText = "";
    
    while (mainElapsed < introDuration)
    {
        mainElapsed += Time.deltaTime;
        float t = mainElapsed / introDuration;
        
        // Camera movement
        Vector3 cameraPos = path.GetPoint(t);
        introCamera.transform.position = cameraPos;
        Vector3 lookTarget = path.GetPoint(Mathf.Clamp01(t + lookAheadAmount));
        introCamera.transform.LookAt(lookTarget);
        
        // Dialogue
        if (dialogueIndex < dialogueEntries.Count)
        {
            var entry = dialogueEntries[dialogueIndex];
            if (mainElapsed >= entry.displayTime && lastDialogueText != entry.text)
            {
                Debug.Log($"Showing dialogue: '{entry.text}' at time {mainElapsed}");
                
                // Fade OUT old (quick, 0.3s)
                if (currentDialogueFade != null) StopCoroutine(currentDialogueFade);
                if (dialogueCanvasGroup.alpha > 0f)
                    yield return FadeCanvasGroup(dialogueCanvasGroup, 1f, 0f, 0.3f);
                
                // Fade IN new
                dialogueText.text = entry.text;
                lastDialogueText = entry.text;
                currentDialogueFade = StartCoroutine(FadeCanvasGroup(dialogueCanvasGroup, 0f, 1f, entry.fadeDuration));
                dialogueIndex++;
            }
        }
        
        // Blur
        if (depthOfField != null)
            depthOfField.focalLength.value = Mathf.Lerp(10f, 150f, t);
        
        yield return null;
    }
    
    // Phase 4: Final Fade to Black
    if (currentDialogueFade != null) StopCoroutine(currentDialogueFade);
    if (dialogueCanvasGroup.alpha > 0f)
        yield return FadeCanvasGroup(dialogueCanvasGroup, 1f, 0f, 0.5f);
    
    if (fullScreenFadeCanvasGroup)
        yield return FadeCanvasGroup(fullScreenFadeCanvasGroup, 0f, 1f, finalFadeDuration);
    
    yield return new WaitForSeconds(finalHoldDuration);
    
    // Phase 5: Transition to gameplay and fade back
    if (introCamera) introCamera.gameObject.SetActive(false);
    if (gameplayCamera) gameplayCamera.gameObject.SetActive(true);
    
    // IMPORTANT: Fade OUT black screen
    if (fullScreenFadeCanvasGroup)
        yield return FadeCanvasGroup(fullScreenFadeCanvasGroup, 1f, 0f, initialFadeDuration);
    
    // Enable player after fade
    if (player)
    {
        player.SetActive(true);
        player.transform.position = spawnPoint.position;
        player.transform.rotation = spawnPoint.rotation;
    }
    
    if (postProcessVolume) postProcessVolume.enabled = false;
    
    Debug.Log("Intro Complete!");
}
    IEnumerator TransitionToGameplay()
    {
        // Switch cameras while screen is black
        if (introCamera) introCamera.gameObject.SetActive(false);
        if (gameplayCamera) gameplayCamera.gameObject.SetActive(true);
        
        // Fade out black screen
        if (fullScreenFadeCanvasGroup)
        {
            yield return FadeCanvasGroup(fullScreenFadeCanvasGroup, 1f, 0f, initialFadeDuration);
        }
        else
        {
            yield return null; // Wait one frame if no fade panel
        }
        
        // Enable player after fade completes
        if (player)
        {
            player.SetActive(true);
            player.transform.position = spawnPoint.position;
            player.transform.rotation = spawnPoint.rotation;
        }
        
        // Disable post-processing
        if (postProcessVolume) postProcessVolume.enabled = false;
        
        Debug.Log("Intro Complete!");
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float startAlpha, float endAlpha, float duration)
    {
        if (cg == null || duration <= 0f) yield break;
        
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / duration);
            yield return null;
        }
        cg.alpha = endAlpha;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (introCoroutine != null)
            {
                StopCoroutine(introCoroutine);
                // Fast transition to gameplay
                StartCoroutine(SkipIntro());
            }
        }
    }

    IEnumerator SkipIntro()
    {
        // Instant black
        if (fullScreenFadeCanvasGroup) fullScreenFadeCanvasGroup.alpha = 1f;
        
        yield return new WaitForSeconds(0.1f); // Tiny pause
        
        yield return StartCoroutine(TransitionToGameplay());
    }
}