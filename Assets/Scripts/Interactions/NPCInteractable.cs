using UnityEngine;

public class NPCInteractable : Interactable2D
{
    [Header("Настройки диалога")]
    [SerializeField]
    private string[] dialogueLines = new string[] {
        "Привет!",
        "Хороший сегодня день."
    };

    private int currentLineIndex = 0;

    // Переопределяем метод взаимодействия под логику NPC
    public override void Interact()
    {
        if (dialogueLines.Length == 0) return;

        // Пока выводим в консоль
        Debug.Log($"<b>[{objectName}]</b>: {dialogueLines[currentLineIndex]}");

        currentLineIndex++;
        if (currentLineIndex >= dialogueLines.Length)
        {
            currentLineIndex = 0;
        }
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other); // Выполняем базовый отход (прячем подсказку)
        currentLineIndex = 0;        // Сбрасываем реплику на начало
    }
}