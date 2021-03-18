using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleController : ElectricalElementController
{
    public bool isOn = false;
    private GameObject toggleStick;
    private OVRGrabbable grab = null;

    new private void Start()
    {
        base.Start();
        toggleStick = this.transform.Find("Scene").Find("Sphere.003").gameObject;
        StartCoroutine(AddGrabbable());
    }

    private void Update()
    {
        if (grab != null && !grab.isGrabbed)
            ReleaseToggle();

        if (toggleStick.transform.localRotation.eulerAngles.x < 330 && toggleStick.transform.localRotation.eulerAngles.x > 180)
            toggleStick.transform.localRotation = Quaternion.Euler(new Vector3(-30, 0, 0));
        else if (toggleStick.transform.localRotation.eulerAngles.x > 30 && toggleStick.transform.localRotation.eulerAngles.x < 180)
            toggleStick.transform.localRotation = Quaternion.Euler(new Vector3(30, 0, 0));

        if (toggleStick.transform.localRotation.eulerAngles.x < 180)
            isOn = true;
        else
            isOn = false;
    }

    IEnumerator AddGrabbable()
    {
        yield return new WaitForSeconds(.2f);
        grab = toggleStick.AddComponent<OVRGrabbable>();
        grab.enabled = true;
    }

    public void ReleaseToggle()
    {
        if (toggleStick.transform.localRotation.eulerAngles.x < 180)
            toggleStick.transform.localRotation = Quaternion.Euler(new Vector3(30, 0, 0));
        else
            toggleStick.transform.localRotation = Quaternion.Euler(new Vector3(-30, 0, 0));
    }
}
