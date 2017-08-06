using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshGenerator : MonoBehaviour
{
    public static System.Action onGenerated;
    protected static bool _generated;
    public static bool generated
    {
        get
        {
            return _generated;
        }
    }

    public NavMeshSurface surface;
	// Use this for initialization
	void OnEnable ()
    {
        Generate();
    }	

    public void Generate()
    {
        surface.BuildNavMesh();
        _generated = true;
        if (onGenerated != null) onGenerated();
    }
}
