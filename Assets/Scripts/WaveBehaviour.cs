using System.Collections.Generic;
using UnityEngine;

public class WaveBehaviour : MonoBehaviour
{
    [SerializeField] private float InitialiseDelay = 3.0f;
    [SerializeField] public GameObject[] LargeAsteroidPrefabList;
    [SerializeField] public GameObject[] MediumAsteroidPrefabList;
    [SerializeField] public GameObject[] SmallAsteroidPrefabList;
    [SerializeField] private GameObject PlayerPrefab;
    [SerializeField] private GameObject EnemyShipPrefab;
    [SerializeField] private GameObject PlayerSafefyZone;
    [SerializeField] private AudioClip BigExplosionAudioClip;
    [SerializeField] private AudioClip MediumExplosionAudioClip;
    [SerializeField] private AudioClip SmallExplosionAudioClip;    
    [SerializeField] public float SeparationDistance = 3.0f;
    [SerializeField] public int NumberOfAsteroids = 3;
    [SerializeField] private float MinEnemySpawnRate = 2.0f;
    [SerializeField] private float MaxEnemySpawnRate = 4.0f;
    [SerializeField] private int EnemyShipFrequency = 5;
    [SerializeField] public int MaxEnemies = 2;
    
    private Camera MainCamera;
    private Vector2 ScreenBounds;
    private int NumberOfEnemies = 0;
    private float currentEnemySpawnRate = 10.0f;
    private float enemySpawnTimer = 0.0f;

    // #####################
    // # Lifecycle Methods #
    // #####################

