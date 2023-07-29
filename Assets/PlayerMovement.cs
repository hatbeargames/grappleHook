using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    private float speed = 8f;
    private float jumpingPower = 16f;
    private float longJumpPower = 12f;
    private bool isFacingRight = true;
    private float acceleration = 2;
    private float decceleration = 4;
    private float velPower = 1.5f;
    private float gravityScale = 4f;
    private float gravityMultiplier = .5f;
    private bool doubleJump;
    private bool jump;
    private bool jumpReleased;
    private bool longJump;
    private bool applyFriction;

    private float comboThreshhold = .20f;
    private float frictionAmount = .5f;

    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    private bool isWallSliding;
    private float wallSlidingSpeed = 0f;

    private float minSpeed = 0f;
    private float maxSpeed = 10f;
    private float acceleratePerSecond = .70f;
    private float accelerantBase = .70f;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(8f, 16f);

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform wallCheck2;
    [SerializeField] private LayerMask wallLayer;


    private void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        if ((IsGrounded() || IsWalled()) && !Input.GetButton("Jump"))
        {
            doubleJump = false;
            //if (acceleratePerSecond > accelerantBase) { acceleratePerSecond = accelerantBase; }
        }


        if (IsGrounded() || IsWalled())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }


        if (Input.GetButtonDown("Jump"))
        {
            if (coyoteTimeCounter > 0f || doubleJump)
            {
                //rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                jump = !jump;
                doubleJump = !doubleJump;
                //if (Input.GetButtonDown("Horizontal"))
                //{
                //    Debug.Log("Long Jump true");
                //    longJump = true;
                //}
            }
        }
        if (coyoteTimeCounter > 0 && Mathf.Abs(horizontal) < 0.01f)
        {
            applyFriction = true;
        }
        else
        {
            applyFriction = false;
        }


        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            //rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

            jumpReleased = true;
            coyoteTimeCounter = 0f;
        }

        WallSlide();
        WallJump();

        if (!isWallJumping)
        {
            Flip();
        }

    }

    private void FixedUpdate()
    {
        float targetSpeed = horizontal * speed;
        float speedDif = targetSpeed - rb.velocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);
        float checkRateThreshold;
        if (targetSpeed != 0 && speedDif != 0 && accelRate != 0 && movement != 0)
        {
            checkRateThreshold = (targetSpeed - speedDif) / targetSpeed;
        }
        else { checkRateThreshold = 0; }
        //Debug.Log("targetSpeed: " + targetSpeed + ", speedDif: " + speedDif + ", accelRate: " + accelRate + ", movement: " + movement+", rateThreshold: "+checkRateThreshold);
        if (applyFriction)
        {
            float amount = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(frictionAmount));
            amount *= Mathf.Sign(rb.velocity.x);
            rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }
        else
        {
            rb.AddForce(movement * Vector2.right);
        }
        //if (!isWallJumping)
        //{
        //    rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        //}

        if (jump)
        {
            if (checkRateThreshold < comboThreshhold && targetSpeed != 0)
            {
                //If early in horizontal press, user can long jump.
                rb.velocity = new Vector2((movement * 1) / 2.25f, longJumpPower);
                //Debug.Log(rb.velocity.y);
                //Debug.Log("Long Jump Activated. checkRateThreshold: "+checkRateThreshold);
                //Debug.Log("targetSpeed: " + targetSpeed + ", speedDif: " + speedDif + ", accelRate: " + accelRate + ", movement: " + movement + ", rateThreshold: " + checkRateThreshold);
            }
            else
            {
                //Debug.Log(rb.velocity.y);
                rb.AddForce((Vector2.up * jumpingPower), ForceMode2D.Impulse);
                //Debug.Log("Regular Jump Activated. checkRateThreshold: " + checkRateThreshold);
                //Debug.Log("targetSpeed: " + targetSpeed + ", speedDif: " + speedDif + ", accelRate: " + accelRate + ", movement: " + movement + ", rateThreshold: " + checkRateThreshold);
            }
            jump = false;
        }
        if (isWallJumping)
        {
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;
            isWallJumping = false;
            //Debug.Log(rb.velocity.y);
        }
        if (jumpReleased)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.75f);
            //coyoteTimeCounter = 0f;
            jumpReleased = false;
        }
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = gravityScale * gravityMultiplier;
            //Debug.Log("rb.gravityScale: " + rb.gravityScale);
        }
        else
        {
            rb.gravityScale = gravityScale;
        }
    }

    private bool IsGrounded()
    {
        if (Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer) != null)
        {
            Debug.Log("IsGrounded");
        }
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private bool IsWalled()
    {
        Debug.Log(Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer));
        if (Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer) != null)
        {
            Debug.Log("IsWalled");
            return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
        }
        if (Physics2D.OverlapCircle(wallCheck2.position, 0.2f, wallLayer) != null)
        {
            Debug.Log("IsWalled");
            return Physics2D.OverlapCircle(wallCheck2.position, 0.2f, wallLayer);
        }
        //return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && horizontal != 0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
            if (wallSlidingSpeed < maxSpeed)
            {
                wallSlidingSpeed += acceleratePerSecond * Time.deltaTime;
                if (wallSlidingSpeed > 1 && acceleratePerSecond < 20)
                {
                    acceleratePerSecond += .25f;
                    //Debug.Log("Made it in");
                }
                //Debug.Log("slideSpeed:" + wallSlidingSpeed + "AccelSpeed:" + acceleratePerSecond);
            }
            else
            {
                wallSlidingSpeed = maxSpeed;
                acceleratePerSecond = accelerantBase;
            }
        }
        else
        {
            isWallSliding = false;
            wallSlidingSpeed = minSpeed;
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            //rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            //wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
}