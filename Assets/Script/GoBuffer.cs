using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoBuffer : MonoBehaviour
{    
    public GameObject prefab;
    public int maxInstances = 10;
    protected GameObject[] gos;
    protected int instanceIndex;

    private void Awake()
    {
        gos = new GameObject[maxInstances];
        for (int i = 0; i < maxInstances; i++)
        {
            gos[i] = Instantiate<GameObject>(prefab);
            gos[i].transform.SetParent(transform);
            gos[i].gameObject.SetActive(false);
        }
    }

    public GameObject Spawn(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        gos[instanceIndex].transform.position = position;
        gos[instanceIndex].transform.rotation = rotation;
        gos[instanceIndex].transform.localScale = scale;
        gos[instanceIndex].gameObject.SetActive(true);

        GameObject output = gos[instanceIndex];
        instanceIndex = (instanceIndex + 1) % gos.Length;
        return output;
    }
}
