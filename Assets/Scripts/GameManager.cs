using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public bool battery;
    public Vector3 batteryCoords;
    public float batteryRot;
    public bool bulb;
    public Vector3 bulbCoords;
    public float bulbRot;
    public bool resistor;
    public Vector3 resistorCoords;
    public float resistorRot;
    public bool toggle;
    public Vector3 toggleCoords;
    public float toggleRot;
}

public class Node
{
    public List<GameObject> edges = new List<GameObject>();
    public List<GameObject> essentialEdges = new List<GameObject>();
}

public class GameManager : MonoBehaviour
{
    public GameObject battery = null;
    public GameObject bulb = null;
    public GameObject resistor = null;
    public GameObject toggle = null;
    public echoAR echoAR;

    public Material[] instructions;
    public Renderer screen;

    public int level = 0;

    private bool isReady = false;
    private string echoARurl;
    private string[] levelFiles = new string[] {
        "level0params",
        "level1params",
        "level2params",
        "level3params"
    };
    private ElectricalElementController batteryController;
    private BulbController bulbController;
    private ElectricalElementController resistorController;
    private ToggleController toggleController;
    private GameObject ui;

    // Start is called before the first frame update
    void Start()
    {
        echoARurl = "https://console.echoAR.xyz/post?key=" + echoAR.APIKey;
        ui = this.transform.Find("UI").gameObject;
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
            {
                isReady = true;
                batteryController = battery.GetComponent<ElectricalElementController>();
                bulbController = bulb.GetComponent<BulbController>();
                resistorController = resistor.GetComponent<ElectricalElementController>();
                toggleController = toggle.GetComponent<ToggleController>();
                setupLevel();
            }
        }
        else
        {
            Electrify();
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
        GameObject[] vertices = GameObject.FindGameObjectsWithTag("Vertex");
        foreach (GameObject vertex in vertices)
        {
            VertexController controller = vertex.GetComponent<VertexController>();
            if (controller != null)
                controller.spawnNewWire();
        }
        TextAsset levelJSON = Resources.Load<TextAsset>(levelFiles[level]);
        LevelData levelData = JsonUtility.FromJson<LevelData>(levelJSON.ToString());

        setupObject(battery, levelData.battery, levelData.batteryCoords, levelData.batteryRot);
        setupObject(bulb, levelData.bulb, levelData.bulbCoords, levelData.bulbRot);
        setupObject(resistor, levelData.resistor, levelData.resistorCoords, levelData.resistorRot);
        setupObject(toggle, levelData.toggle, levelData.toggleCoords, levelData.toggleRot);
        screen.material = instructions[level];
        ui.SetActive(true);
        if (level == 0)
            ui.GetComponent<UIController>().PrevActive(false);
        else
            ui.GetComponent<UIController>().PrevActive(true);

        if (level == levelFiles.Length - 1)
            ui.GetComponent<UIController>().NextActive(false);
        else
            ui.GetComponent<UIController>().NextActive(true);
    }

    public void PreviousLevel()
    {
        if (level > 0)
            level--;
        setupLevel();
    }

    public void NextLevel()
    {
        if (level < levelFiles.Length - 1)
            level++;
        setupLevel();
    }

    private void setupObject(GameObject obj, bool active, Vector3 pos, float rot)
    {
        obj.SetActive(active);
        obj.transform.position = pos;
        obj.transform.rotation = Quaternion.Euler(new Vector3(0, rot, 0));
    }

