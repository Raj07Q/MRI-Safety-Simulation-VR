using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class vkEnabler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //ShowVirtualKeyboard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public void ShowVirtualKeyboard(){
		TNVirtualKeyboard.instance.ShowVirtualKeyboard();
		TNVirtualKeyboard.instance.targetText = gameObject.GetComponent<TMP_InputField>();
	}
}
