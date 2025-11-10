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
    private Coroutine currentDialogueFade; // cache dialogue fades
    private bool introFinished = false;

    // optional: cache FreeCamera on gameplay camera to keep it disabled during intro
    private FreeCamera cachedFreeCam;

    void Start()
    {
        if (introCamera)
        {
            introCamera.gameObject.SetActive(true);
            introCamera.targetDisplay = 0; // Display 1
        }

        if (gameplayCamera)
        {
            cachedFreeCam = gameplayCamera.GetComponent<FreeCamera>();
            if (cachedFreeCam) cachedFreeCam.enabled = false; // no player cam movement during intro
            gameplayCamera.gameObject.SetActive(false);
            gameplayCamera.targetDisplay = 0; // Display 1
        }

        if (player) player.SetActive(false);
        if (fullScreenFadeCanvasGroup) fullScreenFadeCanvasGroup.alpha = 1f;

        if (postProcessVolume) postProcessVolume.profile.TryGet(out depthOfField);

        if (introCamera && path)
        {
            introCamera.transform.position = path.GetPoint(0f);
            introCamera.transform.LookAt(path.GetPoint(lookAheadAmount));
        }

        introCoroutine = StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro()
    {
        // Phase 1: Initial Black Screen
        if (dialogueEntries.Count > 0)
        {
            dialogueText.text = dialogueEntries[0].text;
            currentDialogueFade = StartCoroutine(FadeCanvasGroup(dialogueCanvasGroup, 0f, 1f, 1f));
        }
        yield return new WaitForSeconds(initialHoldDuration);

        if (dialogueEntries.Count > 0)
        {
            if (currentDialogueFade != null) StopCoroutine(currentDialogueFade);
            yield return FadeCanvasGroup(dialogueCanvasGroup, 1f, 0f, 0.5f);
        }

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
            {
                var entry = dialogueEntries[dialogueIndex];
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

        // 1) Put the player at the spawn BEFORE activation so it doesn't update at old position
        if (player)
        {
            player.transform.position = spawnPoint.position;
            player.transform.rotation = spawnPoint.rotation;
        }

        // 2) Activate the player
        if (player) player.SetActive(true);

        // 3) Ensure gameplay camera renders (turn on first), then enable any free-cam
        if (gameplayCamera)
        {
            gameplayCamera.gameObject.SetActive(true);
            gameplayCamera.enabled = true; // ensure Camera component is on
        }
        if (cachedFreeCam) cachedFreeCam.enabled = true;

        // 4) Let gameplay camera render one frame before disabling intro camera
        yield return null;

        // 5) Now disable the intro camera (avoids "No cameras rendering")
        if (introCamera) introCamera.gameObject.SetActive(false);

        // 6) Fade back from black
        if (fullScreenFadeCanvasGroup)
            yield return FadeCanvasGroup(fullScreenFadeCanvasGroup, 1f, 0f, initialFadeDuration);

        if (postProcessVolume) postProcessVolume.enabled = false;
        introFinished = true;
        Debug.Log("Intro Complete!");
        if (tutorialManager != null){
            tutorialManager.StartTutorial();
        }
    }

    IEnumerator TransitionToGameplay()
    {
        // Put player at spawn BEFORE activation
        if (player)
        {
            player.transform.position = spawnPoint.position;
            player.transform.rotation = spawnPoint.rotation;
        }

        // Activate player
        if (player) player.SetActive(true);

        // Make sure gameplay camera actually renders
        if (gameplayCamera)
        {
            gameplayCamera.gameObject.SetActive(true);
            gameplayCamera.enabled = true;
        }
        if (cachedFreeCam) cachedFreeCam.enabled = true;

        // Allow one frame with gameplay camera on
        yield return null;

        // Disable intro camera and fade back
        if (introCamera) introCamera.gameObject.SetActive(false);
        if (fullScreenFadeCanvasGroup)
            yield return FadeCanvasGroup(fullScreenFadeCanvasGroup, 1f, 0f, initialFadeDuration);

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
