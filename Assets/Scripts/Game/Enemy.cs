using UnityEngine;

public class Enemy : MonoBehaviour
{
    private int hp = 10;

    public float speed = 1.5f;
    public GameObject snowballPrefab;
    public float throwInterval = 2f;   // ennyi másodpercenként dobhat (hógolyója mindig van)

    private Player player;
    private float throwTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<Player>();
        throwTimer = throwInterval;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.transform.position);
        throwTimer -= Time.deltaTime;

        // 1. szabály: ha észreveszi a játékost, de még messze van -> közelít
        if (distance < 10f && distance > 3f)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        }

        // 2. szabály: ha dobótávolságban van és letelt az idő -> hógolyót dob
        if (throwTimer <= 0f)
        {
            Vector2 dir = player.transform.position - transform.position;
            GameObject ball = Instantiate(snowballPrefab, transform.position, Quaternion.identity);
            //ball.GetComponent<Snowball>().Launch(dir, false);
            throwTimer = throwInterval;
        }
    }

    public void TakeDamage(int damageAmount)
    {
        hp -= damageAmount;
        Debug.LogWarning($"Taken damage: {damageAmount}, hp: {hp}");
        if (hp <= 0)
        {
            Debug.LogError("Died");
            Destroy(gameObject);
        }
    }
}