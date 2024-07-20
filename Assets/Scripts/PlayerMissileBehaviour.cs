using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMissileBehaviour : MonoBehaviour
{
    [SerializeField] public float Force = 100.0f;
    [SerializeField] public float Duration = 2.0f;
    
    // ######################
    // # Life Cycle Methods #
    // ######################

    //On trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Asteroid")
        {
            DisableMissle();
            other.gameObject.GetComponent<AsteroidBehaviour>().PlayerMissileHit();
        }
    }

    // ##################
    // # Custom Methods #
    // ##################

    public void Fire()
    {
        gameObject.SetActive(true);
        GetComponent<Rigidbody2D>().AddForce(transform.up * Force);
        Invoke("DisableMissle", 2.0f);
    }

    private void DisableMissle()
    {
        gameObject.SetActive(false);
        CancelInvoke();
    } 
}
