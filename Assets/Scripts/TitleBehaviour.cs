using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleBehaviour : MonoBehaviour
{
    private Transform _transform;

    // Start is called before the first frame update
    void Start()
    {
        _transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        // add a floating effect to the title change position and rotation slightly
        _transform.position = new Vector3(_transform.position.x, _transform.position.y + Mathf.Sin(Time.time) * 0.0015f, _transform.position.z);
        _transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time) * 1.0f);
    }
}