using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireController : ElectricalElementController
{
	public GameObject cylinder;
	private float thickness;

    // Start is called before the first frame update
    void Start()
    {
        thickness = cylinder.transform.localScale.x;
        voltage = 0;
        resistance = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 avgPos = 0.5f * (endpoint0.transform.localPosition + endpoint1.transform.localPosition);
        Vector3 difference = endpoint0.transform.localPosition - endpoint1.transform.localPosition;
        float length = 0.5f * difference.magnitude;
        Quaternion rotation = new Quaternion();
        rotation.SetFromToRotation(Vector3.up, difference);
        cylinder.transform.localRotation = rotation;
        cylinder.transform.localPosition = avgPos;
        cylinder.transform.localScale = new Vector3(thickness, length, thickness);
    }
}
