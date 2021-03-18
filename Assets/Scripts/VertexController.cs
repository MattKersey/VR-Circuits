using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexController : MonoBehaviour
{
    public List<GameObject> connectedComponents = new List<GameObject>();
    public GameObject wirePrefab;

    public void spawnNewWire()
    {
        GameObject wire = Instantiate(wirePrefab, this.transform.position, Quaternion.identity);
        wire.transform.parent = this.transform;
        wire.GetComponent<WireController>().vertex0 = this.gameObject;
        wire.transform.Find("Endpoint (1)").GetComponent<EndpointController>().AddVertex(this.gameObject);
        addConnectedComponent(wire);
    }

    public void removeConnectedComponent(GameObject go)
    {
        connectedComponents.Remove(go);
        if (connectedComponents.Count == 0)
            Destroy(gameObject);
    }

    public void addConnectedComponent(GameObject go)
    {
        if (!connectedComponents.Contains(go))
            connectedComponents.Add(go);
    }
}
