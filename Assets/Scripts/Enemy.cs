using UnityEngine;

public class Enemy : MonoBehaviour
{
    private int hp = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damageAmount)
    {
        hp -= damageAmount;
        Debug.LogWarning($"Taken damage: {damageAmount}, hp: {hp}");
        if(hp <= 0)
        {
            Debug.LogError("Died");
            Destroy(gameObject);
        }
    }
}
