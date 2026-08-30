using UnityEngine;

/// <summary>
/// Spawns one world-readable <see cref="Thing"/> - a Sign, a Screen or a Note - and places it
/// against the surface it belongs on:
///
///   Sign / Screen  -> wall-mounted. A short cardinal probe finds a <c>Wall</c>-tagged
///                     collider; the Thing gets a Y rotation snapped to that wall's
///                     orientation (N/S/E/W -> 0/90/180/270) and is slid out along the wall
///                     normal until its bounding box clears the wall.
///   Note           -> ground. A downward ray finds the floor; the Note lies flat on it
///                     (optional random spin) and is lifted until it clears the floor.
///
/// Optional per-axis position jitter is available for both cases and is always re-projected
/// back onto the surface so a jittered Thing still sits flush. Analogous to
/// <see cref="Thingamabob"/> (NPCs); this one is unparented, tags the instance
/// <c>Clickable</c> and ensures a <see cref="Thing"/> component for <see cref="Interactor"/>.
/// Probing is cardinal-only because the room is axis-aligned.
/// </summary>
public class ThingSpawner : MonoBehaviour
{
    private enum ThingKind { Sign, Screen, Note }

    [Tooltip("Pick a random kind (Sign / Screen / Note) at spawn instead of using the field below.")]
    [SerializeField] private bool randomizeKind = true;
    [SerializeField] private ThingKind kind = ThingKind.Note;
    [SerializeField] [Range(0f, 100f)] private float spawnChance = 100f;

    [Header("Prefabs")]
    [SerializeField] private GameObject signPrefab;
    [SerializeField] private GameObject screenPrefab;
    [SerializeField] private GameObject notePrefab;

    [Header("Surface probing")]
    [Tooltip("How far to probe around the spawn point for a wall (Sign / Screen).")]
    [SerializeField] private float wallProbeDistance = 2f;
    [Tooltip("How far to probe downward for the floor (Note, and the wall fallback).")]
    [SerializeField] private float groundProbeDistance = 5f;
    [SerializeField] private string wallTag = "Wall";
    [Tooltip("Add 180 to the wall rotation when the prefab's readable face points -Z.")]
    [SerializeField] private bool flipWallFacing = false;
    [Tooltip("Gap kept between the Thing's bounding box and the surface, so it never clips in.")]
    [SerializeField] private float surfaceClearance = 0.02f;

    [Header("Optional position randomization")]
    [Tooltip("Wall Things: random slide along the wall - x = sideways, y = vertical (metres, +/-).")]
    [SerializeField] private Vector2 wallJitter = Vector2.zero;
    [Tooltip("Ground Things: random offset radius on the floor (metres).")]
    [SerializeField] private float groundJitterRadius = 0f;
    [Tooltip("Ground Things: also spin them to a random yaw.")]
    [SerializeField] private bool randomizeNoteYaw = true;

    private void Start()
    {
        if (Random.value * 100f > spawnChance)
        {
            return;
        }

        if (randomizeKind)
        {
            kind = (ThingKind)Random.Range(0, System.Enum.GetValues(typeof(ThingKind)).Length);
        }

        GameObject prefab = kind switch
        {
            ThingKind.Sign => signPrefab,
            ThingKind.Screen => screenPrefab,
            _ => notePrefab,
        };

        if (prefab == null)
        {
            Debug.LogWarning($"ThingSpawner: no prefab assigned for {kind}.", this);
            return;
        }

        bool wantsWall = kind != ThingKind.Note;

        if (wantsWall && TryFindWall(out Vector3 wallPoint, out Vector3 wallNormal))
        {
            JitterOnWall(ref wallPoint, ref wallNormal);
            SpawnOnSurface(prefab, wallPoint, wallNormal, WallRotation(wallNormal));
        }
        else if (TryFindGround(transform.position, out Vector3 groundPoint, out Vector3 groundNormal))
        {
            JitterOnGround(ref groundPoint, ref groundNormal);
            SpawnOnSurface(prefab, groundPoint, groundNormal, GroundRotation(groundNormal));
        }
        else
        {
            SpawnOnSurface(prefab, transform.position, Vector3.up, GroundRotation(Vector3.up));
        }
    }

    // ---------------- surface probing ----------------

    // Nearest Wall-tagged collider found by casting the four cardinal directions.
    private bool TryFindWall(out Vector3 point, out Vector3 normal)
    {
        point = transform.position;
        normal = Vector3.forward;

        Vector3 origin = transform.position;
        Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };

        float nearest = float.MaxValue;
        bool found = false;

        foreach (Vector3 direction in directions)
        {
            if (Physics.Raycast(origin, direction, out RaycastHit hit, wallProbeDistance)
                && hit.collider.CompareTag(wallTag)
                && hit.distance < nearest)
            {
                nearest = hit.distance;
                point = hit.point;
                normal = hit.normal;
                found = true;
            }
        }

