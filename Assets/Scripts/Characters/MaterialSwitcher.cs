using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MaterialSwitcher : MonoBehaviour
{
    public List<MaterialContainer> meshParts;
    private float countDown;

    void Update()
    {
        if (countDown <= 0) return;

        countDown -= Time.deltaTime;
        if (countDown <= 0)
        {
            SwitchOff();
        }
    }

    public void SwitchOn()
    {
        countDown = 0.5f;
        foreach (MaterialContainer meshPart in meshParts)
        {
            meshPart.meshRenderer.material = meshPart.updatedMaterial;
        }
    }

    public void SwitchOff()
    {
        foreach (MaterialContainer meshPart in meshParts)
        {
            meshPart.meshRenderer.material = meshPart.normalMaterial;
        }
    }
}

[Serializable]
public class MaterialContainer
{
    public Renderer meshRenderer;
    public Material normalMaterial;
    public Material updatedMaterial;
}