using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;

public class AttractorScript : MonoBehaviour
{
    public XRDirectInteractor leftInteractor;
    public XRDirectInteractor rightInteractor;

    public Animator patientAnim;

    public Transform leftPos;
    public Transform headPosition;
    public float speed;

    public GameObject standCollider;

    private bool IsHited;
    void Start()
    {
        IsHited = false;
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(headPosition.position, transform.position);
        

        if(dist <= 3f && !IsHited)
        {
            transform.position = Vector3.MoveTowards(transform.position, headPosition.position, speed * Time.deltaTime);
            //standCollider.GetComponent<BoxCollider>().enabled = false;

            print("Distance to other: " + dist);
            leftInteractor.interactionLayers = 0;
            rightInteractor.interactionLayers = 0;
        }
        
        if(dist <= 0.2f)
        {
            patientAnim.SetTrigger("hit");
            print("Hit Player!!!!!");
            transform.position = leftPos.position;
            
            IsHited = true;
            leftInteractor.interactionLayers = 1;
            rightInteractor.interactionLayers = 0;
        }

    }
}
