using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Grid : MonoBehaviour
{
#if UNITY_EDITOR

    public float snapInterval = 0.7f;

    private void Update()
    {
        Transform[] children = new Transform[transform.childCount];
        for(int i = 0; i < children.Length; i++)
        {
            children[i] = transform.GetChild(i);
            children[i].transform.position = RoundVector(children[i].transform.position, snapInterval);
        }
    }

    float Round(float number, float interval)
    {
        return (float)(Mathf.RoundToInt(number / interval)) * interval;
    }


    Vector3 RoundVector(Vector3 vector, float interval)
    {
        return new Vector3(Round(vector.x, interval), Round(vector.y, interval), Round(vector.z, interval));
    }
#endif
}
