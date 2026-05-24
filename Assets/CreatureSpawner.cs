using UnityEngine;

public class CreatureSpawner : MonoBehaviour
{
    [Header("References")]
    public CreatureMovement creaturePrefab; 
    public Transform[] spawnPoints;

    [Header("Settings")]
    public float spawnInterval = 3f;
    public CreatureMovement.CurveType defaultCurveStyle = CreatureMovement.CurveType.QuadraticArc; 
    public float curveOffset = 5f; 
    
    [Header("Creature Global Settings")]
    public float creatureSpeed = 4f; 

    [Header("Line Visuals")]
    public Material pathMaterial; 
    public float lineWidth = 0.2f;

    private float nextSpawnTime;
    private Transform playerTransform;
    private LineRenderer[] pathRenderers; // One for each spawn point

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        SetupLineRenderers();
    }

    void SetupLineRenderers()
    {
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        pathRenderers = new LineRenderer[spawnPoints.Length];

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i] == null) continue;

            // Create an empty child game object to hold this specific track line
            GameObject lineObj = new GameObject($"TrackLine_{spawnPoints[i].name}");
            lineObj.transform.SetParent(this.transform);

            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
            lr.positionCount = 31;
            lr.useWorldSpace = true;
            
            if (pathMaterial != null)
            {
                lr.material = pathMaterial;
            }
            else
            {
                lr.material = new Material(Shader.Find("Sprites/Default"));
            }

            pathRenderers[i] = lr;
        }
    }

    void Update()
    {
        // Continuously update ALL lines in real-time tracking the player
        DrawAllVisibleTracks();

        if (Time.time >= nextSpawnTime)
        {
            SpawnCreature();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    void DrawAllVisibleTracks()
    {
        if (spawnPoints == null || playerTransform == null || pathRenderers == null) return;

        int segments = 30;

        for (int p = 0; p < spawnPoints.Length; p++)
        {
            if (spawnPoints[p] == null || pathRenderers[p] == null) continue;

            Vector3 start = spawnPoints[p].position;
            Vector3 end = playerTransform.position;
            
            // Hover lines slightly above floor (y=0.1) to completely eliminate Z-Fighting texture clipping
            start.y = 0.1f; 
            end.y = 0.1f;

            Vector3 dir = (end - start).normalized;
            Vector3 perpendicular = Vector3.Cross(dir, Vector3.up).normalized;

            // Alternate directions: Left spawn offsets left, Right spawn offsets right automatically
            float dynamicOffset = curveOffset;
            if (spawnPoints[p].name.ToLower().Contains("left") || p % 2 == 0)
            {
                dynamicOffset = -curveOffset; 
            }

            Vector3 sideOffset = perpendicular * dynamicOffset;

            for (int i = 0; i <= segments; i++)
            {
                float t = i / (float)segments;
                Vector3 pointOnPath;

                if (defaultCurveStyle == CreatureMovement.CurveType.QuadraticArc) 
                {
                    Vector3 midPoint = Vector3.Lerp(start, end, 0.5f);
                    Vector3 controlPoint = midPoint + sideOffset;
                    pointOnPath = Bezier.GetQuadraticPoint(start, controlPoint, end, t);
                }
                else
                {
                    Vector3 firstThird = Vector3.Lerp(start, end, 0.33f);
                    Vector3 secondThird = Vector3.Lerp(start, end, 0.66f);
                    Vector3 cp1 = firstThird + sideOffset;
                    Vector3 cp2 = secondThird - sideOffset;
                    pointOnPath = Bezier.GetCubicPoint(start, cp1, cp2, end, t);
                }

                pathRenderers[p].SetPosition(i, pointOnPath);
            }
        }
    }

    void SpawnCreature()
    {
        if (creaturePrefab == null || spawnPoints.Length == 0 || playerTransform == null) return;

        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform chosenSpawn = spawnPoints[randomIndex];
        if (chosenSpawn == null) return;

        CreatureMovement newCreature = Instantiate(creaturePrefab, chosenSpawn.position, Quaternion.identity);
        
        newCreature.pathType = defaultCurveStyle;
        
        float dynamicOffset = curveOffset;
        if (chosenSpawn.name.ToLower().Contains("left") || randomIndex % 2 == 0)
        {
            dynamicOffset = -curveOffset;
        }
        newCreature.sideOffsetDistance = dynamicOffset; 
        newCreature.moveSpeed = creatureSpeed; 
        
        newCreature.InitializePath(chosenSpawn.position, playerTransform);
    }
}