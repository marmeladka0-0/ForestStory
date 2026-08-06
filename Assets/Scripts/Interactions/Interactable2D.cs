using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Interactable2D : MonoBehaviour
{
    [Header("Object settings")]
    [SerializeField] protected string objectName = "I dont know what was here";
    [SerializeField] protected GameObject interactionHint; // we can interact with E button
    //but idk what the hint is, this code need to be refactored

    protected bool isPlayerInRange = false;

    protected virtual void Start()
    {
        //set unactive
        if (interactionHint != null)
        {
            interactionHint.SetActive(false);
        }
    }

    protected virtual void Update()
    {
        //If we are near interactable object and E button is pressed
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    //abstract function for npc, some objects and so on
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