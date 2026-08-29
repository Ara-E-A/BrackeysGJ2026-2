using UnityEngine;

public class Thingamabob : MonoBehaviour
{
	[SerializeField] private Thing.type spawnType = Thing.type.Note;
	[SerializeField] [Range(0f, 100f)] private float spawnChance = 100f;
	[Tooltip("Vertical offset applied to spawned NPCs, relative to this spawner's position.")]
	[SerializeField] private float spawnYOffset = 0f;
	[Tooltip("When on, the spawned object gets a random 0-360 yaw instead of this spawner's rotation.")]
	[SerializeField] private bool randomizeYRotation = false;
	[SerializeField] private GameObject npcPrefab;
	[SerializeField] private GameObject notePrefab;
	[SerializeField] private GameObject screenPrefab;
	[SerializeField] private GameObject tablePrefab;

    public void Start()
    {
        SpawnDefaultCube();
    }

    public GameObject SpawnDefaultCube()
	{
		if (Random.value * 100f > spawnChance)
		{
			return null;
		}

		switch (spawnType)
		{
			case Thing.type.NPC:
				if (npcPrefab == null)
				{
					Debug.LogWarning("Thingamabob NPC prefab is not assigned. Falling back to a default cube.");
					return SpawnCubeFallback();
				}
				return SpawnPrefab(npcPrefab);

			case Thing.type.Note:
				if (notePrefab == null)
				{
					Debug.LogWarning("Thingamabob Note prefab is not assigned. Falling back to a default cube.");
					return SpawnCubeFallback();
				}
				return SpawnPrefab(notePrefab);

			case Thing.type.Screen:
				if (screenPrefab == null)
				{
					Debug.LogWarning("Thingamabob Screen prefab is not assigned. Falling back to a default cube.");
					return SpawnCubeFallback();
				}
				return SpawnPrefab(screenPrefab);

			case Thing.type.Table:
				if (tablePrefab == null)
				{
					Debug.LogWarning("Thingamabob Table prefab is not assigned. Falling back to a default cube.");
					return SpawnCubeFallback();
				}
				return SpawnPrefab(tablePrefab);

			default:
				return SpawnCubeFallback();
		}
	}

	private GameObject SpawnPrefab(GameObject prefab)
	{
		Vector3 spawnPosition = transform.position;
		if (spawnType == Thing.type.NPC)
		{
			spawnPosition.y += spawnYOffset;
		}

		GameObject spawned = Instantiate(prefab, spawnPosition, GetSpawnRotation());
		spawned.tag = "Clickable";
		Thing thing = spawned.GetComponent<Thing>();
		if (thing == null)
		{
			thing = spawned.AddComponent<Thing>();
		}
		thing.thingType = spawnType;
		return spawned;
	}

	private GameObject SpawnCubeFallback()
	{
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.transform.SetPositionAndRotation(transform.position, GetSpawnRotation());
		cube.tag = "Clickable";
		Thing thing = cube.AddComponent<Thing>();
		thing.thingType = spawnType;
		Rigidbody rigidbody = cube.AddComponent<Rigidbody>();
		rigidbody.useGravity = true;
		return cube;
	}

	// Spawner rotation, or a random 0-360 yaw when randomizeYRotation is enabled.
	private Quaternion GetSpawnRotation()
	{
		return randomizeYRotation
			? Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)
			: transform.rotation;
	}
}