using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class LevelData
{
    public bool battery;
    public Vector3 batteryCoords;
    public bool bulb;
    public Vector3 bulbCoords;
    public bool resistor;
    public Vector3 resistorCoords;
    public bool toggle;
    public Vector3 toggleCoords;
}

public class GameManager : MonoBehaviour
{
    public GameObject battery = null;
    public GameObject bulb = null;
    public GameObject resistor = null;
    public GameObject toggle = null;
    public echoAR echoAR;

    public int level = 0;

    private bool isReady = false;
    private string echoARurl;
    private string[] levelFiles = new string[] {
        "level0params",
        "level1params",
        "level2params",
        "level3params",
        "level4params"
    };

    // Start is called before the first frame update
    void Start()
    {
        echoARurl = "https://console.echoAR.xyz/post?key=" + echoAR.APIKey;
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
                setupLevel();
            }
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

        setupObject(battery, levelData.battery, levelData.batteryCoords);
        setupObject(bulb, levelData.bulb, levelData.bulbCoords);
        setupObject(resistor, levelData.resistor, levelData.resistorCoords);
        setupObject(toggle, levelData.toggle, levelData.toggleCoords);
    }

    private void setupObject(GameObject obj, bool active, Vector3 pos)
    {
        obj.SetActive(active);
        if (active)
        {
            StartCoroutine(UploadPosition(obj, "x", pos.x.ToString()));
            StartCoroutine(UploadPosition(obj, "y", pos.y.ToString()));
            StartCoroutine(UploadPosition(obj, "z", pos.z.ToString()));
        }
    }

    IEnumerator UploadPosition(GameObject obj, string data, string val)
    {
        Entry entry = obj.GetComponent<RemoteTransformations>().entry;
        string url = echoARurl + "&entry=" + entry.getId();
        url += "&data=" + data;
        url += "&value=" + val;
        WWWForm form = new WWWForm();

        form.AddField("key", echoAR.APIKey);
        form.AddField("entry", entry.getId());
        form.AddField("data", data);
        form.AddField("value", val);
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();
        }
    }
}
