using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraFollow : MonoBehaviour
{
    public float smooth = 10f;   // minél nagyobb, annál szorosabban követ
    public Tilemap map;          // a pálya tilemapje (elhagyható)

    Transform target;
    Camera cam;
    float desiredSize;           // az Orthographic Size, amit te beállítottál

    void Awake()
    {
        cam = GetComponent<Camera>();
        desiredSize = cam.orthographicSize;
        if (map != null) map.CompressBounds();
    }

    void LateUpdate()
    {
        // megkeressük a saját játékosunkat
        if (target == null)
        {
            var nm = NetworkManager.Singleton;
            if (nm == null || !nm.IsClient || nm.LocalClient == null || nm.LocalClient.PlayerObject == null)
                return;
            target = nm.LocalClient.PlayerObject.transform;
        }

        Vector3 pos = target.position;

        if (map != null)
        {
            Bounds b = map.localBounds;
            Vector3 min = map.transform.TransformPoint(b.min);
            Vector3 max = map.transform.TransformPoint(b.max);

            // a kamera képe ne lehessen nagyobb a pályánál
            float maxSize = Mathf.Min((max.y - min.y) / 2f, (max.x - min.x) / (2f * cam.aspect));
            cam.orthographicSize = Mathf.Min(desiredSize, maxSize);

            float halfH = cam.orthographicSize;
            float halfW = halfH * cam.aspect;

            pos.x = Mathf.Clamp(pos.x, min.x + halfW, max.x - halfW);
            pos.y = Mathf.Clamp(pos.y, min.y + halfH, max.y - halfH);
        }

        pos.z = transform.position.z;   // a kamera Z-je maradjon (2D-ben általában -10)
        transform.position = Vector3.Lerp(transform.position, pos, smooth * Time.deltaTime);
    }
}