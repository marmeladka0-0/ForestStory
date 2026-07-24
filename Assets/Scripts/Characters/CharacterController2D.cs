using UnityEngine;

// чучуть инфа про MonoBehaviour но нам пока не актуально
// https://www.youtube.com/watch?v=t9lkekE4_vk
public class CharacterController2D : MonoBehaviour
{
    public int characterID; // 1 - дедушка, 2 - внучка
    // приватная переменая, но видимая в инспекторе юнити
    // https://www.youtube.com/watch?v=INWP96nNg_0
    [SerializeField] public float moveSpeed = 3.5f;
    
    private Vector3 targetPosition;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        targetPosition = transform.position;
    }

    // Подключаемся к радиоволне при появлении
    void OnEnable()
    {
        EventManager.OnCharacterSelected += OnSelectionChanged;
    }

    // Отключаемся, чтобы не засорять память
    void OnDisable()
    {
        EventManager.OnCharacterSelected -= OnSelectionChanged;
    }

    void Update()
    {
        // Каждая персонажка плавно идет к своей целевой точке
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    // Команда задать новую точку ходьбы
    public void SetTarget(Vector3 newTarget)
    {
        targetPosition = newTarget;
    }

    // Срабатывает АВТОМАТИЧЕСКИ, когда передают смену выбора!
    private void OnSelectionChanged(int selectedID)
    {
        if (spriteRenderer == null) return;

        if (selectedID == characterID)
            spriteRenderer.color = Color.yellow; // Выбран я!
        else
            spriteRenderer.color = Color.white;  // Ходим вместе или выбран другой
    }

    private void OnMouseDown()
    {
        // Отправляем сигнал: "Выбран персонаж с моим ID!"
        EventManager.OnCharacterSelected?.Invoke(characterID);
    }
}


//[ ИНИЦИАЛИЗАЦИЯ (Initialization) ]
//       │
//       ▼
// 1.Awake() < --Вызывается всегда при создании объекта (даже если скрипт выключен)
//       │
//       ▼
// 2. OnEnable()          <-- Вызывается при каждом включении скрипта или объекта
//       │
//       ▼
// 3. Start()             <-- Вызывается один раз перед первым кадром Update (скрипт должен быть включен)
//       │
//       ▼
//[ИГРОВОЙ ЦИКЛ(Physics & Game Logic)]
//       │
// 4.FixedUpdate() < --Вызывается фиксированное число раз в секунду (для физики Rigidbody)
//       │
// 5. Update()            <-- Вызывается каждый кадр (для логики, ввода игрока, таймеров)
//       │
// 6. LateUpdate()        <-- Вызывается каждый кадр ПОСЛЕ Update (для движения камеры за персонажем)
//       │
//       ▼
//[ДЕАКТИВАЦИЯ И УНИЧТОЖЕНИЕ(Decommissioning)]
//       │
// 7.OnDisable() < --Вызывается при выключении скрипта или объекта
//       │
//       ▼
// 8. OnDestroy()         <-- Вызывается один раз перед полным удалением объекта из памяти