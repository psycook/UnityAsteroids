using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyMissileBehaviour : MonoBehaviour
{
    [SerializeField] public float Duration = 2.0f;
    public Vector2 ScreenBounds { get; set; }
    private float Width;
    private float Height;
    
    // ######################
    // # Life Cycle Methods #
    // ######################

    void Start()
    {
        ScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        Width = transform.GetComponent<SpriteRenderer>().bounds.extents.x;
        Height = transform.GetComponent<SpriteRenderer>().bounds.extents.y;
    }

    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.gameObject.tag == "Asteroid")
    //     {
    //         DisableMissle();
    //         other.gameObject.GetComponent<AsteroidBehaviour>().PlayerMissileHit();
    //     }

    //     else if (other.gameObject.tag == "EnemyShip")
    //     {
    //         DisableMissle();
    //         other.gameObject.GetComponent<EnemyBehaviour>().PlayerMissileHit();
    //     }
    // }

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

    // ##################
    // # Custom Methods #
    // ##################

    public void Fire()
    {
        gameObject.SetActive(true);
        Invoke("DisableMissle", Duration);
    }

    private void DisableMissle()
    {
        gameObject.SetActive(false);
        CancelInvoke();
    } 
}