using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject battery = null;
    public GameObject bulb = null;
    public GameObject resistor = null;
    public GameObject toggle = null;

    public int level = 0;

    private bool isReady = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isReady)
        {
            if (battery != null &&
                bulb != null &&
                resistor != null &&
                toggle != null
                )
                isReady = true;
        }
    }

    public void setupLevel()
    {
        GameObject[] edges = GameObject.FindGameObjectsWithTag("Edge");
        foreach (GameObject edge in edges)
        {
            ElectricalElementController controller = edge.GetComponent<ElectricalElementController>();
            if (controller != null)
                controller.Delete();
        }
    }
}
