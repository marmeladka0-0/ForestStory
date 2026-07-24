using UnityEngine;

public class GroupController : MonoBehaviour
{
    [SerializeField] private CharacterController2D grandfather;
    [SerializeField] private CharacterController2D granddaughter;

    private int selectedCharacter = 0; // 0 - оба, 1 - дед, 2 - внучка

    void Update()
    {
        // 1. Переключение по ПКМ
        if (Input.GetMouseButtonDown(1))
        {
            selectedCharacter = (selectedCharacter + 1) % 3;
            // Отправляем сигнал всем: "Выбран персонаж № X!"
            EventManager.OnCharacterSelected?.Invoke(selectedCharacter);
        }

        // 2. Движение по ЛКМ
        if (Input.GetMouseButtonDown(0))
        {
            HandleMapClick();
        }

    }

    void HandleMapClick()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        // Проверяем, куда попал клик
        Collider2D hit = Physics2D.OverlapPoint(mousePos);

        if (hit != null)
        {
            // Если кликнули по самому персонажу — ничего не делаем (он сам выберет себя через OnMouseDown)
            if (hit.GetComponent<CharacterController2D>() != null)
            {
                return;
            }

            //тут еще будет если клик был по нпс
        }

        MoveGroupTo(mousePos); //если по пустому месту то двигаем
    }

    public void MoveGroupTo(Vector3 point)
    {
        // Если выбран ТОЛЬКО Дедушка (ID = 1)
        if (selectedCharacter == 1)
        {
            if (grandfather != null)
                grandfather.SetTarget(point);
        }
        // Если выбрана ТОЛЬКО Внучка (ID = 2)
        else if (selectedCharacter == 2)
        {
            if (granddaughter != null)
                granddaughter.SetTarget(point);
        }
        // Если выбраны ОБА (ID = 0)
        else
        {
            // Идут вместе в одну точку (или с небольшим смещением)
            if (grandfather != null)
                grandfather.SetTarget(point + new Vector3(-0.6f, 0, 0));

            if (granddaughter != null)
                granddaughter.SetTarget(point + new Vector3(0.6f, 0, 0));
        }
    }

    void OnEnable()
    {
        // Слушаем, когда кто-то изменил выбор (например, через клик мышкой по персонажу)
        EventManager.OnCharacterSelected += UpdateSelectedCharacter;
    }

    void OnDisable()
    {
        EventManager.OnCharacterSelected -= UpdateSelectedCharacter;
    }

    // Этот метод обновляет переменную selectedCharacter внутри TeamController!
    private void UpdateSelectedCharacter(int newSelectedID)
    {
        selectedCharacter = newSelectedID;
    }
}
