using System.Collections.Generic;
using UnityEngine;

public enum AsteroidWaveState
{
    Default,
    Initialising,
    InProgress,
    Finished
}

public class AsteroidsWaveBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject[] asteroidsPrefabList;
    [SerializeField] private int numberOfAsteroids = 10;
    [SerializeField] private GameObject playerSafefyZone;

    private Camera mainCamera;
    private Vector2 ScreenBounds;
    private List<GameObject> activeAsteroids = new List<GameObject>();


    // #####################
    // # Lifecycle Methods #
    // #####################

    void Start()
    {
        mainCamera = Camera.main;
        ScreenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
        InitialiseWave();
        Invoke("StateWave", 3.0f);
    }

    void Update()
    {
    }

    // #################
    // # Event Methods #
    // #################

    [SerializeField] private AsteroidWaveState _currentWaveState;

    public delegate void OnWaveStateChanged(AsteroidWaveState newState);
    public event OnWaveStateChanged OnWaveStateChange;

    public AsteroidWaveState CurrentWaveState
    {
        get { return _currentWaveState; }
        set
        {
            _currentWaveState = value;
            OnWaveStateChange?.Invoke(_currentWaveState);
        }
    }

    // ##################
    // # Custom Methods #
    // ##################

    private void CreateAsteroid(string name)
    {
        int randomAsteroid = Random.Range(0, asteroidsPrefabList.Length);
        Vector3 randomPosition = new Vector3(Random.Range(-ScreenBounds.x, ScreenBounds.x), Random.Range(-ScreenBounds.y, ScreenBounds.y), 0);
        while (playerSafefyZone.GetComponent<Collider2D>().bounds.Contains(randomPosition))
        {
            randomPosition = new Vector3(Random.Range(-ScreenBounds.x, ScreenBounds.x), Random.Range(-ScreenBounds.y, ScreenBounds.y), 0);
        }
        GameObject newAsteroid = Instantiate(asteroidsPrefabList[randomAsteroid], randomPosition, Quaternion.identity);
        newAsteroid.name = name;
        newAsteroid.GetComponent<AsteroidBehaviour>().ScreenBounds = ScreenBounds;
        activeAsteroids.Add(newAsteroid);
    }

    private void InitialiseWave()
    {
        Debug.Log("AsteroidsWaveBehaviour:InitialiseWave called");
        CurrentWaveState = AsteroidWaveState.Initialising;
    }

    private void NextWave()
    {
        FinishWave();
        InitialiseWave();
        Invoke("StateWave", 3.0f);
    }

    private void StateWave()
    {
        CurrentWaveState = AsteroidWaveState.InProgress;
        activeAsteroids.Clear();
        // create asteroids
        for (int i = 0; i < numberOfAsteroids; i++)
        {
            CreateAsteroid($"Asteroid_{i}");
        }
    }

    private void FinishWave()
    {
        CurrentWaveState = AsteroidWaveState.Finished;
    }
}