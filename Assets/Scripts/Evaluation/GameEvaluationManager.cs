using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The evaluation loop: the player gets <see cref="maxAttempts"/> tries to submit a valid
/// paper. Each failed submission decrements the count; reaching 0 triggers Game Over, a
/// passing submission triggers Victory.
///
/// The <see cref="Inspector"/> drives this via <see cref="RegisterFailure"/> /
/// <see cref="RegisterSuccess"/>. Hook UI to the UnityEvents. Lazily self-creates so it
/// works with no scene setup.
/// </summary>
public class GameEvaluationManager : MonoBehaviour
{
    private static GameEvaluationManager instance;

    public static GameEvaluationManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<GameEvaluationManager>();
                if (instance == null)
                {
                    instance = new GameObject(nameof(GameEvaluationManager)).AddComponent<GameEvaluationManager>();
                }
            }

            return instance;
        }
    }

    [SerializeField] private int maxAttempts = 3;

    public int AttemptsRemaining { get; private set; }
    public bool Finished { get; private set; }

    [Header("Events")]
    public UnityEvent<int> onAttemptsChanged;
    public UnityEvent onGameOver;
    public UnityEvent onGameVictory;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }

        instance = this;
        AttemptsRemaining = Mathf.Max(1, maxAttempts);
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    /// <summary>A failing paper was submitted: decrement attempts; 0 -> Game Over.</summary>
    public void RegisterFailure()
    {
        if (Finished)
        {
            return;
        }

        AttemptsRemaining = Mathf.Max(0, AttemptsRemaining - 1);
        onAttemptsChanged?.Invoke(AttemptsRemaining);

        if (AttemptsRemaining <= 0)
        {
            Finished = true;
            Debug.Log("GameEvaluationManager: GAME OVER.");
            onGameOver?.Invoke();
        }
    }

    /// <summary>A fully passing paper was submitted: Victory.</summary>
    public void RegisterSuccess()
    {
        if (Finished)
        {
            return;
        }

        Finished = true;
        Debug.Log("GameEvaluationManager: VICTORY.");
        onGameVictory?.Invoke();
    }
}
