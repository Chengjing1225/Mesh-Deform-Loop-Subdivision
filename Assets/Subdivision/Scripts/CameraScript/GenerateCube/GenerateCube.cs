using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subdivision.Core;

public class GenerateCube : MonoBehaviour
{

    GameObject baseCube;
    Shape shape;

    void Start()
    {
        baseCube = GameObject.CreatePrimitive(PrimitiveType.Cube);

    }
}
