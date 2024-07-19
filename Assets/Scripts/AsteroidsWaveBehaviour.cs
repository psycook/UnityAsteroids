using System.Collections.Generic;
using UnityEngine;

public class AsteroidsWaveBehaviour : MonoBehaviour
{
    [SerializeField] public GameObject[] ShipPrefabList;
    [SerializeField] public GameObject[] LargeAsteroidPrefabList;
    [SerializeField] public GameObject[] MediumAsteroidPrefabList;
    [SerializeField] public GameObject[] SmallAsteroidPrefabList;
    [SerializeField] public float SeparationDistance = 3.0f;
    [SerializeField] private int NumberOfAsteroids = 10;
    [SerializeField] private GameObject PlayerSafefyZone;

    private Camera MainCamera;
    private Vector2 ScreenBounds;

    // #####################
    // # Lifecycle Methods #
    // #####################

    void Start()
    {
        MainCamera = Camera.main;
        ScreenBounds = MainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, MainCamera.transform.position.z));
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
    public void AsteroidHit(GameObject asteroid)
    {
        AsteroidBehaviour asteroidBehaviour = asteroid.GetComponent<AsteroidBehaviour>();
        List<GameObject> spawnPoints;

        switch(asteroidBehaviour.AsteroidSize)
        {
            case AsteroidSize.Large:
                spawnPoints = Constants.FindChildrenWithTag(asteroid, "SpawnPoint");
                Debug.Log($"Large - There are {spawnPoints.Count} spawn points on this asteroid");
                for(int i = 0; i < spawnPoints.Count; i++)
                {
                    GameObject newAsteroid = CreateAsteroid($"Asteroid_Medium_{i}", MediumAsteroidPrefabList[Random.Range(0, MediumAsteroidPrefabList.Length)], gameObject);
                    newAsteroid.transform.parent = gameObject.transform;
                    newAsteroid.transform.position = spawnPoints[i].transform.position;
                    newAsteroid.transform.rotation = spawnPoints[i].transform.rotation;
                }
                break;
            case AsteroidSize.Medium:
                spawnPoints = Constants.FindChildrenWithTag(asteroid, "SpawnPoint");
                Debug.Log($"Medium - There are {spawnPoints.Count} spawn points on this asteroid");
                for(int i = 0; i < spawnPoints.Count; i++)
                {
                    GameObject newAsteroid = CreateAsteroid($"Asteroid_Small_{i}", SmallAsteroidPrefabList[Random.Range(0, SmallAsteroidPrefabList.Length)], gameObject);
                    newAsteroid.transform.parent = gameObject.transform;
                    newAsteroid.transform.position = spawnPoints[i].transform.position;
                    newAsteroid.transform.rotation = spawnPoints[i].transform.rotation;
                }
                break;
        }
        Destroy(asteroid);
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

    private void InitialiseWave()
    {
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
        Collider2D playerSafetyZone = PlayerSafefyZone.GetComponent<Collider2D>();
        List<GameObject> ActiveAsteroids = new List<GameObject>();

        // create asteroids
        for (int i = 0; i < NumberOfAsteroids; i++)
        {
            int randomAsteroid = Random.Range(0, LargeAsteroidPrefabList.Length);
            GameObject prefab = LargeAsteroidPrefabList[randomAsteroid];
            GameObject newAsteroid = CreateAsteroid($"Asteroid_{i}", prefab, gameObject);
            float x = 0.0f, y= 0.0f;
            bool isPositionValid = false;
            while(!isPositionValid)
            {
                x = Random.Range(-ScreenBounds.x, ScreenBounds.x);
                y = Random.Range(-ScreenBounds.y, ScreenBounds.y);
                Vector3 potentialPosition = new Vector3(x,y,0);
                if(!playerSafetyZone.bounds.Contains(potentialPosition))
                {
                    isPositionValid = true;
                    foreach(GameObject asteroid in ActiveAsteroids)
                    {
                        if( (asteroid.transform.position - potentialPosition).magnitude < SeparationDistance) 
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
    }

    private void FinishWave()
    {
        CurrentWaveState = AsteroidWaveState.Finished;
    }
}