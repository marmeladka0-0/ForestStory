using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Interactable2D : MonoBehaviour
{
    [Header("Базовые настройки")]
    [SerializeField] protected string objectName = "Интерактивный объект";
    [SerializeField] protected GameObject interactionHint; // Иконка или текст "[E]"

    protected bool isPlayerInRange = false;

    protected virtual void Start()
    {
        // Прячем подсказку при старте
        if (interactionHint != null)
        {
            interactionHint.SetActive(false);
        }
    }

    protected virtual void Update()
    {
        // Если игрок в зоне и нажал E — взаимодействуем
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    // Этот метод переопределит каждый конкретный объект (NPC, сундук и т.д.)
    public abstract void Interact();

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (interactionHint != null) interactionHint.SetActive(true);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (interactionHint != null) interactionHint.SetActive(false);
        }
    }
}