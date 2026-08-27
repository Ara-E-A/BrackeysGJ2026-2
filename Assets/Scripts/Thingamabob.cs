using UnityEngine;

public class Thingamabob : MonoBehaviour
{
	[SerializeField] private Thing.type spawnType = Thing.type.Note;

    public void Start()
    {
        SpawnDefaultCube();
    }

    public GameObject SpawnDefaultCube()
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