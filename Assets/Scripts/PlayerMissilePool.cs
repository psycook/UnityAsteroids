using System.Collections.Generic;
using UnityEngine;
public class PlayerMissilePool : MonoBehaviour
{
    [SerializeField] private GameObject Prefab;
    [SerializeField] private int PoolSize = 10;
    [SerializeField] private bool CanGrow = true;
    private List<GameObject> Pool = new List<GameObject>();
    private Transform Parent;

    void Awake()
    {
        // get our parent gameobject
        Parent = gameObject.transform;
        
        for (int i = 0; i < PoolSize; i++)
        {
            CreateMissile(Vector3.zero, Quaternion.identity);
        }
    }

    public GameObject GetMissile()
    {
        foreach (GameObject missile in Pool)
        {
            if (!missile.activeInHierarchy)
            {
                missile.SetActive(true);
                return missile;
            }
        }
        if (CanGrow)
        {
            return CreateMissile(Vector3.zero, Quaternion.identity);
        }
        return null;
    }

    public GameObject CreateMissile(Vector3 position, Quaternion rotation)
    {
        GameObject missile = Instantiate(Prefab, Vector3.zero, Quaternion.identity);
        missile.name = "PlayerMissile";
        missile.tag = "PlayerMissile";
        if(Parent)
        {
            missile.transform.parent = Parent;
        } 
        else 
        {
            Debug.LogError("EnemyMissilePool: Parent not set");
        }
        missile.SetActive(false);
        Pool.Add(missile);
        return missile;
    }
}