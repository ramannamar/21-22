using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Health Settings")]
    public int maxHealth;
    public int currentHealth;
    public HealthBar healthBar;

    private float damageCooldown = 0f;
    private const float damageInterval = 1f;
    private bool isInCombat = false;


    [Header("Key Settings")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("GroundCheck Settings")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 movementDirection;

    Rigidbody rb;

    public BonusCheck bonusCheck;

   
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }   

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        PlayerInput();
        SpeedControl();
        

        if (grounded)
        rb.drag = groundDrag;
        else
        rb.drag = 0;

        if (damageCooldown > 0)
        damageCooldown -= Time.deltaTime;
        
    }
    private void FixedUpdate()
        {
            MovePlayer();
        }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BeastBonus"))
        {
            StartCoroutine(RageMode());
        }
    }

    private IEnumerator RageMode()
    {
        if (bonusCheck != null)
        {
            moveSpeed = 7f;
            bonusCheck.isActive = true;

            yield return new WaitForSeconds(10f);

            moveSpeed = 5f;
            bonusCheck.isActive = false;
        }
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (!isInCombat)
            {
                isInCombat = true;
                StartCoroutine(DamageOverTime());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            isInCombat = false;
            StopCoroutine(DamageOverTime());
            damageCooldown = 0f;
        }
    }

    private IEnumerator DamageOverTime()
    {
        while (isInCombat)
        {
            if (damageCooldown <= 0 && bonusCheck.isActive == false)
            {
                TakeDamage();
                damageCooldown = damageInterval;
            }
            else
            {
                damageCooldown -= Time.deltaTime;
            }

            yield return null;
        }
    }

    private void TakeDamage()
    {
        currentHealth -= 1;
        healthBar.SetHealth(currentHealth);
    }




    private void PlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        movementDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (grounded)
            rb.AddForce(movementDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        else if (!grounded)
            rb.AddForce(movementDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    public void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    public void ResetJump()
    {
        readyToJump = true;
    }
}