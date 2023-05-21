using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class AnimateHandOnInput : MonoBehaviour
{
    public InputActionProperty pinchAction;
    public InputActionProperty gripAction;
    public InputActionProperty A_Action;

    public Animator handAnimator;
    public GameObject virtualKeyboard;

    private bool IsPressed;

    
    void Awake()
    {
        A_Action.action.performed += Button_APressed;

        virtualKeyboard.SetActive(false);
    }

    private void OnDestroy()
    {
        A_Action.action.performed -= Button_APressed;
    }


    // Update is called once per frame
    void Update()
    {
        float pinchValue = pinchAction.action.ReadValue<float>();
        handAnimator.SetFloat("Trigger", pinchValue);

        float gripValue = gripAction.action.ReadValue<float>();
        handAnimator.SetFloat("Grip", gripValue);

        //A_Action.action.performed += context =>
        //{
            
        //};
        
    }

    public void Button_APressed(InputAction.CallbackContext context)
    {
        Debug.Log("A Pressed!!!!!!!!!!!!!!");
        //SceneManager.LoadScene("Final");
        if (!IsPressed)
        {
            virtualKeyboard.SetActive(true);

            IsPressed = true;
        }
        else
        {
            virtualKeyboard.SetActive(false);

            IsPressed = false;
        }
    }
}
