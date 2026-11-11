using UnityEngine;

// Az EnemySpawns objektumra kerül: a gyerekei az Enemy snowpile-ok (lehetséges helyek).
public class EnemySnowPileSpawner : MonoBehaviour
{
    public float minInterval = 2f;   // két megjelenés között ennyi mp telik el (véletlenszerűen)
    public float maxInterval = 4f;

    private float timer;

    void Start()
    {
        // Induláskor mind rejtve van, utána egyesével jelennek meg
        foreach (Transform pile in transform) pile.gameObject.SetActive(false);
        timer = Random.Range(minInterval, maxInterval);
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0f) return;
        timer = Random.Range(minInterval, maxInterval);

        // Véletlenszerűen kiválaszt egy helyet, és megjeleníti ott a hókupacot
        transform.GetChild(Random.Range(0, transform.childCount)).gameObject.SetActive(true);
    }
}