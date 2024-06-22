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

    public EnemyController enemyController;

    private List<AttributesManager> enemiesInContact = new List<AttributesManager>();

    public AttributesManager playerAtm;
    public AttributesManager enemyAtm;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        
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
      
    }
    private void FixedUpdate()
        {
            MovePlayer();
        }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            AttributesManager enemyAtm = other.GetComponent<AttributesManager>();
            if (enemyAtm != null)
            {
                enemiesInContact.Add(enemyAtm);
            }
        }

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
            bonusCheck.SetBonusActive(true);

            yield return new WaitForSeconds(10f);

            moveSpeed = 5f;
            bonusCheck.SetBonusActive(false);
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            AttributesManager enemyAtm = other.GetComponent<AttributesManager>();
            if (enemiesInContact.Contains(enemyAtm))
            {
                enemiesInContact.Remove(enemyAtm);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            foreach (var enemyAtm in enemiesInContact)
            {
                enemyAtm.DealDamage(gameObject);
            }

            if (bonusCheck.isActive)
            {
                playerAtm.DealDamage(other.gameObject);
            }
        }
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