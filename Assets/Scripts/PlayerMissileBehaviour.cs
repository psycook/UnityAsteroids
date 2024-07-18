using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMissileBehaviour : MonoBehaviour
{
    [SerializeField] private float Force = 100.0f;
    
    // ######################
    // # Life Cycle Methods #
    // ######################

        void Start()
    {
        GetComponent<Rigidbody2D>().AddForce(transform.up * Force);
        Destroy(gameObject, 2.0f);
    }

    //On trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Asteroid")
        {
            Destroy(gameObject);
            other.gameObject.GetComponent<AsteroidBehaviour>().PlayerMissileHit();
        }
    }

    // ##################
    // # Custom Methods #
    // ##################
}
