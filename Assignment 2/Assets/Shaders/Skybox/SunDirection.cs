using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunDirection : MonoBehaviour
{
    private void Update()
    {
        Shader.SetGlobalVector("_SunDirection", transform.forward);
    }
}
