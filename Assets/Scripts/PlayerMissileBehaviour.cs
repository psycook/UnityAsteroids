using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMissileBehaviour : MonoBehaviour
{
    [SerializeField] private float Force = 100.0f;
    // Start is called before the first frame update
    void Start()
    {
        // Apply forward force to the missile based on the direction it is facing
        GetComponent<Rigidbody2D>().AddForce(transform.up * Force);

        // Destroy the missile after 2 seconds
        Destroy(gameObject, 2.0f);
    }

    // Update is called once per frame
    void Update()
    {        
    }
}
