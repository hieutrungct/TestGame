using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    [Header("References (can auto-assign if left empty)")]
    public CharacterController controller;
    public Transform cam;                // transform của camera chính
    public Animator animator;

    [Header("Movement")]
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float turnSmoothTime = 0.1f;

    [Header("Jump & Gravity")]
    public float jumpHeight = 1.6f;
    public float gravity = -9.81f;
    public Transform groundCheck;        // empty object đặt gần chân
    public float groundDistance = 0.15f;
    public LayerMask groundMask;         // tick layer "Ground" trong Inspector

    // internal
    float turnSmoothVelocity;
    Vector3 velocity;
    bool isWalking;
    bool isRunning;
    bool isGrounded;

    void Start()
    {
        // tự gán nếu người dùng quên
        if (controller == null) controller = GetComponent<CharacterController>();
        if (cam == null && Camera.main != null) cam = Camera.main.transform;

        // safety: nếu groundCheck chưa gán, tạo tạm một điểm ở chân
        if (groundCheck == null)
        {
            GameObject g = new GameObject("groundCheck_temp");
            g.transform.parent = transform;
            g.transform.localPosition = new Vector3(0f, 0.1f, 0f);
            groundCheck = g.transform;
        }
    }

    void Update()
    {
        // --- Ground check ---
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0f)
        {
            // ensure small downward to keep controller grounded
            velocity.y = -2f;
        }

        // --- Input ---
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;

        isWalking = inputDir.magnitude >= 0.1f;
        isRunning = isWalking && Input.GetKey(KeyCode.LeftShift);

        // --- Rotation & Movement ---
        if (isWalking)
        {
            // direction relative to camera
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + (cam ? cam.eulerAngles.y : 0f);
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            float speed = isRunning ? runSpeed : walkSpeed;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        // --- Jump ---
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // v = sqrt(2 * g * h) (g negative)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            if (animator != null) animator.SetTrigger("Jump");
        }

        // --- Gravity ---
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // --- Animator updates (if assigned) ---
        if (animator != null)
        {
            animator.SetBool("isWalking", isWalking);
            animator.SetBool("isRunning", isRunning);
            animator.SetBool("isGrounded", isGrounded);
            // animator.SetTrigger("Jump") đã gọi khi nhảy
        }
    }

    // visualize groundCheck sphere
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}
