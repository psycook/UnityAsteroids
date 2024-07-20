using System.Collections.Generic;
using UnityEngine;
public class PlayerMisslePool : MonoBehaviour
{
    [SerializeField] private GameObject Prefab;
    [SerializeField] private int PoolSize = 10;
    [SerializeField] private bool CanGrow = true;
    private List<GameObject> Pool = new List<GameObject>();

    void Awake()
    {
        for (int i = 0; i < PoolSize; i++)
        {
            GameObject missile = Instantiate(Prefab, Vector3.zero, Quaternion.identity);
            missile.SetActive(false);
            Pool.Add(missile);
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
            GameObject missile = Instantiate(Prefab, Vector3.zero, Quaternion.identity);
            missile.SetActive(true);
            Pool.Add(missile);
            return missile;
        }
        return null;
    }

}
