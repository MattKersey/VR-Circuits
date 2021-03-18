using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        Debug.Log("mkersey95" + transform.localScale);
        Debug.Log("mkersey95" + GetComponent<RemoteTransformations>());
    }
}
