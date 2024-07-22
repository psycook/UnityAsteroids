
using UnityEngine;
using UnityEngine.InputSystem;

public class TitleBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject Wave;
    [SerializeField] private InputAction StartAction;
    private WaveBehaviour _waveBehaviour;
    private Transform _transform;
    
    // #####################
    // # Lifecycle Methods #
    // #####################

    void OnEnable()
    {
        StartAction.Enable();
    }

    void OnDisable()
    {
        StartAction.Disable();
    }

    void Start()
    {
        _transform = GetComponent<Transform>();
        if(Wave) _waveBehaviour = Wave.GetComponent<WaveBehaviour>();
        Invoke("StartAsteroids", 1.0f);
    }

    void Update()
    {
        _transform.position = new Vector3(_transform.position.x, _transform.position.y + Mathf.Sin(Time.time) * 0.0015f, _transform.position.z);
        _transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time) * 1.0f);

        if (StartAction.triggered)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
        }
    }

    // ##################
    // # Custom Methods #
    // ##################

    public void StartAsteroids()
    {
        _waveBehaviour?.InitialiseWave();
    }
}