using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;

    public Transform wallCheck;
    public LayerMask wallLayer;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    private float horizontal;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpPower = 5f;
    private bool isFacingRight = true;

    [SerializeField] private bool isWallSliding;
    public float wallSlidingSpeed = 1.5f;

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);

        if (!isFacingRight && horizontal > 0f)
        {
            Flip();
        }

        else if (isFacingRight && horizontal < 0f)
        {
            Flip();
        }

        if (rb.linearVelocity.y < 0)
        {
            
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }

        WallSlide();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(context.performed && IsGrounded() && !IsWalled())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
        }

        if (context.canceled && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y*0.5f);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
    }

    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && Mathf.Abs(horizontal) > 0f)
        {
            isWallSliding = true;
            // Limit downward velocity
            if (rb.linearVelocity.y < -wallSlidingSpeed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlidingSpeed);
            }

            if (rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            }
        }
        else
        {
            isWallSliding = false;
        }
    }

}
