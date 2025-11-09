using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class DialogueEntry
{
    [Tooltip("Text to display")] public string text;
    [Tooltip("Time in seconds to show this dialogue (0 = during initial black screen)")] public float displayTime;
    [Tooltip("Duration of the fade transition (seconds)")] public float fadeDuration = 0.5f;
}

public class IntroSequenceController : MonoBehaviour
{
    [Header("References")] public Camera introCamera; public SplinePath path; public GameObject player;
    public Transform spawnPoint; public Camera gameplayCamera; public TutorialManager tutorialManager;

    [Header("UI")] public TextMeshProUGUI dialogueText; public CanvasGroup dialogueCanvasGroup;

    [Header("Full Screen Fade")] public CanvasGroup fullScreenFadeCanvasGroup;
    public float initialHoldDuration = 2f, initialFadeDuration = 1f, finalFadeDuration = 1f, finalHoldDuration = 0.5f;

    [Header("Post-Processing")] public UnityEngine.Rendering.Volume postProcessVolume;
    private UnityEngine.Rendering.Universal.DepthOfField depthOfField;

    [Header("Timing")] public float introDuration = 10f; public float lookAheadAmount = 0.05f;
    public List<DialogueEntry> dialogueEntries = new List<DialogueEntry>();

    private Coroutine introCoroutine;
    private Coroutine currentDialogueFade; // <-- NEW: cache dialogue fades
    private bool introFinished = false;

    void Start()
    {
        if (player) player.SetActive(false);
        if (gameplayCamera) gameplayCamera.gameObject.SetActive(false);
        if (introCamera) introCamera.gameObject.SetActive(true);
        if (fullScreenFadeCanvasGroup) fullScreenFadeCanvasGroup.alpha = 1f;

        if (postProcessVolume) postProcessVolume.profile.TryGet(out depthOfField);

        if (introCamera && path)
        { introCamera.transform.position = path.GetPoint(0f);
          introCamera.transform.LookAt(path.GetPoint(lookAheadAmount)); }

        introCoroutine = StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro()
    {
        // Phase 1: Initial Black Screen
        if (dialogueEntries.Count > 0)
        {   dialogueText.text = dialogueEntries[0].text;
            currentDialogueFade = StartCoroutine(FadeCanvasGroup(dialogueCanvasGroup, 0f, 1f, 1f)); }
        yield return new WaitForSeconds(initialHoldDuration);

        if (dialogueEntries.Count > 0)
        {   if (currentDialogueFade != null) StopCoroutine(currentDialogueFade);
            yield return FadeCanvasGroup(dialogueCanvasGroup, 1f, 0f, 0.5f); }

        // Phase 2: Fade From Black
        if (fullScreenFadeCanvasGroup)
            yield return FadeCanvasGroup(fullScreenFadeCanvasGroup, 1f, 0f, initialFadeDuration);

        // Phase 3: Main Camera Path
        float mainElapsed = 0f; int dialogueIndex = 1; string lastDialogueText = "";
        while (mainElapsed < introDuration)
        {
            mainElapsed += Time.deltaTime; float t = mainElapsed / introDuration;

            Vector3 cameraPos = path.GetPoint(t); introCamera.transform.position = cameraPos;
            introCamera.transform.LookAt(path.GetPoint(Mathf.Clamp01(t + lookAheadAmount)));

            // Dialogue
            if (dialogueIndex < dialogueEntries.Count)
            {   var entry = dialogueEntries[dialogueIndex];
                if (mainElapsed >= entry.displayTime && lastDialogueText != entry.text)
                {
                    Debug.Log($"Showing dialogue: '{entry.text}' at time {mainElapsed}");
                    if (currentDialogueFade != null) StopCoroutine(currentDialogueFade);
                    if (dialogueCanvasGroup.alpha > 0f) yield return FadeCanvasGroup(dialogueCanvasGroup, 1f, 0f, 0.3f);
                    dialogueText.text = entry.text; lastDialogueText = entry.text;
                    currentDialogueFade = StartCoroutine(FadeCanvasGroup(dialogueCanvasGroup, 0f, 1f, entry.fadeDuration));
                    dialogueIndex++;
                }
            }

            if (depthOfField != null) depthOfField.focalLength.value = Mathf.Lerp(10f, 150f, t);
            yield return null;
        }

        // Phase 4: Final Fade to Black
        if (currentDialogueFade != null) StopCoroutine(currentDialogueFade);
        if (dialogueCanvasGroup.alpha > 0f) yield return FadeCanvasGroup(dialogueCanvasGroup, 1f, 0f, 0.5f);
        if (fullScreenFadeCanvasGroup) yield return FadeCanvasGroup(fullScreenFadeCanvasGroup, 0f, 1f, finalFadeDuration);
        yield return new WaitForSeconds(finalHoldDuration);

        // Phase 5: Transition to gameplay and fade back
        if (introCamera) introCamera.gameObject.SetActive(false);
        if (gameplayCamera) gameplayCamera.gameObject.SetActive(true);
        if (fullScreenFadeCanvasGroup) yield return FadeCanvasGroup(fullScreenFadeCanvasGroup, 1f, 0f, initialFadeDuration);

        if (player)
        {   player.SetActive(true);
            player.transform.position = spawnPoint.position;
            player.transform.rotation = spawnPoint.rotation;
            yield return null;
            player.transform.position = spawnPoint.position;
            player.transform.rotation = spawnPoint.rotation;
         }

        if (postProcessVolume) postProcessVolume.enabled = false;
        introFinished = true;
        Debug.Log("Intro Complete!");
        if (tutorialManager != null){
            tutorialManager.StartTutorial();
        }
    }

    IEnumerator TransitionToGameplay()
    {
        if (introCamera) introCamera.gameObject.SetActive(false);
        if (gameplayCamera) gameplayCamera.gameObject.SetActive(true);
        if (fullScreenFadeCanvasGroup) yield return FadeCanvasGroup(fullScreenFadeCanvasGroup, 1f, 0f, initialFadeDuration);

        if (player)
        {   player.SetActive(true);
            player.transform.position = spawnPoint.position;
            player.transform.rotation = spawnPoint.rotation; }

        if (postProcessVolume) postProcessVolume.enabled = false;
        Debug.Log("Intro Complete!");
        if (tutorialManager != null){
            tutorialManager.StartTutorial();
        }
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float startAlpha, float endAlpha, float duration)
    {
        if (cg == null || duration <= 0f) yield break;
        float timer = 0f;
        while (timer < duration)
        { timer += Time.deltaTime; cg.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / duration); yield return null; }
        cg.alpha = endAlpha;
    }

    void Update()
    {
        if (introFinished) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (introCoroutine != null)
            {
                // INSTANTLY kill any on-screen dialogue when skipping  <-- NEW
                if (currentDialogueFade != null) StopCoroutine(currentDialogueFade);
                dialogueCanvasGroup.alpha = 0f;

                StopCoroutine(introCoroutine);
                StartCoroutine(SkipIntro());
            }
        }
    }

    IEnumerator SkipIntro()
    {
        if (fullScreenFadeCanvasGroup) fullScreenFadeCanvasGroup.alpha = 1f;
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(TransitionToGameplay());
        introFinished = true;
    }
}