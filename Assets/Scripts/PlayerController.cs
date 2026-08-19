using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float fallDeathY = -15f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float groundCheckDistance = 0.6f;
    [SerializeField] private LayerMask pulpitLayer = ~0; 

    private CharacterController controller;
    private float verticalVelocity;
    private float moveSpeed = 3f;

    private int lastScoredPulpitId = -1;
    private bool hasFallen;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        moveSpeed = ConfigLoader.Instance != null && ConfigLoader.Instance.IsLoaded
            ? ConfigLoader.Instance.Config.player_data.speed
            : 3f;

        lastScoredPulpitId = 0;
    }

    private void Update()
    {
        if (hasFallen) return;

        HandleMovement();
        HandleGroundCheck();
        HandleFallCheck();
    }

    private void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal"); 
        float v = Input.GetAxisRaw("Vertical");   

        Vector3 move = new Vector3(h, 0f, v);
        if (move.sqrMagnitude > 1f) move.Normalize();

        bool grounded = controller.isGrounded;
        verticalVelocity = grounded ? -0.5f : verticalVelocity + gravity * Time.deltaTime;

        Vector3 velocity = move * moveSpeed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);

        if (move.sqrMagnitude > 0.001f)
            transform.forward = move;
    }

    private void HandleGroundCheck()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, groundCheckDistance + 0.1f, pulpitLayer))
        {
            PulpitController pulpit = hit.collider.GetComponentInParent<PulpitController>();
            if (pulpit != null && pulpit.PulpitId != lastScoredPulpitId)
            {
                lastScoredPulpitId = pulpit.PulpitId;
                GameManager.Instance.RegisterSuccessfulMove();
            }
        }
    }

    private void HandleFallCheck()
    {
        if (transform.position.y <= fallDeathY)
        {
            hasFallen = true;
            GameManager.Instance.ReportPlayerFell();
        }
    }
}