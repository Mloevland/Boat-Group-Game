using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Burst;

[BurstCompile(CompileSynchronously = true)]
public class Scr_WaterBouyancy : MonoBehaviour
{
    public string[] floatingTags = { "Material/Wood", "Material/Plastic" };

    private List<Scr_BouyantObject> inVolume;
    private bool isEmpty = true;

    public float waterHeight;
    private Collider waterCollider;
    public float maxSubmersionDepth = 1f; // Maximum depth for buoyancy calculation

    private void Awake()
    {
        inVolume = new List<Scr_BouyantObject>();

        waterCollider = GetComponent<Collider>();

        if (waterCollider != null)
        {
            // Calculate the water surface height from the collider's bounds
            waterHeight = waterCollider.bounds.max.y;
        }
        else
        {
            Debug.LogWarning("No Collider found on water volume! Buoyancy might not work properly.");
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Object entered with tag: " + other.tag);

        if (!floatingTags.Contains(other.tag))
            return;

        Scr_BouyantObject tempBody;
        if (tempBody = other.GetComponent<Scr_BouyantObject>())
        {
            inVolume.Add(tempBody);
            tempBody.IsInWater(true);

            isEmpty = false;
        }
        
    }

    public void OnTriggerExit(Collider other)
    {
        if (!floatingTags.Contains(other.tag))
            return;

        Scr_BouyantObject tempBody;
        if(tempBody = other.GetComponent<Scr_BouyantObject>())
        {
            inVolume.Remove(tempBody);
            tempBody.IsInWater(false);

            if (inVolume.Count == 0)
                isEmpty = true;
        }
        
    }

    [BurstCompile]
    private void FixedUpdate()
    {
        if (isEmpty)
            return;

        foreach (Scr_BouyantObject body in inVolume)
        {
            body.CalculateBouyancy(waterHeight);
        }
    }

   

}