    private void Electrify()
    {
        if (batteryController.vertex0.GetComponent<VertexController>().connectedComponents.Count == 1 ||
            batteryController.vertex1.GetComponent<VertexController>().connectedComponents.Count == 1)
            return;
        Node batteryNode0 = null;
        Node batteryNode1 = null;
        Node bulbNode0 = null;
        Node bulbNode1 = null;
        Node resistorNode0 = null;
        Node resistorNode1 = null;

        List<Node> nodes = new List<Node>();
        List<GameObject> vertices = new List<GameObject>(GameObject.FindGameObjectsWithTag("Vertex"));
        int index = 0;
        for (int i = 0; i < vertices.Count; i++)
        {
            GameObject vertex = vertices[i];
            if (vertex == null)
                continue;
            nodes.Add(new Node());
            List<GameObject> queue = new List<GameObject>();
            GameObject currentVertex;
            queue.Add(vertex);
            while (queue.Count > 0)
            {
                currentVertex = queue[0];
                queue.RemoveAt(0);
                Debug.Log(currentVertex);
                if (vertices.Contains(currentVertex))
                {
                    vertices[vertices.IndexOf(currentVertex)] = null;
                    foreach (GameObject edge in currentVertex.GetComponent<VertexController>().connectedComponents)
                    {
                        if (edge.CompareTag("Edge"))
                        {
                            nodes[index].edges.Add(edge);
                            queue.Add(edge.GetComponent<ElectricalElementController>().vertex0);
                            queue.Add(edge.GetComponent<ElectricalElementController>().vertex1);
                        }
                        else if (edge.CompareTag("Essential Edge"))
                        {
                            nodes[index].essentialEdges.Add(edge);
                            if (edge == battery)
                            {
                                if (currentVertex == batteryController.vertex0)
                                    batteryNode0 = nodes[index];
                                if (currentVertex == batteryController.vertex1)
                                    batteryNode1 = nodes[index];
                            }
                            if (edge == bulb)
                            {
                                if (currentVertex == bulbController.vertex0)
                                    bulbNode0 = nodes[index];
                                if (currentVertex == bulbController.vertex1)
                                    bulbNode1 = nodes[index];
                            }
                            if (edge == resistor)
                            {
                                if (currentVertex == resistorController.vertex0)
                                    resistorNode0 = nodes[index];
                                if (currentVertex == resistorController.vertex1)
                                    resistorNode1 = nodes[index];
                            }
                            if (edge == toggle)
                            {
                                if (toggleController.isOn)
                                {
                                    queue.Add(edge.GetComponent<ElectricalElementController>().vertex0);
                                    queue.Add(edge.GetComponent<ElectricalElementController>().vertex1);
                                }
                            }
                        }
                    }
                }
            }
            index++;
        }
        if (batteryNode0 == batteryNode1)
        {
            foreach (GameObject wire in batteryNode0.edges)
                wire.GetComponent<ElectricalElementController>().isPowered = true;
            
            foreach (Node node in nodes)
            {
                if (node != batteryNode0)
                {
                    foreach (GameObject wire in node.edges)
                        wire.GetComponent<ElectricalElementController>().isPowered = true;
                }
            }

            bulbController.isPowered = false;
            resistorController.isPowered = false;
            toggleController.isPowered = false;
            return;
        }

        List<Node> poweredNodes = new List<Node>();
        bool bulbPowered = false;
        bool resistorPowered = false;

        if ((resistorNode0 == batteryNode0 && resistorNode1 == batteryNode1) ||
            (resistorNode0 == batteryNode1 && resistorNode1 == batteryNode0))
        {
            resistorPowered = true;
            if (!poweredNodes.Contains(resistorNode0))
                poweredNodes.Add(resistorNode0);
            if (!poweredNodes.Contains(resistorNode1))
                poweredNodes.Add(resistorNode1);
        }

        if ((bulbNode0 == batteryNode0 && bulbNode1 == batteryNode1) ||
            (bulbNode0 == batteryNode1 && bulbNode1 == batteryNode0))
        {
            bulbController.power = resistorPowered ? 5.0f : 8.0f;
            bulbPowered = true;
            if (!poweredNodes.Contains(bulbNode0))
                poweredNodes.Add(bulbNode0);
            if (!poweredNodes.Contains(bulbNode1))
                poweredNodes.Add(bulbNode1);
        }

        if (((bulbNode0 == batteryNode0 && bulbNode1 == resistorNode1 && resistorNode0 == batteryNode1) ||
            (bulbNode0 == batteryNode0 && bulbNode1 == resistorNode0 && resistorNode1 == batteryNode1) ||
            (bulbNode1 == batteryNode0 && bulbNode0 == resistorNode1 && resistorNode0 == batteryNode1) ||
            (bulbNode1 == batteryNode0 && bulbNode0 == resistorNode0 && resistorNode1 == batteryNode1) ||
            (bulbNode0 == batteryNode1 && bulbNode1 == resistorNode1 && resistorNode0 == batteryNode0) ||
            (bulbNode0 == batteryNode1 && bulbNode1 == resistorNode0 && resistorNode1 == batteryNode0) ||
            (bulbNode1 == batteryNode1 && bulbNode0 == resistorNode1 && resistorNode0 == batteryNode0) ||
            (bulbNode1 == batteryNode1 && bulbNode0 == resistorNode0 && resistorNode1 == batteryNode0)) && 
            (resistorNode0 != resistorNode1 && bulbNode0 != bulbNode1))
        {
            if (!poweredNodes.Contains(bulbNode0))
                poweredNodes.Add(bulbNode0);
            if (!poweredNodes.Contains(bulbNode1))
                poweredNodes.Add(bulbNode1);
            if (!poweredNodes.Contains(batteryNode0))
                poweredNodes.Add(batteryNode0);
            if (!poweredNodes.Contains(batteryNode1))
                poweredNodes.Add(batteryNode1);
            resistorPowered = true;
            bulbController.power = 2.0f;
            bulbPowered = true;
        }

        foreach (Node node in nodes)
        {
            bool isPowered = poweredNodes.Contains(node);
            foreach (GameObject wire in node.edges)
               wire.GetComponent<ElectricalElementController>().isPowered = isPowered;
            bulbController.isPowered = bulbPowered;
            resistorController.isPowered = resistorPowered;
        }
    }
}
