using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    [SerializeField] NetworkObject playerPrefab;
    [SerializeField] Vector2 minPos = new Vector2(-8f, -4f);
    [SerializeField] Vector2 maxPos = new Vector2(8f, 4f);

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        NetworkManager.SceneManager.OnLoadEventCompleted += OnLoadCompleted;
    }

    void OnLoadCompleted(string sceneName, LoadSceneMode mode,
        List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach (ulong clientId in clientsCompleted)
        {
            Vector2 pos = new Vector2(Random.Range(minPos.x, maxPos.x),
                                      Random.Range(minPos.y, maxPos.y));
            NetworkObject player = Instantiate(playerPrefab, pos, Quaternion.identity);
            player.SpawnAsPlayerObject(clientId);
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer && NetworkManager != null)
            NetworkManager.SceneManager.OnLoadEventCompleted -= OnLoadCompleted;
    }
}