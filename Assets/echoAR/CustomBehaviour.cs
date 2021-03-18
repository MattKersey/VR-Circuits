/**************************************************************************
* Copyright (C) echoAR, Inc. 2018-2020.                                   *
* echoAR, Inc. proprietary and confidential.                              *
*                                                                         *
* Use subject to the terms of the Terms of Service available at           *
* https://www.echoar.xyz/terms, or another agreement                      *
* between echoAR, Inc. and you, your company or other organization.       *
***************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomBehaviour : MonoBehaviour
{
    [HideInInspector]
    public Entry entry;

    private Material filamentMaterial;
    private Material glassMaterial;
    private GameObject endpointPrefab;
    private GameObject vertexPrefab;
    private GameObject gameManager;

    private GameObject endpoint0;
    private GameObject endpoint1;
    private GameObject vertex0;
    private GameObject vertex1;

    private ElectricalElementController controller;

    private bool isSetup = false;

    private Func<bool> setup = null;
    /// <summary>
    /// EXAMPLE BEHAVIOUR
    /// Queries the database and names the object based on the result.
    /// </summary>

    // Use this for initialization
    void Start()
    {
        // Add RemoteTransformations script to object and set its entry
        this.gameObject.AddComponent<RemoteTransformations>().entry = entry;
        BehaviorLinks bl = this.transform.parent.GetComponent<BehaviorLinks>();
        filamentMaterial = bl.filamentMaterial;
        glassMaterial = bl.glassMaterial;
        endpointPrefab = bl.endpointPrefab;
        vertexPrefab = bl.vertexPrefab;
        gameManager = bl.gameManager;

        this.tag = "Essential Edge";

        endpoint0 = Instantiate(endpointPrefab);
        endpoint0.SetActive(false);
        endpoint0.transform.parent = this.transform;
        endpoint0.name = "Endpoint (0)";
        endpoint1 = Instantiate(endpointPrefab);
        endpoint1.SetActive(false);
        endpoint1.transform.parent = this.transform;
        endpoint1.name = "Endpoint (1)";

        vertex0 = Instantiate(vertexPrefab);
        vertex0.SetActive(false);
        vertex0.transform.parent = this.transform;
        vertex0.GetComponent<VertexController>().connectedComponents.Add(this.gameObject);
        vertex1 = Instantiate(vertexPrefab);
        vertex1.SetActive(false);
        vertex1.transform.parent = this.transform;
        vertex1.GetComponent<VertexController>().connectedComponents.Add(this.gameObject);

        // Qurey additional data to get the name
        string value = "";
        if (entry.getAdditionalData() != null && entry.getAdditionalData().TryGetValue("name", out value))
        {
            // Set name
            this.gameObject.name = value;
        }

        if (entry.getAdditionalData() != null && entry.getAdditionalData().TryGetValue("type", out value))
        {
            switch (value)
            {
                case "battery":
                    setup = batterySetup;
                    break;
                case "bulb":
                    setup = bulbSetup;
                    break;
                case "resistor":
                    setup = resistorSetup;
                    break;
                case "toggle":
                    setup = toggleSetup;
                    break;
            }
        }
    }

    private bool batterySetup()
    {
        if (this.transform.Find("Scene") == null)
            return false;
        controller = this.gameObject.AddComponent<ElectricalElementController>();
        controller.Start();
        connectionSetup(
            new Vector3(0, 0, 0.025f),
            new Vector3(0, 0, -0.025f)
            );
        gameManager.GetComponent<GameManager>().battery = this.gameObject;
        return true;
    }

    private bool bulbSetup()
    {
        if (this.transform.Find("Scene") == null ||
            this.transform.Find("Scene").Find("glass0") == null ||
            this.transform.Find("Scene").Find("filament0") == null ||
            this.transform.Find("Scene").Find("filament1") == null ||
            this.transform.Find("Scene").Find("filament2") == null
            )
            return false;
        controller = this.gameObject.AddComponent<ElectricalElementController>();
        controller.Start();
        this.transform.Find("Scene").Find("glass0").GetComponent<MeshRenderer>().material = glassMaterial;
        this.transform.Find("Scene").Find("filament0").GetComponent<MeshRenderer>().material = filamentMaterial;
        this.transform.Find("Scene").Find("filament1").GetComponent<MeshRenderer>().material = filamentMaterial;
        this.transform.Find("Scene").Find("filament2").GetComponent<MeshRenderer>().material = filamentMaterial;
        connectionSetup(
            new Vector3(0, -0.125f, 0),
            new Vector3(0, -0.075f, 0.04f)
            );
        gameManager.GetComponent<GameManager>().bulb = this.gameObject;
        return true;
    }

    private bool resistorSetup()
    {
        if (this.transform.Find("Scene") == null)
            return false;
        controller = this.gameObject.AddComponent<ElectricalElementController>();
        controller.Start();
        connectionSetup(
            new Vector3(0, 0, 0.04f),
            new Vector3(0, 0, -0.04f)
            );
        gameManager.GetComponent<GameManager>().resistor = this.gameObject;
        return true;
    }

    private bool toggleSetup()
    {
        if (this.transform.Find("Scene") == null)
            return false;
        controller = this.gameObject.AddComponent<ElectricalElementController>();
        controller.Start();
        connectionSetup(
            new Vector3(0, 0, 0.005f),
            new Vector3(0, 0, -0.005f)
            );
        gameManager.GetComponent<GameManager>().toggle = this.gameObject;
        return true;
    }

    private void connectionSetup(Vector3 pos0, Vector3 pos1)
    {
        Vector3 endpointScale = new Vector3(
            endpoint0.transform.localScale.x / this.transform.localScale.x,
            endpoint0.transform.localScale.y / this.transform.localScale.y,
            endpoint0.transform.localScale.z / this.transform.localScale.z
           );
        endpoint0.transform.localPosition = pos0;
        endpoint0.transform.localScale = endpointScale;
        endpoint0.SetActive(true);
        endpoint1.transform.localPosition = pos1;
        endpoint1.transform.localScale = endpointScale;
        endpoint1.SetActive(true);

        Vector3 vertexScale = new Vector3(
            vertex0.transform.localScale.x / this.transform.localScale.x,
            vertex0.transform.localScale.y / this.transform.localScale.y,
            vertex0.transform.localScale.z / this.transform.localScale.z
           );
        vertex0.transform.localPosition = pos0;
        vertex0.transform.localScale = vertexScale;
        vertex0.SetActive(true);
        vertex1.transform.localPosition = pos1;
        vertex1.transform.localScale = vertexScale;
        vertex1.SetActive(true);
        controller.vertex0 = vertex0;
        controller.vertex1 = vertex1;
        vertex0.GetComponent<VertexController>().spawnNewWire();
        vertex1.GetComponent<VertexController>().spawnNewWire();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isSetup && setup != null)
        {
            isSetup = setup();
            this.gameObject.SetActive(!isSetup);
        }
    }
}