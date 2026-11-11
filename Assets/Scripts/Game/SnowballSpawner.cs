using Unity.Netcode;
using UnityEngine;

public class SnowballSpawner : MonoBehaviour
{
    public GameObject pickupPrefab;
    public Vector2 minPos = new Vector2(-8f, -4f);   // a pálya két sarka
    public Vector2 maxPos = new Vector2(8f, 4f);

    void Start()
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer) return;
        for (int i = 0; i < 10; i++) Spawn();
    }

    // A Player hívja, ha felvett egy hógolyót
    public void Spawn()
    {
        Vector2 pos = new Vector2(Random.Range(minPos.x, maxPos.x), Random.Range(minPos.y, maxPos.y));
        GameObject pickup = Instantiate(pickupPrefab, pos, Quaternion.identity);
        pickup.GetComponent<NetworkObject>().Spawn();
    }
}