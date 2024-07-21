using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class EnemyShipBehaviour : MonoBehaviour
{
    [SerializeField] private float MinSpeed = 75.0f;
    [SerializeField] private float MaxSpeed = 150.0f;
    [SerializeField] public int Points = 100;
    [SerializeField] private ParticleSystem explosionParticles;
    public Vector2 ScreenBounds { get; set; }
    private float Width;
    private float Height;
    private WaveBehaviour ParentScript;


    // Start is called before the first frame update
    void Start()
    {
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
        //if the enemy ship goes off the side of the screen then bring it back to the other side
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

    // ##################
    // # Custom Methods #
    // ##################

    public void PlayerMissileHit()
    {
        ParticleSystem particles = Instantiate(explosionParticles, transform.position, Quaternion.identity);
        particles.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        if(ParentScript)
        {
            ParentScript.EnemyShipHit(gameObject);
        }
        else
        {
            Debug.Log("ParentScript is null");
        }
    }
}