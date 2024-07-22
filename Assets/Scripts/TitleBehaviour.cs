
using UnityEngine;

public class TitleBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject Wave;
    private WaveBehaviour _waveBehaviour;
    private Transform _transform;

    // #####################
    // # Lifecycle Methods #
    // #####################

    // Start is called before the first frame update
    void Start()
    {
        _transform = GetComponent<Transform>();
        if(Wave) _waveBehaviour = Wave.GetComponent<WaveBehaviour>();
        Invoke("StartAsteroids", 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        _transform.position = new Vector3(_transform.position.x, _transform.position.y + Mathf.Sin(Time.time) * 0.0015f, _transform.position.z);
        _transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time) * 1.0f);
    }

    // ##################
    // # Custom Methods #
    // ##################

    public void StartAsteroids()
    {
        _waveBehaviour?.InitialiseWave();
    }
}