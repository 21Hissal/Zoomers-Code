using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed, acceleration, jumpStrength, wallSlideSpeed;
    public int amountOfJumpsInAir = 1;
    int jumpsLeft;

    [HideInInspector]
    public int movementDirection = 1;

    public float fallMultiplier = 2.5f, lowJumpMultiplier = 2;

    public float dashMultiplier, dashDuration, dashCooldown;
    public bool dashing, canDash;

    public float groundPoundSpeed;
    bool groundPounding;

    bool isOnWall;
    public Transform wallCheck;
    public Vector3 wallCheckRadius;

    bool isOnGround;
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask whatIsGround;

    bool facingRight;

    public bool startingRight;

    bool ableToResetSpeed;

    [HideInInspector]
    public bool canMove, hasInput, hasForce;

    Rigidbody2D rb;

    PlayerInputActions playerActions;

    [HideInInspector]
    public UnityEvent useItem, stopUsingItem;

    [HideInInspector]
    public bool forceLowJump;
    bool lowJumping;

    [HideInInspector]
    public Quaternion aim;

    public Vector2 spawnPos;

    Animator anim;

    float movementSpeedhelper;
    float accelerationHelper;
    float groundPoundSpeedHelper;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        PlayerInputActions playerInputActions = new PlayerInputActions();
        playerActions = playerInputActions;
    }

    private void Start()
    {
        movementSpeedhelper = movementSpeed;
        accelerationHelper = acceleration;
        groundPoundSpeedHelper = groundPoundSpeed;

        transform.position = spawnPos;

        if (CouchPartyManager.Instance.slowMode)
        {
            movementSpeed *= 0.7f;
            acceleration *= 0.7f;
            groundPoundSpeed *= 0.8f;
        }

        //playerActions.Player.Jump.performed += Jump;
        //playerActions.Player.Jump.canceled += ctx => { lowJumping = true; };

        //playerActions.Player.Dash.performed += ctx => { if (canDash) StartCoroutine(Dash(ctx)); };

        //playerActions.Player.GroundPound.performed += ctx => { groundPounding = true; if (!isOnGround && !isOnWall) StartCoroutine(GroundPound()); };
        //playerActions.Player.GroundPound.canceled += ctx => { groundPounding = false; };

        //playerActions.Player.UseItem.performed += UseItem;
        //playerActions.Player.UseItem.canceled += StopUsingItem;


        //playerActions.Player.Aim.performed += Aim;

        forceLowJump = false;
        facingRight = true;
        isOnWall = false;
        dashing = false;
        canDash = true;
        ableToResetSpeed = true;
        groundPounding = false;
        canMove = false;
        hasInput = true;
        hasForce = true;

        if (!startingRight)
        {
            Flip();
        }

        StartCoroutine(ResetMovementRestriction(1, true, true, true));
    }

    void Update()
    {
        if (GameManager.Instance != null)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                GameManager.Instance.Invoke("ReloadScene", 0.3f);
            }
            else if (Input.GetKeyUp(KeyCode.R))
            {
                GameManager.Instance.CancelInvoke("ReloadScene");
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameManager.Instance.LoadScene("Menu");
            }

            if (transform.position.y < -20)
            {
                GameManager.Instance.ReloadScene();
            }
        }

        if (isOnGround)
        {
            jumpsLeft = amountOfJumpsInAir;
        }
        else if (isOnWall)
        {
            jumpsLeft = amountOfJumpsInAir;
        }

    }

    private void FixedUpdate()
    {
        isOnGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        //isOnWall = Physics2D.OverlapBox(wallCheck.position, wallCheckRadius, 0, whatIsGround);
        isOnWall = Physics2D.OverlapCapsule(wallCheck.position, wallCheckRadius, CapsuleDirection2D.Vertical, 0, whatIsGround);

        float xVelocity = Mathf.Abs(rb.velocity.x);
        float negativeXVelocity = xVelocity * -1;

        if (movementDirection == 1 && rb.velocity.x > 0 || movementDirection == -1 && rb.velocity.x < 0)
        {
            anim.SetFloat("movementSpeed", xVelocity / 2.5f);
        }
        else
        {
            anim.SetFloat("movementSpeed", negativeXVelocity / 5);
        }

        if (canMove && hasForce)
        {
            if (rb.velocity.y < 0)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
            }
            else if (rb.velocity.y > 0 && lowJumping || rb.velocity.y > 0 && forceLowJump)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
            }

            if (movementDirection == 1 && rb.velocity.x < movementDirection * movementSpeed || movementDirection == -1 && rb.velocity.x > movementDirection * movementSpeed)
            {
                rb.AddForce(new Vector2(movementDirection * acceleration * 33, 0));
            }

            if (!dashing && !groundPounding)
            {
                rb.AddForce(new Vector2(movementDirection * acceleration, 0));
            }
            else if (dashing)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }

            if (isOnWall)
            {
                rb.velocity = new Vector2(rb.velocity.x, wallSlideSpeed);
            }
        }
        else if (!canMove)
        {
            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezePosition;
        }     
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        if (canMove && hasInput)
        {
            if (ctx.performed)
            {
                lowJumping = false;
                forceLowJump = false;

                if (isOnWall)
                {
                    WallJump();
                }
                else if (isOnGround || jumpsLeft > 0)
                {
                    if (!isOnGround)
                    {
                        jumpsLeft -= 1;
                    }

                    rb.velocity = new Vector2(rb.velocity.x, 0);
                    rb.AddForce(new Vector2(0, jumpStrength), ForceMode2D.Impulse);
                }
            }
            else if (ctx.canceled)
            {
                lowJumping = true;
            }
        }
    }

    void WallJump()
    {
        Flip();

        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(new Vector2(0, jumpStrength), ForceMode2D.Impulse);

        ResetSpeed();
    }

    public void StartGroundPound(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !isOnGround && !isOnWall && canMove && hasInput)
        {
            groundPounding = true;
            StartCoroutine(GroundPound());
        }
        else if (ctx.canceled)
        {
            groundPounding = false;
        }
    }

    IEnumerator GroundPound()
    {
        while (groundPounding && !isOnGround && !isOnWall)
        {
            hasForce = false;
            rb.velocity = new Vector2(0, groundPoundSpeed * -1);
            yield return null;
        }

        rb.velocity = Vector2.zero;
        hasForce = true;

        yield return null;
    }

    public void StartDash(InputAction.CallbackContext ctx)
    {
        if (canDash && ctx.performed && canMove && hasInput)
        {
            StartCoroutine(Dash());
        }
    }

    public IEnumerator Dash()
    {
        dashing = true;
        canDash = false;

        rb.velocity = new Vector2(rb.velocity.x * dashMultiplier, 0);

        yield return new WaitForSeconds(dashDuration);

        dashing = false;
        rb.velocity = new Vector2(rb.velocity.x / (dashMultiplier - 0.1f), rb.velocity.y);

        yield return new WaitForSeconds(dashCooldown - dashDuration);

        canDash = true;
    }

    public void Aim(InputAction.CallbackContext ctx)
    {
        //if (playerActions.KeyboardMouseScheme.Equals(true))
        //{
        //    print("Keyboard");

        //    Vector3 v3 = ctx.ReadValue<Vector2>();
        //    Vector3 dir = v3 - Camera.main.WorldToScreenPoint(transform.position);
        //    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        //    aim = Quaternion.AngleAxis(angle, Vector3.forward);
        //}
        //else if (playerActions.ControllerScheme.Equals(true))
        //{
        //    print("Controller");

        //    Vector3 dir = ctx.ReadValue<Vector2>();
        //    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        //    aim = Quaternion.AngleAxis(angle, Vector3.forward);
        //}

        if (ctx.ReadValue<Vector2>().y > 1 || ctx.ReadValue<Vector2>().x > 1)
        {
            Vector3 v3 = ctx.ReadValue<Vector2>();
            Vector3 dir = v3 - Camera.main.WorldToScreenPoint(transform.position);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            aim = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else
        {
            Vector3 dir = ctx.ReadValue<Vector2>();
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            aim = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    public void Flip()
    {
        movementDirection *= -1;

        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;

        if (GetComponent<PlayerItemManager>().item != null)
        {
            GetComponent<PlayerItemManager>().item.Flip();
        }

        TextMeshPro numberText = GetComponentInChildren<TextMeshPro>();
        Vector3 numberScaler = numberText.transform.localScale;
        numberScaler.x *= -1;
        numberText.rectTransform.localScale = numberScaler;
    }

    bool usedItem;
    public void UseItem(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && canMove && hasInput)
        {
            useItem.Invoke();
            usedItem = true;
        }
        else if (ctx.canceled && usedItem)
        {
            usedItem = false;
            stopUsingItem.Invoke();
        }
    }

    //void StopUsingItem(InputAction.CallbackContext ctx)
    //{
    //    stopUsingItem.Invoke();
    //}

    void ResetSpeed()
    {
        if (ableToResetSpeed)
        {
            rb.velocity = new Vector2(movementDirection * movementSpeed, rb.velocity.y);
        }
    }

    public void ForceLowJump()
    {
        forceLowJump = true;
        StartCoroutine(ResetForceLowJump());
    }

    public IEnumerator ResetForceLowJump()
    {
        yield return new WaitForSeconds(0.05f);
        yield return new WaitUntil(() => rb.velocity.y < 0);
        forceLowJump = false;
    }

    public void ResetMovementRestrictions(float time, bool setHasForce, bool setHasInput, bool setCanMove)
    {
        StartCoroutine(ResetMovementRestriction(time, setHasForce, setHasInput, setCanMove));
    }

    IEnumerator ResetMovementRestriction(float time, bool setHasForce, bool setHasInput, bool setCanMove)
    {
        yield return new WaitForSeconds(time);

        if (setHasForce)
        {
            hasForce = true;
            print("hasforce");
        }            
        if (setHasInput)
        {
            hasInput = true;
            print("hasinput");
        }           
        if (setCanMove)
        {
            canMove = true;
            print("canmove");
        }

        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        transform.eulerAngles = Vector3.zero;
    }

    public void SetMode(bool slowMode)
    {
        if (slowMode)
        {
            movementSpeed *= 0.7f;
            acceleration *= 0.7f;
            groundPoundSpeed *= 0.8f;

            rb.velocity = new Vector2(movementSpeed * movementDirection, rb.velocity.y);
        }
        else
        {
            movementSpeed = movementSpeedhelper;
            acceleration = accelerationHelper;
            groundPoundSpeed = groundPoundSpeedHelper;
        }
    }

    private void OnEnable()
    {
        playerActions.Player.Enable();
    }

    private void OnDisable()
    {
        playerActions.Player.Disable();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(wallCheck.position, wallCheckRadius);
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
