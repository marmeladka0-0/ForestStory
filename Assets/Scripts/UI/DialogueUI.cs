using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    public GameObject dialoguePanel; //UI panel
    public TextMeshProUGUI dialogueText; //Text of the phrase

    private void Awake()
    {
        //turn of the panel on the start
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    //can be used for dialog or should be deleted

}
