using System.Collections.Generic;
using UnityEngine;
public class EnemyMissilePool : MonoBehaviour
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
                missile.GetComponent<EnemyMissileBehaviour>().Fire();
                return missile;
            }
        }
        if (CanGrow)
        {
            GameObject missile = CreateMissile(Vector3.zero, Quaternion.identity);
            missile.GetComponent<EnemyMissileBehaviour>().Fire();
            return missile;
        }
        return null;
    }

    public GameObject CreateMissile(Vector3 position, Quaternion rotation)
    {
        GameObject missile = Instantiate(Prefab, Vector3.zero, Quaternion.identity);
        missile.name = "EnemyMissile";
        missile.tag = "EnemyMissile";
        missile.SetActive(false);
        if(Parent)
        {
            missile.transform.parent = Parent;
        } 
        else 
        {
            Debug.LogError("EnemyMissilePool: Parent not set");
        }
        Pool.Add(missile);
        return missile;
    }
}