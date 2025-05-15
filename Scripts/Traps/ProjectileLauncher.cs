using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    public Transform lauchPoint;
    public GameObject projectilePrefab;
    void FireProjectile()
    {
        Instantiate(projectilePrefab, lauchPoint.position, transform.rotation, transform);
    }
}
