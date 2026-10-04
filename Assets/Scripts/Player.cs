using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    public float speed = 0f;

    public Vector2 direction;

    private Rigidbody2D rb;

    private SpriteRenderer sr;

    private int applecounter = 0;

    private int meloncounter = 0;
    bool canattack = false;

    public Enemy enemy;


    //private bool isGrounded;

    //private float jumpVelocity = 12f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    //60 fps-> 60x 120 fps -> 120x
    void Update()
    {
        PlayerMovement();

        
        if(rb != null)
        {
            rb.linearVelocity = direction * speed;
        }
        else
        {
            transform.position += (Vector3)direction * speed * Time.deltaTime;
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
        if(h != 0f || v != 0f)
        {
            speed = 2f;
            direction = new Vector2(h, v).normalized;
        }
        else
        {
            speed = 0f;
            direction = Vector2.zero;
        }

        if (Keyboard.current.eKey.wasPressedThisFrame && canattack)
        {
            Attack();
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Collider2D>())
        {
            if(collision.gameObject.tag != "Enemy")
            {
                if (collision.gameObject.CompareTag("Apple")) applecounter++;
                else if (collision.gameObject.CompareTag("Melon")) meloncounter++;
                //collision.gameObject.SetActive(false);

                Destroy(collision.gameObject);
                Debug.Log(applecounter);
                Debug.Log(meloncounter);
            }
            else canattack = true;
          
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            canattack = false;
        }
    }


    private void Attack()
    {
        enemy.TakeDamage(5);
    }
}
