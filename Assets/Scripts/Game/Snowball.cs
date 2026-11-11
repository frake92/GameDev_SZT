using Unity.Netcode;
using UnityEngine;

public class Snowball : NetworkBehaviour
{
    public float speed = 6f;
    public int damage = 5;

    private Vector2 direction;
    private ulong shooterId;

    // A dobó hívja meg közvetlenül az Instantiate után
    public void Launch(Vector2 dir, ulong shooterId)
    {
        direction = dir.normalized;
        this.shooterId = shooterId;
    }
    public override void OnNetworkSpawn()
    {
        if (IsServer) Invoke(nameof(Remove), 3f);   // 3 mp után eltűnik
    }

    void Update()
    {
        if(!IsServer) return;   // a mozgást a szerver végzi, a NetworkTransform szinkronizálja
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return;
        // a saját dobó nem sebezheti magát
        if (other.TryGetComponent(out Player player) && player.OwnerClientId != shooterId)
        {
            player.TakeDamage(damage);
            Remove();
        }
        if (other.CompareTag("Wall")) { Remove(); return; }
    }

    void Remove()
    {
        if (IsSpawned) NetworkObject.Despawn();
    }
}