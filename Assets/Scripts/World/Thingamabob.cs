using UnityEngine;

public class Thingamabob : MonoBehaviour
{
	[SerializeField] private Thing.type spawnType = Thing.type.Note;
	[SerializeField] [Range(0f, 100f)] private float spawnChance = 100f;
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
		GameObject spawned = Instantiate(prefab, transform.position, transform.rotation);
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
		cube.transform.SetPositionAndRotation(transform.position, transform.rotation);
		cube.tag = "Clickable";
		Thing thing = cube.AddComponent<Thing>();
		thing.thingType = spawnType;
		Rigidbody rigidbody = cube.AddComponent<Rigidbody>();
		rigidbody.useGravity = true;
		return cube;
	}
}