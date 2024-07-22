using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] private float MinSpeed = 75.0f;
    [SerializeField] private float MaxSpeed = 150.0f;
    [SerializeField] public int Points = 100;
    [SerializeField] private ParticleSystem explosionParticles;
    [SerializeField] private int ShootFrequency = 200;
    [SerializeField] private GameObject MissilePrefab;
    [SerializeField] private float MissleSpeed = 100.0f;
    public Vector2 ScreenBounds { get; set; }
    private float Width;
    private float Height;
    private WaveBehaviour ParentScript;
    private EnemySize[] PossibleSizesArray = new EnemySize[] { EnemySize.Small, EnemySize.Large, EnemySize.Large };
    private EnemySize Size;

    // Start is called before the first frame update
    void Start()
    {
        EnemySize size = PossibleSizesArray[Random.Range(0, PossibleSizesArray.Length)];

        switch (size)
        {
            case EnemySize.Small:
                transform.localScale = new Vector3(0.3f, 0.22f, 0.5f);
                Points = 200;
                break;
            case EnemySize.Large:
                transform.localScale = new Vector3(0.4f, 0.33f, 1.0f);
                Points = 100;
                break;
        }

        ScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        Width = transform.GetComponent<LineRenderer>().bounds.extents.x;
        Height = transform.GetComponent<LineRenderer>().bounds.extents.y;
        transform.Rotate(0, 0, Random.Range(-30, 30));
        GetComponent<Rigidbody2D>().AddForce(transform.right * Random.Range(MinSpeed, MaxSpeed));
        ParentScript = GetComponentInParent<WaveBehaviour>();
    }

    // Update is called once per frame
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

        // find the player gameobject
        if (Random.Range(0, ShootFrequency) == 0)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player)
            {
                Vector3 playerPosition = player.transform.position;
                float angle = Random.Range(0, 360);
                float distance = Random.Range(1.0f, (Size == EnemySize.Small) ? 2.0f : 4.0f);
                Vector3 targetPosition = new Vector3(playerPosition.x + Mathf.Cos(angle) * distance, playerPosition.y + Mathf.Sin(angle) * distance, 0);
                Vector3 direction = (targetPosition - transform.position).normalized;
                GameObject missile = Instantiate(MissilePrefab, transform.position, Quaternion.identity) as GameObject;
                missile.GetComponent<Rigidbody2D>().AddForce(direction * MissleSpeed);
            }
            else
            {
                Debug.Log("Player not found, what is that about?");
            }
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
}