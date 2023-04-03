using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;

public class WheelchairController : MonoBehaviour
{
    public XROrigin rig;
    public ActionBasedContinuousMoveProvider cont;

    public InputActionProperty gripAction;
    public GameObject player;
    //public GameObject wheelChair;

    private bool IsEntered;
    private float gripValue;

    private void Awake()
    {
        rig = FindObjectOfType<XROrigin>();
    }

    private void FixedUpdate()
    {
        gripValue = gripAction.action.ReadValue<float>();

        if(gripValue > 0.9f && IsEntered)
        {
            //Debug.Log("Grip Pressed!!!!!");
            this.transform.parent = player.transform;
            cont.moveSpeed = 0.45f;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Hands"))
        {
            IsEntered = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hands"))
        {
            IsEntered = false;
            this.transform.parent = null;
            cont.moveSpeed = 0.75f;
        }
    }
}
