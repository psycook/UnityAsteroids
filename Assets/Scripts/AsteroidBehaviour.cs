using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class AsteroidBehaviour : MonoBehaviour
{
    [SerializeField] public AsteroidSize AsteroidSize;
    [SerializeField] private ParticleSystem collisionParticles;
    [SerializeField] private ParticleSystem explosionParticles;
    [SerializeField] public int Points = 10;
    private Camera MainCamera;
    public Vector2 ScreenBounds { get; set; }
    private float Width;
    private float Height;
    public bool hasCollidedWithAnotherAsteroid = false;
    private WaveBehaviour ParentScript;

    void Start()
    {
        MainCamera = Camera.main;
        ScreenBounds = MainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, MainCamera.transform.position.z));
        Width = transform.GetComponent<LineRenderer>().bounds.extents.x;
        Height = transform.GetComponent<LineRenderer>().bounds.extents.y;
        ParentScript = GetComponentInParent<WaveBehaviour>();
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
        ParticleSystem particles = Instantiate(explosionParticles, transform.position, Quaternion.identity);
        switch(AsteroidSize)
        {
            case AsteroidSize.Medium:
                particles.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                break;
            case AsteroidSize.Small:
                particles.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                break;
        }
        if(ParentScript)
        {
            ParentScript.AsteroidHit(gameObject);
        }
    }
}