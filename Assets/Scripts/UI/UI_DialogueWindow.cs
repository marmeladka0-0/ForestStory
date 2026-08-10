using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_DialogueWindow : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image mainArtworkImage;
    [SerializeField] private Image speakerAvatar;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Transform topicButtonsContainer;
    [SerializeField] private Button nextButton;

    [Header("Prefabs")]
    [SerializeField] private GameObject topicButtonPrefab;

    private DialogueLineData[] currentLines;
    private int currentLineIndex = 0;
    private bool isCurrentTopicGoodbye = false;

    private List<UI_TopicButton> spawnedTopicButtons = new List<UI_TopicButton>();

    private void Awake()
    {
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(DisplayNextLine);
        }

        CloseDialogue();
    }

    public void OpenDialogue(Sprite artwork, DialogueLineData greeting, string[] topics, System.Action<int> onTopicSelected)
    {
        gameObject.SetActive(true);

        if (mainArtworkImage != null)
        {
            mainArtworkImage.sprite = artwork;
            mainArtworkImage.gameObject.SetActive(artwork != null);
        }

        isCurrentTopicGoodbye = false;
        currentLines = new DialogueLineData[] { greeting };
        currentLineIndex = 0;

        DisplayCurrentLine();
        BuildTopicButtons(topics, onTopicSelected);
    }

    public void ShowTopicDialogue(DialogueLineData[] lines, bool isGoodbye = false)
    {
        isCurrentTopicGoodbye = isGoodbye;

        if (lines == null || lines.Length == 0)
        {
            if (isGoodbye)
            {
                CloseDialogue();
            }
            return;
        }

        currentLines = lines;
        currentLineIndex = 0;
        DisplayCurrentLine();
    }

    // Disables the pressed topic button
    public void DisableTopicButton(int topicIndex)
    {
        if (topicIndex >= 0 && topicIndex < spawnedTopicButtons.Count)
        {
            if (spawnedTopicButtons[topicIndex] != null)
            {
                spawnedTopicButtons[topicIndex].SetInteractable(false);
            }
        }
    }

    private void DisplayCurrentLine()
    {
        if (currentLines == null || currentLines.Length == 0) return;

        DialogueLineData line = currentLines[currentLineIndex];

        if (speakerAvatar != null)
        {
            speakerAvatar.sprite = line.speakerAvatar;
            speakerAvatar.gameObject.SetActive(line.speakerAvatar != null);
        }

        if (dialogueText != null)
        {
            dialogueText.text = line.text;
        }

        UpdateNextButtonState();
    }

    public void DisplayNextLine()
    {
        if (currentLines == null || currentLines.Length == 0) return;

        // If there is a next line, show it
        if (currentLineIndex < currentLines.Length - 1)
        {
            currentLineIndex++;
            DisplayCurrentLine();
        }
        // If lines are finished and it is a goodbye topic, close the dialogue
        else if (isCurrentTopicGoodbye)
        {
            CloseDialogue();
        }
    }

    // Controls the visibility of the "Next" button
    private void UpdateNextButtonState()
    {
        if (nextButton == null) return;

        bool hasNextLine = currentLines != null && currentLineIndex < currentLines.Length - 1;

        // Button is visible if there is a next line, or if it's a goodbye line (to close on click)
        bool shouldShowNextButton = hasNextLine || isCurrentTopicGoodbye;

        nextButton.gameObject.SetActive(shouldShowNextButton);
    }

    private void BuildTopicButtons(string[] topics, System.Action<int> onTopicSelected)
    {
        spawnedTopicButtons.Clear();

        if (topicButtonsContainer == null) return;

        foreach (Transform child in topicButtonsContainer)
        {
            Destroy(child.gameObject);
        }

        if (topics != null && topicButtonPrefab != null)
        {
            for (int i = 0; i < topics.Length; i++)
            {
                GameObject newButton = Instantiate(topicButtonPrefab, topicButtonsContainer);

                if (newButton.TryGetComponent<UI_TopicButton>(out var topicButtonScript))
                {
                    topicButtonScript.Setup(topics[i], i, onTopicSelected);
                    spawnedTopicButtons.Add(topicButtonScript);
                }
            }
        }
    }

    public void CloseDialogue()
    {
        currentLineIndex = 0;
        currentLines = null;
        isCurrentTopicGoodbye = false;
        spawnedTopicButtons.Clear();
        gameObject.SetActive(false);
    }
}