    void Start()
    {
        MainCamera = Camera.main;
        ScreenBounds = MainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, MainCamera.transform.position.z));
        SetEnemyRandomSpawnRate();
    }

    void Update()
    {
        if(CurrentWaveState == AsteroidWaveState.InProgress)
        {
            CheckForEnemyShip();
        }
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

    public delegate void OnAsteroidHit(int points);

    public event OnAsteroidHit OnAsteroidHitEvent;

    public void AsteroidHitEvent(int points)
    {
        OnAsteroidHitEvent?.Invoke(points);
    }

    public delegate void OnEnemyShipHit(int points);

    public event OnEnemyShipHit OnEnemyShipHitEvent;

    public void EnemyShipHitEvent(int points)
    {
        OnEnemyShipHitEvent?.Invoke(points);
    }

    // ##################
    // # Custom Methods #
    // ##################

    private void SetEnemyRandomSpawnRate()
    {
        currentEnemySpawnRate = Random.Range(MinEnemySpawnRate, MaxEnemySpawnRate);
    }

private void CheckForEnemyShip()
{
    enemySpawnTimer += Time.deltaTime;    
    if (enemySpawnTimer >= currentEnemySpawnRate && NumberOfEnemies < MaxEnemies)
    {
        if (Random.Range(0, EnemyShipFrequency) == 0)
        {
            GameObject enemyShip = CreateEnemyShip("EnemyShip", EnemyShipPrefab, gameObject);
            enemyShip.transform.position = new Vector3(-ScreenBounds.x, Random.Range(-ScreenBounds.y, ScreenBounds.y), 0);
            NumberOfEnemies++;
        }
        enemySpawnTimer = 0.0f;
        SetEnemyRandomSpawnRate();
    }
}
    public void AsteroidHit(GameObject asteroid)
    {
        AsteroidBehaviour asteroidBehaviour = asteroid.GetComponent<AsteroidBehaviour>();
        List<GameObject> spawnPoints;

        AsteroidHitEvent(asteroidBehaviour.Points);

        switch (asteroidBehaviour.AsteroidSize)
        {
            case AsteroidSize.Large:
                if(BigExplosionAudioClip)
                {
                    AudioManager.Instance.PlaySound(BigExplosionAudioClip, 1.0f);
                }
                spawnPoints = Constants.FindChildrenWithTag(asteroid, "SpawnPoint");
                for (int i = 0; i < spawnPoints.Count; i++)
                {
                    GameObject newAsteroid = CreateAsteroid($"Asteroid_Medium", MediumAsteroidPrefabList[Random.Range(0, MediumAsteroidPrefabList.Length)], gameObject);
                    newAsteroid.transform.parent = gameObject.transform;
                    newAsteroid.transform.position = spawnPoints[i].transform.position;
                    newAsteroid.transform.rotation = spawnPoints[i].transform.rotation;
                }
                break;
            case AsteroidSize.Medium:
                if(MediumExplosionAudioClip)
                {
                    AudioManager.Instance.PlaySound(MediumExplosionAudioClip, 1.0f);
                }
                spawnPoints = Constants.FindChildrenWithTag(asteroid, "SpawnPoint");
                for (int i = 0; i < spawnPoints.Count; i++)
                {
                    GameObject newAsteroid = CreateAsteroid($"Asteroid_Small", SmallAsteroidPrefabList[Random.Range(0, SmallAsteroidPrefabList.Length)], gameObject);
                    newAsteroid.transform.parent = gameObject.transform;
                    newAsteroid.transform.position = spawnPoints[i].transform.position;
                    newAsteroid.transform.rotation = spawnPoints[i].transform.rotation;
                }
                break;
            case AsteroidSize.Small:
                if(SmallExplosionAudioClip)
                {
                    AudioManager.Instance.PlaySound(SmallExplosionAudioClip, 1.0f);
                }
                GameObject[] asteroids = GameObject.FindGameObjectsWithTag("Asteroid");
                if(asteroids.Length == 1)
                {
                    CurrentWaveState = AsteroidWaveState.Finished;
                }
                break;
        }
        Destroy(asteroid);
    }

    public void EnemyShipHit(GameObject enemyShip)
    {
        if(BigExplosionAudioClip)
        {
            AudioManager.Instance.PlaySound(BigExplosionAudioClip, 1.0f);
        }
        EnemyShipHitEvent(enemyShip.GetComponent<EnemyBehaviour>().Points);  
        NumberOfEnemies--;
        Destroy(enemyShip);
    }

    private GameObject CreateAsteroid(string name, GameObject prefab, GameObject parent)
    {
        //set this gameobject as the new asteroid's parent
        GameObject newAsteroid = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        newAsteroid.name = name;
        newAsteroid.transform.parent = parent.transform;
        newAsteroid.GetComponent<AsteroidBehaviour>().ScreenBounds = ScreenBounds;
        return newAsteroid;
    }

    private GameObject CreateEnemyShip(string name, GameObject prefab, GameObject parent)
    {
        //set this gameobject as the new asteroid's parent
        GameObject newEnemyShip = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        newEnemyShip.name = name;
        newEnemyShip.transform.parent = parent.transform;
        newEnemyShip.GetComponent<EnemyBehaviour>().ScreenBounds = ScreenBounds;
        return newEnemyShip;
    }

    public void InitialiseWave()
    {
        CurrentWaveState = AsteroidWaveState.Initialising;
        Invoke("StartWave", InitialiseDelay);
    }

    public void NextWave()
    {
        FinishWave();
        InitialiseWave();
        Invoke("StartWave", 3.0f);
    }

    private void StartWave()
    {
        Collider2D playerSafetyZone = PlayerSafefyZone.GetComponent<Collider2D>();
        List<GameObject> ActiveAsteroids = new List<GameObject>();

        // create asteroids
        for (int i = 0; i < NumberOfAsteroids; i++)
        {
            int randomAsteroid = Random.Range(0, LargeAsteroidPrefabList.Length);
            GameObject prefab = LargeAsteroidPrefabList[randomAsteroid];
            GameObject newAsteroid = CreateAsteroid("Asteroid_Large", prefab, gameObject);
            float x = 0.0f, y = 0.0f;
            bool isPositionValid = false;
            while (!isPositionValid)
            {
                x = Random.Range(-ScreenBounds.x, ScreenBounds.x);
                y = Random.Range(-ScreenBounds.y, ScreenBounds.y);
                Vector3 potentialPosition = new Vector3(x, y, 0);
                if (!playerSafetyZone.bounds.Contains(potentialPosition))
                {
                    isPositionValid = true;
                    foreach (GameObject asteroid in ActiveAsteroids)
                    {
                        if ((asteroid.transform.position - potentialPosition).magnitude < SeparationDistance)
                        {
                            isPositionValid = false;
                            break;
                        }
                    }
                }
            }
            newAsteroid.transform.position = new Vector3(x, y, 0);
            ActiveAsteroids.Add(newAsteroid);
        }
        CurrentWaveState = AsteroidWaveState.InProgress;

        if(PlayerPrefab)
        {
            GameObject player = Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
            player.name = "Player";
        }
        else 
        {
            Debug.Log("PlayerPrefab not set, must be the intro scene.");
        }

    }

    private void FinishWave()
    {
        CurrentWaveState = AsteroidWaveState.Finished;
    }
}