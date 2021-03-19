using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleController : ElectricalElementController
{
    public bool isOn = false;
    private GameObject toggleStick;
    private GameObject toggleHandle;
    private OVRGrabbable grab = null;
    private Vector3 defaultHandlePos;

    new private void Start()
    {
        base.Start();
        toggleStick = this.transform.Find("Scene").Find("Sphere.003").gameObject;
        toggleHandle = this.transform.Find("ToggleHandle(Clone)").gameObject;
        defaultHandlePos = new Vector3(0, 0.01f, 0);
        grab = toggleHandle.GetComponent<OVRGrabbable>();
    }

    private void Update()
    {
        if (!grab.isGrabbed)
            ReleaseToggle();
        else
        {
            Quaternion rotation = new Quaternion();
            rotation.SetFromToRotation(Vector3.up, toggleHandle.transform.localPosition);
            toggleStick.transform.localRotation = Quaternion.Euler(rotation.eulerAngles.x, 0, 0);
        }

        if (toggleStick.transform.localRotation.eulerAngles.x < 330 && toggleStick.transform.localRotation.eulerAngles.x > 180)
            toggleStick.transform.localRotation = Quaternion.Euler(new Vector3(-30, 0, 0));
        else if (toggleStick.transform.localRotation.eulerAngles.x > 30 && toggleStick.transform.localRotation.eulerAngles.x < 180)
            toggleStick.transform.localRotation = Quaternion.Euler(new Vector3(30, 0, 0));

        if (toggleStick.transform.localRotation.eulerAngles.x < 180)
            isOn = true;
        else
            isOn = false;
    }

    public void ReleaseToggle()
    {
        if (toggleStick.transform.localRotation.eulerAngles.x < 180)
            toggleStick.transform.localRotation = Quaternion.Euler(new Vector3(30, 0, 0));
        else
            toggleStick.transform.localRotation = Quaternion.Euler(new Vector3(-30, 0, 0));
        Debug.Log(defaultHandlePos);
        toggleHandle.transform.localPosition = toggleStick.transform.localRotation * defaultHandlePos;
    }
}
