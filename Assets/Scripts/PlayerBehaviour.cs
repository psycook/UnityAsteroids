using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject Thruster;
    [SerializeField] private GameObject MissileSpawn;
    [SerializeField] private PlayerMissilePool MissilePool;
    [SerializeField] private AudioClip FireAudioClip;
    [SerializeField] private AudioClip HitAudioClip;
    [SerializeField] private AudioClip ThrustAudioClip;
    [SerializeField] private float MovementSpeed = 100.0f;
    [SerializeField] private InputAction MovementAction;
    [SerializeField] private InputAction FireAction;
    private Camera MainCamera;
    private Vector2 ScreenBounds;
    private float Width;
    private float Height;
    private bool IsThrusting = false;
    private AudioSource ThrustAudioSource;

    // ##########
    // # Events # 
    // ##########

    public delegate void OnPlayerCreatedAction(PlayerBehaviour player);
    public delegate void OnPlayerDestroyedAction(PlayerBehaviour player);
    public static event OnPlayerCreatedAction OnPlayerCreated;
    public static event OnPlayerDestroyedAction OnPlayerDestroyed;

    // #######################
    // # Lifecycle Functions # 
    // #######################

    void OnEnable()
    {
        MovementAction.Enable();
        FireAction.Enable();
    }

    void OnDisable()
    {
        MovementAction.Disable();
        FireAction.Disable();
    }

    void Start()
    {
        MainCamera = Camera.main;
        ScreenBounds = MainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, MainCamera.transform.position.z));
        Width = transform.GetComponent<LineRenderer>().bounds.extents.x;
        Height = transform.GetComponent<LineRenderer>().bounds.extents.y;
        MissilePool = GameObject.Find("PlayerMissilePool").GetComponent<PlayerMissilePool>();
        ThrustAudioSource = GetComponent<AudioSource>();
        if(OnPlayerCreated != null)
        {
            OnPlayerCreated(this);
        }
    }

    void Update()
    {
        if (FireAction.triggered)
        {
            GameObject missile = MissilePool.GetMissile();
            if (missile != null)
            {
                missile.transform.position = MissileSpawn.transform.position;
                missile.transform.rotation = MissileSpawn.transform.rotation;
                missile.GetComponent<PlayerMissileBehaviour>().Fire();
                AudioManager.Instance.PlaySound(FireAudioClip, 1.0f);
            }
        }

        // roate the player based on left and right input
        Vector2 movement = MovementAction.ReadValue<Vector2>();
        transform.Rotate(0, 0, -movement.x * Time.deltaTime * MovementSpeed);

        // thrust the player based on up input
        if (movement.y > 0)
        {
            IsThrusting = true;
        }
        else
        {
            IsThrusting = false;
        }

        if (IsThrusting)
        {
            Thruster.SetActive(true);
            Thruster.transform.localScale = new Vector3(1, Random.Range(0.5f, 1.0f), 1);
            if(!ThrustAudioSource.isPlaying)
            {
                ThrustAudioSource.PlayOneShot(ThrustAudioClip);
            }
        }
        else
        {
            Thruster.SetActive(false);
        }

        if(movement.y > 0)
        {
            GetComponent<Rigidbody2D>().AddForce(transform.up * movement.y * Time.deltaTime * MovementSpeed);
        }

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

    void OnTriggerEnter2D(Collider2D other)
    {
        switch(other.gameObject.tag)
        {
            case "EnemyMissile":
                other.gameObject.SetActive(false);
                break;
            case "Enemy":
                Destroy(other.gameObject);
                break;    
            case "Asteroid":
                Destroy(other.gameObject);
                break;
            default:
                break;
        }
        if(HitAudioClip != null)
        {
            AudioManager.Instance.PlaySound(HitAudioClip, 1.0f);
        }
        Destroy(gameObject);

        if(OnPlayerDestroyed != null)
        {
            OnPlayerDestroyed(this);
        }
    }
}