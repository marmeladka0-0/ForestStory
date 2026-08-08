using UnityEngine;

// a few informarion about monoBehaviour
// https://www.youtube.com/watch?v=t9lkekE4_vk
public class CharacterController2D : MonoBehaviour
{
    public int characterID; // 1 - grandfather, 2 - granddaughter
    // SerializedField private variable, but visible in unity redactor
    // https://www.youtube.com/watch?v=INWP96nNg_0
    [SerializeField] public float moveSpeed = 3.5f;
    [SerializeField] private float stoppingDistance = 0.25f;

    private Vector3 targetPosition;
    private SpriteRenderer spriteRenderer;
    private bool isSelected = false;

    private Rigidbody2D rb;

    // Переменные для отслеживания застревания в стенах/НПС
    private Vector2 lastPosition;
    private float stuckTimer = 0f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        targetPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        lastPosition = rb.position;
    }

    //character has an event listner to change skin (if selected)
    //group - to move the correct one
    //Start listening events
    void OnEnable()
    {
        EventManager.OnCharacterSelected += OnSelectionChanged;
    }

    //Stop it
    void OnDisable()
    {
        EventManager.OnCharacterSelected -= OnSelectionChanged;
    }

    void Update()
    {
        // В Update оставили воспроизведение звука шагов
        float distance = Vector2.Distance(rb.position, targetPosition);

        if (distance > stoppingDistance)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayFootstep();
            }
        }
    }

    void FixedUpdate()
    {
        // Перемещение физического Rigidbody вынесено в FixedUpdate
        float distance = Vector2.Distance(rb.position, targetPosition);

        if (distance > stoppingDistance)
        {
            Vector2 newPos = Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);

            // ДЕТЕКТОР ЗАСТРЕВАНИЯ:
            // Если персонаж пытается идти, но физика его не пускает (уперся в НПС/стену)
            if (Vector2.Distance(rb.position, lastPosition) < 0.005f)
            {
                stuckTimer += Time.fixedDeltaTime;

                // Если стоим на месте дольше 0.25 сек — останавливаемся
                if (stuckTimer > 0.25f)
                {
                    targetPosition = rb.position;
                    rb.linearVelocity = Vector2.zero;
                    stuckTimer = 0f;
                }
            }
            else
            {
                stuckTimer = 0f; // Движемся нормально — сбрасываем таймер
            }
        }
        else
        {
            // КЛЮЧЕВОЙ МОМЕНТ: Принудительно гасим любую оставшуюся скорость при остановке
            rb.linearVelocity = Vector2.zero;
            stuckTimer = 0f;
        }

        lastPosition = rb.position;
    }

    //To set new target point
    public void SetTarget(Vector3 newTarget)
    {
        targetPosition = newTarget;
        stuckTimer = 0f;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ResetStepTimer();
        }
    }

    //change character selection
    private void OnSelectionChanged(int selectedID)
    {
        if (spriteRenderer == null) return;

        isSelected = (selectedID == characterID);

        //change color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = isSelected ? Color.yellow : Color.white;
        }
    }

    private void OnMouseDown()
    {
        //send the signal
        if (isSelected)
        {
            //if 2 are selected => change 2 to unselected
            EventManager.OnCharacterSelected?.Invoke(0);
        }
        else
        {
            //if not => select one of two
            EventManager.OnCharacterSelected?.Invoke(characterID);
        }
    }
}


//[ INITIALIZATION ]
// │
// ▼
// 1. Awake() <-- Called always when the object is created (even if the script is disabled)
// │
// ▼
// 2. OnEnable() <-- Called every time the script or object is enabled
// │
// ▼
// 3. Start() <-- Called once before the first Update frame (the script must be enabled)
// │
// ▼
//[ GAME LOOP (Physics & Game Logic) ]
// │
// 4. FixedUpdate() <-- Called a fixed number of times per second (for Rigidbody physics)
// │
// 5. Update() <-- Called every frame (for logic, player input, timers)
// │
// 6. LateUpdate() <-- Called every frame AFTER Update (for camera movement following a character)
// │
// ▼
//[ DEACTIVATION & DESTRUCTION (Decommissioning) ]
// │
// 7. OnDisable() <-- Called when the script or object is disabled
// │
// ▼
// 8. OnDestroy() <-- Called once before the object is completely removed from memory