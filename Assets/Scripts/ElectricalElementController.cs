using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricalElementController : MonoBehaviour
{
	public GameObject vertexPrefab;

	public GameObject vertex0;
	public GameObject vertex1;

	public float voltage;
	public float current;
	public float resistance;
	public float snapThreshold;

    public bool isPowered = false;

    public GameObject endpoint0;
    public GameObject endpoint1;
    private Vector3 snapPosition;
	private Quaternion snapRotation;

    protected void Start()
    {
        endpoint0 = transform.Find("Endpoint (0)").gameObject;
        endpoint1 = transform.Find("Endpoint (1)").gameObject;
    }

    void GrabStart()
    {
    	snapPosition = this.transform.position;
    	snapRotation = this.transform.rotation;
    }

    void SnapOrDelete()
    {
    	if ((this.transform.position - snapPosition).magnitude > snapThreshold) {
    		Destroy(gameObject);
    	} else {
    		this.transform.position = snapPosition;
    		this.transform.rotation = snapRotation;
    	}
    }

    void EndManipulation()
    {
    	// Instantiate new wires at vertices
    	if (vertex1 != null) {
    		endpoint1.transform.position = vertex1.transform.position;
    	} else {
    		Instantiate(vertexPrefab, endpoint1.transform.position, Quaternion.identity);
    	}
    	if (vertex0 != null) {
    		endpoint0.transform.position = vertex0.transform.position;
    	} else {
    		Instantiate(vertexPrefab, endpoint0.transform.position, Quaternion.identity);
    	}
    }
}
