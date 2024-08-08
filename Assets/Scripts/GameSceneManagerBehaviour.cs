using System.Collections;
using TMPro;
using UnityEngine;

public class GameSceneManagerBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject AsteroidsWave;
    [SerializeField] private GameObject PlayerLife;
    [SerializeField] private TextMeshProUGUI InfoText;
    [SerializeField] private TextMeshProUGUI ScoreText;
    [SerializeField] private TextMeshProUGUI WaveText;
    [SerializeField] private AsteroidWaveState CurrentWaveState;
    [SerializeField] private int Lives = 3;

    private WaveBehaviour AsteroidsWaveBehaviour;
    private Coroutine FlashingTextCoroutine;
    private int WaveNumber = 0;
    private int Score = 0;

    // #####################
    // # Lifecycle Methods #
    // #####################

    private void OnEnable()
    {
        PlayerBehaviour.OnPlayerCreated += OnPlayerCreated;
        PlayerBehaviour.OnPlayerDestroyed += OnPlayerDestroyed;
        WaveBehaviour.OnWaveStateChange += OnAsteroidWaveStateChanged;
        WaveBehaviour.OnAsteroidHitEvent += AddScore;
        WaveBehaviour.OnEnemyShipHitEvent += AddScore;
    }

    private void OnDisable()
    {
        PlayerBehaviour.OnPlayerCreated -= OnPlayerCreated;
        PlayerBehaviour.OnPlayerDestroyed -= OnPlayerDestroyed;
        WaveBehaviour.OnWaveStateChange -= OnAsteroidWaveStateChanged;
        WaveBehaviour.OnAsteroidHitEvent -= AddScore;
        WaveBehaviour.OnEnemyShipHitEvent -= AddScore;
    }   

    void Start()
    {
        AsteroidsWaveBehaviour = AsteroidsWave.GetComponent<WaveBehaviour>();
        UpdateLives();
        Invoke("NextWave", 1.0f);
    }

    void Destroy()
    {
        if (FlashingTextCoroutine != null)
        {
            StopCoroutine(FlashingTextCoroutine);
        }
    }

    // #################
    // # Event Methods #
    // #################

    private void OnPlayerCreated(PlayerBehaviour player)
    {
        Debug.Log("Player Created" + player);
    }

    private void OnPlayerDestroyed(PlayerBehaviour player)
    {
        Debug.Log("Player Destroyed " + player);
        OnPlayerHit();
    }

    // ##################
    // # Custom Methods #
    // ##################

    private void OnAsteroidWaveStateChanged(AsteroidWaveState newState)
    {
        CurrentWaveState = newState;
        switch(CurrentWaveState)
        {
            case AsteroidWaveState.Initialising:
                Score = 0;
                DisplayScore();
                DisplayInfo("INCOMING ASTEROIDS", true);
                break;
            case AsteroidWaveState.InProgress:
                HideInfo();
                break;
            case AsteroidWaveState.Finished:
                InfoText.text = "ASTEROIDS DESTROYED";
                break;
        }
    }

    private void OnPlayerHit()
    {
        DecrementLives();
    }

    public void AddScore(int score)
    {
        Score += score;
        ScoreText.text = "SCORE " + Score.ToString("00000000");
    }

    private void DisplayScore()
    {
        ScoreText.text = "SCORE " + Score.ToString("00000000");
    }

    private void DisplayWave()
    {
        ScoreText.text = "WAVE " + WaveNumber.ToString("00");
    }

    private void DisplayInfo(string text, bool flashing)
    {
        InfoText.text = text;
        InfoText.enabled = true;
        if (flashing)
        {
            FlashingTextCoroutine = StartCoroutine(FlashText(InfoText));
        }
    }

    private void HideInfo()
    {
        if (FlashingTextCoroutine != null)
        {
            StopCoroutine(FlashingTextCoroutine);
        }
        InfoText.enabled = false;
    }

    void NextWave() 
    {
        WaveNumber++;
        DisplayWave();
        AsteroidsWaveBehaviour.InitialiseWave();
    }

    public void DecrementLives()
    {
        Lives--;
        UpdateLives();
        if (Lives == 0)
        {
            DisplayInfo("GAME OVER", true);
        }
    }

    private void UpdateLives()
    {
        GameObject displayLives = GameObject.Find("DisplayLives");
        foreach (Transform child in displayLives.transform)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < Lives; i++)
        {
            GameObject life = Instantiate(PlayerLife, displayLives.transform);
            life.transform.localPosition = new Vector3(i * 0.35f, 0, 0);
        }
    }


    // ##############
    // # Coroutines #
    // ##############

    private IEnumerator FlashText(TextMeshProUGUI text)
    {
        while (true)
        {
            text.enabled = !text.enabled;
            yield return new WaitForSeconds(Constants.TextFlashSpeed);
        }
    }
}