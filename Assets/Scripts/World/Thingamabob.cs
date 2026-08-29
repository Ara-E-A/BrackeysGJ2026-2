using UnityEngine;

public class Thingamabob : MonoBehaviour
{
	[SerializeField] private Thing.type spawnType = Thing.type.Note;
	[SerializeField] [Range(0f, 100f)] private float spawnChance = 100f;
	[SerializeField] private GameObject npcPrefab;

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

		if (spawnType == Thing.type.NPC)
		{
			if (npcPrefab == null)
			{
				Debug.LogWarning("Thingamabob NPC prefab is not assigned. Falling back to a default cube.");
				return SpawnCubeFallback();
			}

			GameObject npc = Instantiate(npcPrefab, transform.position, transform.rotation);
			npc.tag = "Clickable";
			return npc;
		}

		return SpawnCubeFallback();
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