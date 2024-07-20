using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject Thruster;
    [SerializeField] private GameObject MissileSpawn;
    [SerializeField] private InputAction MovementAction;
    [SerializeField] private InputAction FireAction;
    [SerializeField] private float MovementSpeed = 100.0f;
    private PlayerMisslePool MissilePool;
    private Camera MainCamera;
    private Vector2 ScreenBounds;
    private float Width;
    private float Height;
    private bool IsThrusting = false;

    // ######################
    // # Lifecycle Fuctions # 
    // ######################

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
        MissilePool = GetComponent<PlayerMisslePool>();
    }

    // Update is called once per frame
    void Update()
    {
        // if fire is pressed, fire a missile
        if (FireAction.triggered)
        {
            //GameObject missile = Instantiate(MissilePrefab, MissileSpawn.transform.position, MissileSpawn.transform.rotation) as GameObject;
            GameObject missile = MissilePool.GetMissile();
            if (missile != null)
            {
                missile.transform.position = MissileSpawn.transform.position;
                missile.transform.rotation = MissileSpawn.transform.rotation;
                missile.GetComponent<PlayerMissileBehaviour>().Fire();
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
        }
        else
        {
            Thruster.SetActive(false);
        }

        // apply force to the player based on up input
        if(movement.y > 0)
        {
            GetComponent<Rigidbody2D>().AddForce(transform.up * movement.y * Time.deltaTime * MovementSpeed);
        }

        // wrap the player around the screen
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
}