using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    public GameObject dialoguePanel; // Сама UI панель
    public TextMeshProUGUI dialogueText; // Текст реплики

    private void Awake()
    {
        // Выключаем панель сразу при запуске игры!
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    //будет для диалогов

}
