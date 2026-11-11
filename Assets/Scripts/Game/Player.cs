using System.Collections;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour
{

    public float speed = 0f;

    public Vector2 direction;

    public GameObject snowballPrefab;

    private Rigidbody2D rb;

    private SpriteRenderer sr;

    //private int applecounter = 0;

    //private int meloncounter = 0;

    public float dashSpeed = 8f;
    public float dashTime = 0.15f;      
    public float dashCooldown = 1f; 

    private Vector2 dashDirection;
    private float dashTimer = 0f;
    private float nextDashTime = 0f;


    //EZ KELL (ANIMÁCIÓ)
    private Animator animator;



    //private bool isGrounded;

    //private float jumpVelocity = 12f;


    NetworkVariable<int> hp = new NetworkVariable<int>(10);
    NetworkVariable<bool> hasSnowball = new NetworkVariable<bool>(false);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    public override void OnNetworkSpawn()
    {
        // mások karakterét a NetworkTransform mozgatja, nem a fizika
        if (!IsOwner) rb.bodyType = RigidbodyType2D.Kinematic;
    }

    // Update is called once per frame
    //60 fps-> 60x 120 fps -> 120x
    void Update()
    {
        if (!IsOwner) return;

        PlayerMovement();

        Vector2 velocity = direction * speed;

        // Dash közben a dash sebessége érvényes
        if (dashTimer > 0f)
        {
            dashTimer -= Time.deltaTime;
            velocity = dashDirection * dashSpeed;
        }

        if (rb != null)
        {
            rb.linearVelocity = velocity;
        }
        else
        {
            transform.position += (Vector3)velocity * Time.deltaTime;
        }

        //isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.15f);
    }

    void PlayerMovement()
    {
        float h = 0f;
        float v = 0f;
        if (Keyboard.current.dKey.isPressed)
        {
            h = 1f;
            sr.flipX = false;
        }
        if (Keyboard.current.aKey.isPressed)
        {
            h = -1f;
            sr.flipX = true;
        }
        if (Keyboard.current.wKey.isPressed)
        {
            v += 1f;
        }
        else if (Keyboard.current.sKey.isPressed)
        {
            v -= 1f;
        }
        if (h != 0f || v != 0f)
        {
            animator.SetFloat("Speed", 1f);
            speed = 2f;
            direction = new Vector2(h, v).normalized;
        }
        else
        {
            animator.SetFloat("Speed", 0f);
            speed = 0f;
            direction = Vector2.zero;
        }

        // Dash: Space (csak mozgás közben, abba az irányba, amerre megyünk)
        if (Keyboard.current.spaceKey.wasPressedThisFrame && direction != Vector2.zero && Time.time >= nextDashTime)
        {
            dashDirection = direction;
            dashTimer = dashTime;
            nextDashTime = Time.time + dashCooldown;
        }

        // Hógolyó dobás: E vagy bal egérgomb, az egér irányába
        if ((Keyboard.current.eKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame) && hasSnowball.Value)
        {
            Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 aimDirection = mouseWorld - (Vector2)transform.position;
            ThrowServerRpc(aimDirection);
        }
        /*
        if(Keyboard.current.wKey.wasPressedThisFrame && isGrounded)
        {
            if(rb != null)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);
            }
            else
            {
                transform.position += Vector3.up * (jumpVelocity * 0.1f);
            }
        }
        */
    }


    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Apple")) applecounter++;
        else if (collision.CompareTag("Melon")) meloncounter++;
        else return;

        Destroy(collision.gameObject);
        Debug.Log(applecounter);
        Debug.Log(meloncounter);
    }
    */

    [ServerRpc]
    void ThrowServerRpc(Vector2 aimDirection)
    {
        if (!hasSnowball.Value) return;

        GameObject ball = Instantiate(snowballPrefab, transform.position, Quaternion.identity);
        ball.GetComponent<Snowball>().Launch(aimDirection, OwnerClientId);
        ball.GetComponent<NetworkObject>().Spawn();
        hasSnowball.Value = false;
    }

    // Hógolyó felvétel: csak akkor, ha még nincs nálunk
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!IsServer || hasSnowball.Value || !collision.CompareTag("SnowballPickup")) return;
        if (!collision.TryGetComponent(out NetworkObject pickup) || !pickup.IsSpawned) return;

        hasSnowball.Value = true;
        pickup.Despawn();
        FindFirstObjectByType<SnowballSpawner>().Spawn();
    }
    IEnumerator HitAnim()
    {
        animator.SetBool("Damage", true);
        yield return new WaitForSeconds(0.3f);
        animator.SetBool("Damage", false);
    }

    public void TakeDamage(int damageAmount)
    {
        if (!IsServer) return;
        hp.Value -= damageAmount;
        HitClientRpc();
        if (hp.Value <= 0) Debug.LogError("Player died");
    }

    [ClientRpc]
    void HitClientRpc()
    {
        StartCoroutine(HitAnim());
    }


}