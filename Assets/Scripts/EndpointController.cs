using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndpointController : MonoBehaviour
{
    private WireController wireController;
    private List<GameObject> vertices = new List<GameObject>();

    public void AddVertex(GameObject vertex)
    {
        vertices.Add(vertex);
    }

    private void Start()
    {
        wireController = this.transform.parent.GetComponent<WireController>();
    }

    private void Update()
    {
        GameObject nearestVertex = null;
        float length = float.MaxValue;
        foreach (GameObject vertex in vertices)
        {
            if ((vertex.transform.position - this.transform.position).magnitude < length)
            {
                length = (vertex.transform.position - this.transform.position).magnitude;
                nearestVertex = vertex;
            }
        }
        wireController.vertex1 = nearestVertex;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Vertex"))
            vertices.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Vertex"))
            while (vertices.Contains(other.gameObject))
                vertices.Remove(other.gameObject);
    }
}
