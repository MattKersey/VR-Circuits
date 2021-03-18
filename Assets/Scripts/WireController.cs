using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireController : ElectricalElementController
{
    public Material liveMaterial;
    public Material deadMaterial;
    public Material grabbedMaterial;
    public Material releasedMaterial;

    public float snapThreshold;

    private GameObject cylinder;
    private bool isGrabbed = false;
    private bool isPoweredPrev = false;
    public bool isGrabbedPrev = false;
    private bool isPlaced = false;
	private float thickness;
    private GameObject filament;
    private float filamentThickness;
    private Renderer cylinderRenderer;
    private Renderer endpointRenderer;
    private OVRGrabbable cylinderGrabbable;
    private OVRGrabbable endpointGrabbable;
    private Vector3 snapPosition;
    private Quaternion snapRotation;


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
        cylinderGrabbable = cylinder.GetComponent<OVRGrabbable>();
        cylinderRenderer = cylinder.GetComponent<Renderer>();
        voltage = 0;
        resistance = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPlaced)
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
            if (isGrabbed != isGrabbedPrev)
            {
                isPlaced = isGrabbedPrev;
                isGrabbedPrev = isGrabbed;
                if (isPlaced)
                    EndManipulation();
                endpointRenderer.material = isGrabbed ? grabbedMaterial : releasedMaterial;
            }
        }
        else
        {
            isGrabbed = cylinderGrabbable.isGrabbed;
            if (isGrabbed)
            {
                isGrabbedPrev = isGrabbed;
                Vector3 difference = cylinder.transform.position - snapPosition;
                float length = difference.magnitude;

                if (length >= snapThreshold)
                    Delete();

                cylinderRenderer.material.color = new Color(
                    cylinderRenderer.material.color.r,
                    cylinderRenderer.material.color.g,
                    cylinderRenderer.material.color.b,
                    1f - (length / snapThreshold)
                    );
            }
            else if (isGrabbedPrev)
            {
                isGrabbedPrev = isGrabbed;
                Snap();
                cylinderRenderer.material.color = new Color(
                    cylinderRenderer.material.color.r,
                    cylinderRenderer.material.color.g,
                    cylinderRenderer.material.color.b,
                    1
                    );
            }
        }

        if (isPowered != isPoweredPrev)
        {
            isPoweredPrev = isPowered;
            cylinderRenderer.material = isPowered ? liveMaterial : deadMaterial; 
        }
    }

    void Snap()
    {
        cylinder.transform.position = snapPosition;
        cylinder.transform.rotation = snapRotation;
    }

    new public void EndManipulation()
    {
        endpointGrabbable.enabled = false;
        endpoint1.GetComponent<Collider>().enabled = false;
        base.EndManipulation();
        cylinderGrabbable.GetComponent<OVRGrabbable>().enabled = true;
        snapPosition = cylinder.transform.position;
        snapRotation = cylinder.transform.rotation;
    }
}
