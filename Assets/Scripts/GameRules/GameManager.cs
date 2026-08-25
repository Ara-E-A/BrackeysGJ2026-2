using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PaperReq req;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.req = new PaperReq();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void initPaperReqs()
    {
        
    }
}
