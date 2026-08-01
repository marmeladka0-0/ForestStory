using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Персонажи")]
    [SerializeField] private Transform grandfather;     // Дедушка
    [SerializeField] private Transform granddaughter;  // Внучка

    [Header("Настройки камеры")]
    [SerializeField] private float smoothSpeed = 5f;     // Скорость сглаживания движения
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10f); // Смещение по Z для 2D

    [Header("Границы карты (Bounds)")]
    [SerializeField] private bool useBounds = false;     // Включить ограничения рамок
    [SerializeField] private Vector2 minBounds;          // Нижний левый угол карты (X, Y)
    [SerializeField] private Vector2 maxBounds;          // Верхний правый угол карты (X, Y)

    private int currentSelectedID = 0; // 0 = никто не выбран (следим за обоими)

    private void OnEnable()
    {
        EventManager.OnCharacterSelected += HandleSelectionChanged;
    }

    private void OnDisable()
    {
        EventManager.OnCharacterSelected -= HandleSelectionChanged;
    }

    private void HandleSelectionChanged(int selectedID)
    {
        currentSelectedID = selectedID;
    }

    private void LateUpdate()
    {
        if (grandfather == null || granddaughter == null) return;

        Vector3 targetPosition;

        // 1. Вычисляем целевую позицию камеры
        if (currentSelectedID == 1)
        {
            // Выбран Дедушка (ID 1)
            targetPosition = grandfather.position;
        }
        else if (currentSelectedID == 2)
        {
            // Выбрана Внучка (ID 2)
            targetPosition = granddaughter.position;
        }
        else
        {
            // Никто не выбран (ID 0) — считаем СРЕДНЮЮ ТОЧКУ между двумя персонажами
            targetPosition = (grandfather.position + granddaughter.position) / 2f;
        }

        // Добавляем смещение по Z (обычно -10 для 2D)
        targetPosition += offset;

        // 2. Ограничиваем позицию рамками карты (если включена галочка useBounds)
        if (useBounds)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
        }

        // 3. Плавно перемещаем камеру к целевой точке
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}
