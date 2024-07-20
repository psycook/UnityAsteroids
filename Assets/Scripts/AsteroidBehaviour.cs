using UnityEngine;

public class AsteroidBehaviour : MonoBehaviour
{
    [SerializeField] public AsteroidSize AsteroidSize;
    [SerializeField] private ParticleSystem collisionParticles;
    private Camera MainCamera;
    public Vector2 ScreenBounds { get; set; }
    private float Width;
    private float Height;
    public bool hasCollidedWithAnotherAsteroid = false;
    private AsteroidsWaveBehaviour ParentScript;

    void Start()
    {
        MainCamera = Camera.main;
        ScreenBounds = MainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, MainCamera.transform.position.z));
        Width = transform.GetComponent<LineRenderer>().bounds.extents.x;
        Height = transform.GetComponent<LineRenderer>().bounds.extents.y;
        ParentScript = GetComponentInParent<AsteroidsWaveBehaviour>();
        //point in a random direction
        transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        GetComponent<Rigidbody2D>().AddForce(transform.up * Random.Range(50.0f, 150.0f));
    }

    void Update()
    {
        transform.Rotate(Vector3.forward * Time.deltaTime * 10);

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
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Asteroid") && !hasCollidedWithAnotherAsteroid)
        {
            ContactPoint2D contact = collision.contacts[0];
            Vector3 collisionPoint = contact.point;
            ParticleSystem particles = Instantiate(collisionParticles, collisionPoint, Quaternion.identity);
            hasCollidedWithAnotherAsteroid = true;
            AsteroidBehaviour otherAsteroid = collision.gameObject.GetComponent<AsteroidBehaviour>();
            if (otherAsteroid != null)
            {
                otherAsteroid.hasCollidedWithAnotherAsteroid = true;
            }
        }
    }

    void LateUpdate()
    {
        hasCollidedWithAnotherAsteroid = false;
    }   

    // ##################
    // # Custom Methods #
    // ##################

    public void PlayerMissileHit()
    {
        if(ParentScript)
        {
            ParentScript.AsteroidHit(gameObject);
        }
    }
}