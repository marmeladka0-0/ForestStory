using UnityEngine;

public class NPCInteractable : Interactable2D
{
    [Header("Dialog settings")]
    [SerializeField]
    private string[] dialogueLines = new string[] {
        "Hello!",
        "Today is a good day."
    };

    private int currentLineIndex = 0;

    //ovverride our abstract function
    public override void Interact()
    {
        if (dialogueLines.Length == 0) return;

        //write it on console for now
        Debug.Log($"<b>[{objectName}]</b>: {dialogueLines[currentLineIndex]}");

        currentLineIndex++;
        if (currentLineIndex >= dialogueLines.Length)
        {
            currentLineIndex = 0;
        }
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other); // WTF the hint is, ai is crazy
        currentLineIndex = 0;        // all replics on the start again
    }
}