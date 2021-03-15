using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricalElementController : MonoBehaviour
{
	public GameObject vertexPrefab;

	public GameObject endpoint0;
	public GameObject endpoint1;
	public GameObject vertex0;
	public GameObject vertex1;

	public float voltage;
	public float current;
	public float resistance;
	public float snapThreshold;

	private Vector3 snapPosition;
	private Quaternion snapRotation;

    void GrabStart()
    {
    	snapPosition = this.transform.position;
    	snapRotation = this.transform.rotation;
    }

    void SnapOrDelete()
    {
    	if ((this.transform.position - snapPosition).magnitude > snapThreshold) {
    		destroy(gameObject);
    	} else {
    		this.transform.position = snapPosition;
    		this.transform.rotation = snapRotation;
    	}
    }

    void EndManipulation()
    {
    	// Instantiate new wires at vertices
    	if (vertex1 != null) {
    		endpoint1.position = vertex1.position;
    	} else {
    		Instantiate(vertexPrefab, endpoint1.position, Quaternion.identity);
    	}
    	if (vertex0 != null) {
    		endpoint0.position = vertex0.position;
    	} else {
    		Instantiate(vertexPrefab, endpoint0.position, Quaternion.identity);
    	}
    }
}
