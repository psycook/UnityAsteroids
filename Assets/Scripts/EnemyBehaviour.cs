using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] private float MinSpeed = 75.0f;
    [SerializeField] private float MaxSpeed = 150.0f;
    [SerializeField] public int Points = 100;
    [SerializeField] private ParticleSystem explosionParticles;
    [SerializeField] private float ShootMinRate = 1.0f;
    [SerializeField] private float ShootMaxRate = 5.0f;
    [SerializeField] private GameObject MissilePrefab;
    [SerializeField] private float MissleSpeed = 100.0f;
    [SerializeField] private AudioClip SmallEnemyAudioClip;
    [SerializeField] private AudioClip LargeEnemyAudioClip;
    public Vector2 ScreenBounds { get; set; }
    private float Width;
    private float Height;
    private WaveBehaviour ParentScript;
    private EnemySize[] PossibleSizesArray = new EnemySize[] { EnemySize.Small, EnemySize.Large, EnemySize.Large };
    private EnemySize Size;
    private EnemyMissilePool MissilePool;
    private float currentShootRate = 5.0f;
    private float shootTimer = 0.0f;
    private EnemySize currentSize;

    void Start()
    {
        EnemySize size = PossibleSizesArray[Random.Range(0, PossibleSizesArray.Length)];

        switch (size)
        {
            case EnemySize.Small:
                if (SmallEnemyAudioClip)
                {
                    AudioManager.Instance.PlaySound(SmallEnemyAudioClip, 1.0f);
                }
                transform.localScale = new Vector3(0.3f, 0.22f, 0.5f);
                Points = 200;
                currentSize = EnemySize.Small;
                break;
            case EnemySize.Large:
                if (LargeEnemyAudioClip)
                {
                    AudioManager.Instance.PlaySound(LargeEnemyAudioClip, 1.0f);
                }
                transform.localScale = new Vector3(0.4f, 0.33f, 1.0f);
                Points = 100;
                currentSize = EnemySize.Large;
                break;
        }
        ScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        Width = transform.GetComponent<LineRenderer>().bounds.extents.x;
        Height = transform.GetComponent<LineRenderer>().bounds.extents.y;
        transform.Rotate(0, 0, Random.Range(-30, 30));
        GetComponent<Rigidbody2D>().AddForce(transform.right * Random.Range(MinSpeed, MaxSpeed));
        ParentScript = GetComponentInParent<WaveBehaviour>();
        MissilePool = GameObject.Find("EnemyMissilePool").GetComponent<EnemyMissilePool>();
        SetCurrentShootRate();
    }

    void Update()
    {
        Vector3 viewPos = transform.position;
        if (viewPos.x > ScreenBounds.x + Width)
        {
            viewPos.x = -ScreenBounds.x - Width;
        }
        else if (viewPos.x < -ScreenBounds.x - Width)
        {
            viewPos.x = ScreenBounds.x + Width;
        }
        if (viewPos.y > ScreenBounds.y + Height)
        {
            viewPos.y = -ScreenBounds.y - Height;
        }
        else if (viewPos.y < -ScreenBounds.y - Height)
        {
            viewPos.y = ScreenBounds.y + Height;
        }
        transform.position = viewPos;

        shootTimer += Time.deltaTime;

        if(shootTimer >= currentShootRate)
        {
            Shoot();
            shootTimer = 0.0f;
            SetCurrentShootRate();
        }
    }

    // ##################
    // # Custom Methods #
    // ##################

    public void PlayerMissileHit()
    {
        ParticleSystem particles = Instantiate(explosionParticles, transform.position, Quaternion.identity);
        particles.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        if (ParentScript)
        {
            ParentScript.EnemyShipHit(gameObject);
        }
        else
        {
            Debug.Log("ParentScript is null");
        }
    }

    private void SetCurrentShootRate()
    {
        switch (currentSize)
        {
            case EnemySize.Small:
                currentShootRate = Random.Range(ShootMinRate/2, ShootMaxRate/2);
                break;
            case EnemySize.Large:
                currentShootRate = Random.Range(ShootMinRate, ShootMaxRate);
                break;
        }
    }

    private void Shoot() 
    {
           GameObject player = GameObject.FindGameObjectWithTag("Player");
            Vector3 playerPosition = new Vector3(0, 0, 0);
            if (player)
            {
                playerPosition = player.transform.position;
            }
            float angle = Random.Range(0, 360);
            float distance = Random.Range(1.0f, (Size == EnemySize.Small) ? 2.0f : 4.0f);
            Vector3 targetPosition = new Vector3(playerPosition.x + Mathf.Cos(angle) * distance, playerPosition.y + Mathf.Sin(angle) * distance, 0);
            Vector3 direction = (targetPosition - transform.position).normalized;
            GameObject missile = MissilePool.GetMissile();
            if (missile != null)
            {
                missile.transform.position = transform.position;
                missile.transform.rotation = Quaternion.identity;
                missile.GetComponent<Rigidbody2D>().AddForce(direction * MissleSpeed);
            }
    }

}