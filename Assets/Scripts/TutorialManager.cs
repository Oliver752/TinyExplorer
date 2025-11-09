using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [Header("UI hivatkozások")]
    public GameObject tutorialPanel;         // pl. TutorialPanel
    public TextMeshProUGUI tutorialText;     // pl. TutorialText (TMP)

    [Header("Időzítések")]
    public float moveToJumpDelay = 3f;        // 3 mp mozgás után jön az ugrás
    public float jumpToInteractDelay = 3f;    // 3 mp ugrás után jön az E
    public float interactVisibleDuration = 3f; // 3 mp-ig látszik az E felirat

    private enum Step
    {
        Move,
        Jump,
        Interact,
        Done
    }

    private Step currentStep = Step.Move;
    private bool isActive = false;
    private bool moveDetected = false;
    private bool jumpDetected = false;
    private Coroutine runningCoroutine;

    void Start()
    {
        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);
    }

    void Update()
    {
        if (!isActive) return;

        switch (currentStep)
        {
            case Step.Move:
                HandleMoveStep();
                break;
            case Step.Jump:
                HandleJumpStep();
                break;
            case Step.Interact:
                // időzítést coroutine intézi
                break;
            case Step.Done:
                break;
        }
    }

    public void StartTutorial()
    {
        isActive = true;
        currentStep = Step.Move;
        moveDetected = false;
        jumpDetected = false;

        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }

        ShowMoveText();
    }

    // --- MOVE STEP ---
    void HandleMoveStep()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (!moveDetected && (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f))
        {
            moveDetected = true;

            if (runningCoroutine != null)
                StopCoroutine(runningCoroutine);

            runningCoroutine = StartCoroutine(MoveToJumpTimer());
        }
    }

    System.Collections.IEnumerator MoveToJumpTimer()
    {
        yield return new WaitForSeconds(moveToJumpDelay);
        currentStep = Step.Jump;
        ShowJumpText();
        runningCoroutine = null;
    }

    // --- JUMP STEP ---
    void HandleJumpStep()
    {
        if (!jumpDetected && Input.GetKeyDown(KeyCode.Space))
        {
            jumpDetected = true;

            if (runningCoroutine != null)
                StopCoroutine(runningCoroutine);

            // ugyanúgy 3 mp-ig marad a jump felirat, mielőtt jön az E
            runningCoroutine = StartCoroutine(JumpToInteractTimer());
        }
    }

    System.Collections.IEnumerator JumpToInteractTimer()
    {
        yield return new WaitForSeconds(jumpToInteractDelay);

        currentStep = Step.Interact;
        ShowInteractText();

        if (runningCoroutine != null)
            StopCoroutine(runningCoroutine);

        runningCoroutine = StartCoroutine(HideAfterInteract());
    }

    // --- INTERACT STEP ---
    System.Collections.IEnumerator HideAfterInteract()
    {
        yield return new WaitForSeconds(interactVisibleDuration);

        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);

        currentStep = Step.Done;
        isActive = false;
        runningCoroutine = null;
    }

    // --- UI feliratok ---
    void ShowMoveText()
    {
        if (tutorialPanel != null) tutorialPanel.SetActive(true);
        if (tutorialText != null)
            tutorialText.text = "Použite šípky alebo klávesy WASD na pohyb";
    }

    void ShowJumpText()
    {
        if (tutorialPanel != null) tutorialPanel.SetActive(true);
        if (tutorialText != null)
            tutorialText.text = "Stlačte SPACE, aby ste skočili";
    }

    void ShowInteractText()
    {
        if (tutorialPanel != null) tutorialPanel.SetActive(true);
        if (tutorialText != null)
            tutorialText.text = "Stlačte E, aby ste zdvihli predmety";
    }
}
