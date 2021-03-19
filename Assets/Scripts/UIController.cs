using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    private GameObject prevButton;
    private GameObject resetButton;
    private GameObject nextButton;
    private GameManager gameManager;
    private bool prevGrabbed;
    private bool resetGrabbed;
    private bool nextGrabbed;
    // Start is called before the first frame update
    void Start()
    {
        prevButton = this.transform.Find("Previous").gameObject;
        resetButton = this.transform.Find("Reset").gameObject;
        nextButton = this.transform.Find("Next").gameObject;
        gameManager = this.transform.parent.gameObject.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!prevGrabbed && prevButton.GetComponent<OVRGrabbable>().isGrabbed)
            gameManager.PreviousLevel();
        if (!resetGrabbed && resetButton.GetComponent<OVRGrabbable>().isGrabbed)
            gameManager.setupLevel();
        if (!nextGrabbed && nextButton.GetComponent<OVRGrabbable>().isGrabbed)
            gameManager.NextLevel();

        prevGrabbed = prevButton.GetComponent<OVRGrabbable>().isGrabbed;
        resetGrabbed = resetButton.GetComponent<OVRGrabbable>().isGrabbed;
        nextGrabbed = nextButton.GetComponent<OVRGrabbable>().isGrabbed;
    }

    public void PrevActive(bool active)
    {
        prevButton.GetComponent<Renderer>().enabled = active;
    }

    public void NextActive(bool active)
    {
        nextButton.GetComponent<Renderer>().enabled = active;
    }
}
