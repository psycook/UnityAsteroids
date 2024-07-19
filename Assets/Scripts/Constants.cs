using System.Collections.Generic;
using UnityEngine;

public static class Constants 
{
    public const float TextFlashSpeed = 0.5f;

    public static List<GameObject> FindChildrenWithTag(GameObject parent, string tag)
    {
        List<GameObject> foundObjects = new List<GameObject>();

        void SearchChildren(Transform current)
        {
            foreach (Transform child in current)
            {
                if (child.CompareTag(tag))
                {
                    foundObjects.Add(child.gameObject);
                }
                SearchChildren(child);
            }
        }

        SearchChildren(parent.transform);

        return foundObjects;
    }
}

public enum AsteroidWaveState
{
    Default,
    Initialising,
    InProgress,
    Finished
}
public enum AsteroidSize
{
    Small,
    Medium,
    Large
}