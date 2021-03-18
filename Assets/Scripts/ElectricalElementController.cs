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

    public void Start()
    {
        Debug.Log(this.gameObject.name);
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
            Delete();
    	} else {
    		this.transform.position = snapPosition;
    		this.transform.rotation = snapRotation;
    	}
    }

    public void Delete()
    {
        if (vertex0 != null)
            vertex0.GetComponent<VertexController>().removeConnectedComponent(this.gameObject);
        if (vertex1 != null)
            vertex1.GetComponent<VertexController>().removeConnectedComponent(this.gameObject);
        Destroy(gameObject);
    }

    void EndManipulation()
    {
        if (vertex0 != null && vertex1 != null && vertex0 == vertex1)
        {
            endpoint0.transform.position = vertex0.transform.position;
            endpoint1.transform.position = vertex1.transform.position;
            return;
        }

        if (vertex0 != null)
            endpoint0.transform.position = vertex0.transform.position;
        else
            vertex0 = Instantiate(vertexPrefab, endpoint0.transform.position, Quaternion.identity);

        if (vertex1 != null)
            endpoint1.transform.position = vertex1.transform.position;
    	else
        {
            vertex1 = Instantiate(vertexPrefab, endpoint1.transform.position, Quaternion.identity);
            vertex1.GetComponent<VertexController>().spawnNewWire();
        }

        vertex0.GetComponent<VertexController>().spawnNewWire();
    }
}
