using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class NavMeshScript : MonoBehaviour
{
    public NavMeshSurface surface;

    private void Start()
    {
        Invoke(nameof(BuildNavMesh), 1);
    }

    void BuildNavMesh()
    { 
        surface.BuildNavMesh(); 
    }
}
