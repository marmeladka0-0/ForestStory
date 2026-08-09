using UnityEngine;

public enum SpeakerType
{
    NPC,    // The NPC is speaking
    Player  // The player is speaking (Grandfather or Granddaughter)
}

[System.Serializable]
public struct DialogueLine
{
    [Tooltip("Who speaks this line: NPC or Player")]
    public SpeakerType speaker;

    [TextArea(2, 5)]
    public string text;
}

[System.Serializable]
public struct DialogueTopic
{
    public string topicName;     // Title displayed on the side button (e.g., "Ask about the key")
    public bool isGoodbye;       // If true, selecting this topic will close the dialogue window
    public DialogueLine[] lines; // Lines spoken when this topic is selected
}

public class NPCInteractable : Interactable2D
{
    [Header("NPC Visuals")]
    [SerializeField] private Sprite dialogueArtwork; // Main dialogue artwork (for ArtworkContainer)
    [SerializeField] private Sprite npcAvatar;       // NPC avatar icon for BottomDialogueBox

    [Header("NPC Data")]
    [SerializeField] private float interactionRadius = 1.2f;
    public float InteractionRadius => interactionRadius > 0 ? interactionRadius : 1.2f;

    [Header("Dialogue Settings")]
    [Tooltip("Greeting line spoken by the NPC right when interaction starts")]
    [SerializeField]
    private DialogueLine greetingLine = new DialogueLine
    {
        speaker = SpeakerType.NPC,
        text = "Hello! What would you like to ask?"
    };

    [Tooltip("Topics player can select from side buttons")]
    [SerializeField] private DialogueTopic[] topics;

    [Header("UI Link")]
    [SerializeField] private UI_DialogueWindow dialogueWindow;

    private GroupController groupController;

    protected override void Start()
    {
        base.Start();

        if (dialogueWindow == null)
        {
            dialogueWindow = FindFirstObjectByType<UI_DialogueWindow>();
        }

        groupController = FindFirstObjectByType<GroupController>();
    }

    public override void Interact()
    {
        if (dialogueWindow == null)
        {
            Debug.LogError($"[NPCInteractable] UI_DialogueWindow is not assigned to {gameObject.name}!");
            return;
        }

        // 1. Prepare greeting line data
        DialogueLineData preparedGreeting = PrepareLineData(greetingLine);

        // 2. Extract topic titles for button creation
        string[] topicTitles = new string[topics != null ? topics.Length : 0];
        if (topics != null)
        {
            for (int i = 0; i < topics.Length; i++)
            {
                topicTitles[i] = topics[i].topicName;
            }
        }

        // 3. Open dialogue window with greeting line and callback on topic select
        dialogueWindow.OpenDialogue(dialogueArtwork, preparedGreeting, topicTitles, OnTopicSelected);
    }

    // Called when the player clicks one of the topic buttons
    public void OnTopicSelected(int topicIndex)
    {
        if (topics == null || topicIndex < 0 || topicIndex >= topics.Length) return;

        DialogueTopic selectedTopic = topics[topicIndex];

        // 1. Disable the clicked topic button so it can't be selected again
        dialogueWindow.DisableTopicButton(topicIndex);

        // 2. Handle empty goodbye topic case (closes dialogue immediately)
        if ((selectedTopic.lines == null || selectedTopic.lines.Length == 0) && selectedTopic.isGoodbye)
        {
            dialogueWindow.CloseDialogue();
            return;
        }

        // 3. Prepare line data for all lines in the selected topic
        DialogueLineData[] preparedLines = new DialogueLineData[selectedTopic.lines != null ? selectedTopic.lines.Length : 0];
        if (selectedTopic.lines != null)
        {
            for (int i = 0; i < selectedTopic.lines.Length; i++)
            {
                preparedLines[i] = PrepareLineData(selectedTopic.lines[i]);
            }
        }

        // 4. Send prepared lines and isGoodbye flag to UI
        dialogueWindow.ShowTopicDialogue(preparedLines, selectedTopic.isGoodbye);
    }

    // Helper method to resolve correct avatar based on SpeakerType
    private DialogueLineData PrepareLineData(DialogueLine line)
    {
        Sprite avatarToUse = null;

        if (line.speaker == SpeakerType.NPC)
        {
            avatarToUse = npcAvatar;
        }
        else if (line.speaker == SpeakerType.Player)
        {
            if (groupController != null)
            {
                avatarToUse = groupController.GetActiveCharacterAvatar();
            }
        }

        return new DialogueLineData
        {
            speakerAvatar = avatarToUse,
            text = line.text
        };
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);

        if (other.CompareTag("Player"))
        {
            if (dialogueWindow != null)
            {
                dialogueWindow.CloseDialogue();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, InteractionRadius);
    }
}

// Data structure used to pass dialogue line information to the UI
public struct DialogueLineData
{
    public Sprite speakerAvatar;
    public string text;
}