        return found;
    }

    private bool TryFindGround(Vector3 near, out Vector3 point, out Vector3 normal)
    {
        if (Physics.Raycast(near + Vector3.up, Vector3.down, out RaycastHit hit, groundProbeDistance + 1f))
        {
            point = hit.point;
            normal = hit.normal;
            return true;
        }

        point = near;
        normal = Vector3.up;
        return false;
    }

    // ---------------- optional jitter (re-projected onto the surface) ----------------

    private void JitterOnWall(ref Vector3 point, ref Vector3 normal)
    {
        if (wallJitter == Vector2.zero)
        {
            return;
        }

        Vector3 tangent = Vector3.Cross(Vector3.up, normal);
        if (tangent.sqrMagnitude < 1e-4f)
        {
            return;
        }
        tangent.Normalize();

        Vector3 candidate = point
            + tangent * Random.Range(-wallJitter.x, wallJitter.x)
            + Vector3.up * Random.Range(-wallJitter.y, wallJitter.y);

        // Re-cast into the wall so a jittered Thing stays flush (and never leaves the wall).
        if (Physics.Raycast(candidate + normal * 0.5f, -normal, out RaycastHit hit, 1f)
            && hit.collider.CompareTag(wallTag))
        {
            point = hit.point;
            normal = hit.normal;
        }
    }

    private void JitterOnGround(ref Vector3 point, ref Vector3 normal)
    {
        if (groundJitterRadius <= 0f)
        {
            return;
        }

        Vector2 disc = Random.insideUnitCircle * groundJitterRadius;
        Vector3 candidate = point + new Vector3(disc.x, 0f, disc.y);

        if (TryFindGround(candidate, out Vector3 jitteredPoint, out Vector3 jitteredNormal))
        {
            point = jitteredPoint;
            normal = jitteredNormal;
        }
    }

    // ---------------- rotation ----------------

    // Local +Z faces along the wall normal (into the room), snapped to the nearest 90.
    private Quaternion WallRotation(Vector3 wallNormal)
    {
        Vector3 flat = new(wallNormal.x, 0f, wallNormal.z);
        if (flat.sqrMagnitude < 1e-4f)
        {
            return Quaternion.identity;
        }

        float yaw = Mathf.Atan2(flat.x, flat.z) * Mathf.Rad2Deg;
        yaw = Mathf.Round(yaw / 90f) * 90f;
        if (flipWallFacing)
        {
            yaw += 180f;
        }

        return Quaternion.Euler(0f, yaw, 0f);
    }

    // Flat on the surface (floor normal is +Y, so this is just an optional random spin).
    private Quaternion GroundRotation(Vector3 groundNormal)
    {
        float yaw = randomizeNoteYaw ? Random.Range(0f, 360f) : 0f;
        return Quaternion.FromToRotation(Vector3.up, groundNormal) * Quaternion.Euler(0f, yaw, 0f);
    }

    // ---------------- spawn + clip-free placement ----------------

    private void SpawnOnSurface(GameObject prefab, Vector3 surfacePoint, Vector3 surfaceNormal, Quaternion rotation)
    {
        GameObject spawned = Instantiate(prefab, surfacePoint + surfaceNormal * 0.5f, rotation);
        spawned.tag = "Clickable";

        if (!spawned.TryGetComponent(out Thing thing))
        {
            thing = spawned.AddComponent<Thing>();
        }
        thing.thingType = kind == ThingKind.Note ? Thing.type.Note : Thing.type.Screen;

        PushClearOfSurface(spawned, surfacePoint, surfaceNormal);
    }

    // Slide the instance along the surface normal until its bounding box clears the surface.
    private void PushClearOfSurface(GameObject spawned, Vector3 surfacePoint, Vector3 surfaceNormal)
    {
        Bounds bounds;
        if (spawned.TryGetComponent(out Collider col))
        {
            bounds = col.bounds;
        }
        else if (spawned.TryGetComponent(out Renderer rend))
        {
            bounds = rend.bounds;
        }
        else
        {
            spawned.transform.position = surfacePoint + surfaceNormal * surfaceClearance;
            return;
        }

        Vector3 e = bounds.extents;
        float halfDepth = Mathf.Abs(e.x * surfaceNormal.x)
                        + Mathf.Abs(e.y * surfaceNormal.y)
                        + Mathf.Abs(e.z * surfaceNormal.z);

        float centerDistance = Vector3.Dot(bounds.center - surfacePoint, surfaceNormal);
        float correction = halfDepth + surfaceClearance - centerDistance;
        if (correction > 0f)
        {
            spawned.transform.position += surfaceNormal * correction;
        }
    }
}
