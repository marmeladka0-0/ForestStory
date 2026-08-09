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

    [SerializeField] private Sprite characterAvatar;
    public Sprite CharacterAvatar => characterAvatar;

    // Variables for tracking getting stuck in walls/NPCs
    private Vector2 lastPosition;
    private float stuckTimer = 0f;

    private NPCInteractable targetNPC;

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
        // Footstep sound playback is kept in Update
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
        float distance = Vector2.Distance(rb.position, targetPosition);

        // CHECK NPC APPROACH
        if (targetNPC != null)
        {
            float distanceToNPC = Vector2.Distance(transform.position, targetNPC.transform.position);

            // If close enough to the NPC
            if (distanceToNPC <= targetNPC.InteractionRadius)
            {
                StopMovement(); // Stop the character

                NPCInteractable npcToInteract = targetNPC;
                targetNPC = null; // IMPORTANT: Nullify before calling Interact to prevent repeated calls!

                npcToInteract.Interact();
            }
        }

        if (distance > stoppingDistance)
        {
            Vector2 newPos = Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);

            // Stuck detector
            if (Vector2.Distance(rb.position, lastPosition) < 0.005f)
            {
                stuckTimer += Time.fixedDeltaTime;
                if (stuckTimer > 0.25f)
                {
                    targetPosition = rb.position;
                    targetNPC = null; // Reset NPC if stuck against an obstacle
                    rb.linearVelocity = Vector2.zero;
                    stuckTimer = 0f;
                }
            }
            else
            {
                stuckTimer = 0f;
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            stuckTimer = 0f;
        }

        lastPosition = rb.position;
    }

    public void StopMovement()
    {
        targetPosition = rb.position;
        rb.linearVelocity = Vector2.zero;
        stuckTimer = 0f;
    }

    // Setting a point target (on regular map clicks)
    public void SetTarget(Vector3 newTarget)
    {
        targetPosition = newTarget;
        targetNPC = null; // Cancel NPC target if player clicks somewhere else
        stuckTimer = 0f;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ResetStepTimer();
        }
    }

    // Setting an NPC target
    public void SetTargetNPC(NPCInteractable npc, Vector3 approachPoint)
    {
        SetTarget(approachPoint); // First reset the old target and timer
        targetNPC = npc;          // AND ONLY THEN assign the new NPC!
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