using UnityEngine;
using UnityEngine.EventSystems;

public class GroupController : MonoBehaviour
{
    [SerializeField] private CharacterController2D grandfather;
    [SerializeField] private CharacterController2D granddaughter;

    private int selectedCharacter = 0; // 0 - all, 1 - grandfather, 2 - granddaughter

    void Start()
    {
        // Игнорируем столкновения между членами группы, чтобы они не блочили друг друга
        if (grandfather != null && granddaughter != null)
        {
            Collider2D col1 = grandfather.GetComponent<Collider2D>();
            Collider2D col2 = granddaughter.GetComponent<Collider2D>();

            if (col1 != null && col2 != null)
            {
                Physics2D.IgnoreCollision(col1, col2);
            }
        }
    }

    void Update()
    {
        //move characters
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            HandleMapClick();
        }
    }

    void HandleMapClick()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        //Find the place where the click was
        Collider2D hit = Physics2D.OverlapPoint(mousePos);

        if (hit != null)
        {
            //If on character - do nothing (he will handle it himself)
            if (hit.GetComponent<CharacterController2D>() != null)
            {
                return;
            }

            //Add click on npc here
        }

        MoveGroupTo(mousePos); //If empty space - then move
    }

    public void MoveGroupTo(Vector3 point)
    {
        //moving to the target point the character(s) who are selected
        if (selectedCharacter == 1)
        {
            if (grandfather != null)
                grandfather.SetTarget(point);
        }
        else if (selectedCharacter == 2)
        {
            if (granddaughter != null)
                granddaughter.SetTarget(point);
        }
        else
        {
            if (grandfather != null)
                grandfather.SetTarget(point + new Vector3(-0.6f, 1.5f, 0));

            if (granddaughter != null)
                granddaughter.SetTarget(point + new Vector3(0.6f, 1.0f, 0));
        }
    }

    void OnEnable()
    {
        //Listening events, need the one when the character was selected
        EventManager.OnCharacterSelected += UpdateSelectedCharacter;
    }

    void OnDisable()
    {
        EventManager.OnCharacterSelected -= UpdateSelectedCharacter;
    }

    //change variable selectedCharacter inside TeamController!
    private void UpdateSelectedCharacter(int newSelectedID)
    {
        selectedCharacter = newSelectedID;
    }
}
