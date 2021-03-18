using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireController : ElectricalElementController
{
    public Material liveMaterial;
    public Material deadMaterial;
    public Material grabbedMaterial;
    public Material releasedMaterial;

    private GameObject cylinder;
    private bool isGrabbed = false;
    private bool isPoweredPrev = false;
    private bool isGrabbedPrev = false;
	private float thickness;
    private GameObject filament;
    private float filamentThickness;
    private Renderer cylinderRenderer;
    private Renderer endpointRenderer;
    private OVRGrabbable endpointGrabbable;


    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        endpointGrabbable = endpoint1.GetComponent<OVRGrabbable>();
        endpointRenderer = endpoint1.GetComponent<Renderer>();
        cylinder = transform.Find("Cylinder").gameObject;
        filament = cylinder.transform.Find("Wire Filament").gameObject;
        thickness = cylinder.transform.localScale.x;
        filamentThickness = filament.transform.localScale.x;
        cylinderRenderer = cylinder.GetComponent<Renderer>();
        voltage = 0;
        resistance = 0;
    }

    // Update is called once per frame
    void Update()
    {
        isGrabbed = endpointGrabbable.isGrabbed;
        Vector3 avgPos = 0.5f * (endpoint0.transform.localPosition + endpoint1.transform.localPosition);
        Vector3 difference = endpoint0.transform.localPosition - endpoint1.transform.localPosition;
        float length = 0.5f * difference.magnitude;
        Quaternion rotation = new Quaternion();
        rotation.SetFromToRotation(Vector3.up, difference);
        cylinder.transform.localRotation = rotation;
        cylinder.transform.localPosition = avgPos;
        if (length > 0.125f)
        {
            cylinder.SetActive(true);
            filament.SetActive(true);
            cylinder.transform.localScale = new Vector3(thickness, length - 0.125f, thickness);
            filament.transform.localScale = new Vector3(filamentThickness, length / (length - 0.125f), filamentThickness);
        }
        else
        {
            cylinder.SetActive(false);
            filament.SetActive(false);
        }
        if (isPowered != isPoweredPrev)
        {
            isPoweredPrev = isPowered;
            cylinderRenderer.material = isPowered ? liveMaterial : deadMaterial; 
        }
        if (isGrabbed != isGrabbedPrev)
        {
            isGrabbedPrev = isGrabbed;
            endpointRenderer.material = isGrabbed ? grabbedMaterial : releasedMaterial;
        }
    }
}
