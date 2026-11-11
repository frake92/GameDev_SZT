using UnityEngine;

public class EnemySnowPile : MonoBehaviour
{
    public GameObject enemyPrefab;   // a hóember prefab
    public float countdown = 1f;    // ennyi mp-ig áll a hókupac, utána hóember lesz belőle

    private float timer;

    // Minden alkalommal újraindul, amikor a hókupac megjelenik
    void OnEnable()
    {
        timer = countdown;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
        }
    }

    // Ha ráállunk, eltűnik és nem lesz belőle hóember
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>() != null) gameObject.SetActive(false);
    }
}