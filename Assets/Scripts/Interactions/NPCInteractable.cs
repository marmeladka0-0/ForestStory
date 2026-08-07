using UnityEngine;

public class NPCInteractable : Interactable2D
{
    [Header("NPC Data")]
    [SerializeField] private Sprite avatarSprite; // Картинка дедушки/внучки

    [Header("Dialog settings")]
    [SerializeField] private string[] topics = new string[] { "Спросить про ключ", "Узнать про рецепт", "Попрощаться" };
    [SerializeField]
    private string[] dialogueLines = new string[] {
        "Hello!",
        "Today is a good day."
    };

    [Header("UI Link")]
    [SerializeField] private UI_DialogueWindow dialogueWindow;

    private int currentLineIndex = 0;

    protected override void Start()
    {
        // Вызываем Start из базового класса Interactable2D, чтобы не сломать его инициализацию
        base.Start();

        // Наша доп. логика: если в инспекторе забыли привязать окно — находим его автоматически
        if (dialogueWindow == null)
        {
            dialogueWindow = FindFirstObjectByType<UI_DialogueWindow>();
        }
    }

    public override void Interact()
    {
        if (dialogueLines.Length == 0) return;

        // Если окна всё ещё нет на сцене — выводим ошибку в консоль
        if (dialogueWindow == null)
        {
            Debug.LogError($"[NPCInteractable] Окно UI_DialogueWindow не привязано к {gameObject.name} и не найдено на сцене!");
            return;
        }

        // Показываем диалог
        string currentLine = dialogueLines[currentLineIndex];
        dialogueWindow.OpenDialogue(avatarSprite, objectName, currentLine, topics);

        // Переходим к следующей строке для следующего нажатия
        currentLineIndex++;
        if (currentLineIndex >= dialogueLines.Length)
        {
            currentLineIndex = 0;
        }
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);

        // Закрываем окно ТОЛЬКО если от НПС отошел именно Игрок
        if (other.CompareTag("Player"))
        {
            currentLineIndex = 0;

            if (dialogueWindow != null)
            {
                dialogueWindow.CloseDialogue();
            }
        }
    }
}