using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PaperReq req;
    [SerializeField] private PlayerPaper playerPaper;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.req = new PaperReq();
        playerPaper = new PlayerPaper();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void initPaperReqs()
    {
        
    }
}
