using UnityEngine;
using UnityEngine.InputSystem;

// Kretanje žabice: WASD / strelice, Space = skok (u zraku još jedan), Shift = trčanje.
[RequireComponent(typeof(CharacterController))]
public class FrogController : MonoBehaviour
{
    public static FrogController Instance { get; private set; }

    [Header("Kretanje")]
    public float moveSpeed = 6.5f;
    public float sprintSpeed = 9.5f;
    public float turnSpeed = 12f;
    public float groundAcceleration = 60f;
    public float airAcceleration = 14f;

    [Header("Skok")]
    public float jumpHeight = 2.3f;
    public float gravity = -25f;
    public int extraJumps = 1;
    public float coyoteTime = 0.15f;
    public float jumpBuffer = 0.15f;
    public float maxFallSpeed = 30f;

    [Header("Pad")]
    public float respawnY = -1f;

    public Transform cameraTransform;
    public Transform model;

    public bool IsGrappling { get; set; }
    public Vector3 Velocity { get => velocity; set => velocity = value; }
    public bool IsGrounded => controller != null && controller.isGrounded;
    public Transform GroundTransform { get; private set; }
    public Vector3 Wind { get; set; }      // vjetar (npr. boss) – postavlja se svaki frame
    public event System.Action Jumped;     // javlja animaciji da je žabica skočila

    CharacterController controller;
    Vector3 velocity;
    Vector3 spawnPoint;
    float lastGroundedTime = -10f;
    float lastJumpPressTime = -10f;
    int jumpsLeft;
    Transform groundHit;
    bool wasGrounded;
    float stunTimer;
    Vector3 squash = Vector3.one;
    Vector3 modelBaseScale = Vector3.one;

    void Awake()
    {
        Instance = this;
        controller = GetComponent<CharacterController>();
        spawnPoint = transform.position;
        if (cameraTransform == null && Camera.main != null) cameraTransform = Camera.main.transform;
        if (model) modelBaseScale = model.localScale;
    }

    void Update()
    {
        if (GameManager.Paused) return;

        if (transform.position.y < respawnY)
        {
            var health = GetComponent<PlayerHealth>();
            if (health) health.FellOff(); else Respawn();
            return;
        }

        // nosi žabicu ako stoji na pokretnoj platformi
        if (GroundTransform && !IsGrappling)
        {
            var mp = GroundTransform.GetComponentInParent<MovingPlatform>();
            if (mp) controller.Move(mp.Delta);
        }

        if (IsGrappling) { AnimateModel(); return; }

        stunTimer -= Time.deltaTime;
        var kb = Keyboard.current;
        Vector2 input = Vector2.zero;
        bool sprint = false, jumpPressed = false;
        if (kb != null && stunTimer <= 0f && GameManager.InputEnabled)
        {
            if (kb.wKey.isPressed || kb.upArrowKey.isPressed) input.y += 1;
            if (kb.sKey.isPressed || kb.downArrowKey.isPressed) input.y -= 1;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) input.x += 1;
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) input.x -= 1;
            sprint = kb.leftShiftKey.isPressed;
            jumpPressed = kb.spaceKey.wasPressedThisFrame;
        }
        input = Vector2.ClampMagnitude(input, 1f);

        Vector3 forward = cameraTransform ? cameraTransform.forward : Vector3.forward;
        Vector3 right = cameraTransform ? cameraTransform.right : Vector3.right;
        forward.y = 0; right.y = 0;
        forward.Normalize(); right.Normalize();
        Vector3 move = forward * input.y + right * input.x;

        bool grounded = controller.isGrounded;
        if (grounded)
        {
            if (!wasGrounded && velocity.y < -8f) squash = new Vector3(1.3f, 0.7f, 1.3f);
            lastGroundedTime = Time.time;
            jumpsLeft = extraJumps;
            if (velocity.y < 0) velocity.y = -2f;
        }

        Vector3 targetHorizontal = move * (sprint ? sprintSpeed : moveSpeed);
        Vector3 horizontal = new Vector3(velocity.x, 0, velocity.z);
        float accel = grounded ? groundAcceleration : airAcceleration;
        horizontal = Vector3.MoveTowards(horizontal, targetHorizontal, accel * Time.deltaTime);
        velocity.x = horizontal.x;
        velocity.z = horizontal.z;

        if (jumpPressed) lastJumpPressTime = Time.time;
        if (Time.time - lastJumpPressTime <= jumpBuffer)
        {
            if (Time.time - lastGroundedTime <= coyoteTime)
            {
                Jump(false);
                lastGroundedTime = -10f;
            }
            else if (jumpsLeft > 0 && jumpPressed)
            {
                jumpsLeft--;
                Jump(true);
            }
        }

        velocity.y = Mathf.Max(velocity.y + gravity * Time.deltaTime, -maxFallSpeed);

        groundHit = null;
        controller.Move((velocity + Wind) * Time.deltaTime);
        Wind = Vector3.zero;
        GroundTransform = controller.isGrounded ? groundHit : null;
        wasGrounded = grounded;

        if ((controller.collisionFlags & CollisionFlags.Above) != 0 && velocity.y > 0) velocity.y = 0;

        if (move.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
        }

        AnimateModel();
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.normal.y > 0.6f) groundHit = hit.collider.transform;
    }

    void AnimateModel()
    {
        if (!model) return;
        squash = Vector3.Lerp(squash, Vector3.one, 10f * Time.deltaTime);
        model.localScale = Vector3.Scale(modelBaseScale, squash);
    }

    void Jump(bool air)
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        lastJumpPressTime = -10f;
        squash = new Vector3(0.8f, 1.25f, 0.8f);
        Sfx.Play(air ? "jump2" : "jump", 0.4f);
        Jumped?.Invoke();
    }

    public void Bounce(float height)
    {
        velocity.y = Mathf.Sqrt(height * -2f * gravity);
        jumpsLeft = extraJumps;
        lastGroundedTime = -10f;
        squash = new Vector3(0.75f, 1.3f, 0.75f);
    }

    public void Knockback(Vector3 from, float force)
    {
        var grapple = GetComponent<TongueGrapple>();
        if (grapple) grapple.Cancel();
        Vector3 dir = transform.position - from;
        dir.y = 0;
        if (dir.sqrMagnitude < 0.01f) dir = -transform.forward;
        dir.Normalize();
        velocity = dir * force + Vector3.up * 6f;
        stunTimer = 0.35f;
    }

    public void SetCheckpoint(Vector3 position) => spawnPoint = position;

    public void Respawn()
    {
        IsGrappling = false;
        var grapple = GetComponent<TongueGrapple>();
        if (grapple) grapple.Cancel();
        controller.enabled = false;
        transform.position = spawnPoint;
        controller.enabled = true;
        velocity = Vector3.zero;
        GroundTransform = null;
    }
}
