using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadSprites : MonoBehaviour
{
	public Sprite[] sprite;
	
	void Start()
	{
		//sprite = Resources.Load<Sprite>("Knight_attack_01");

		object[] loadedIcons = Resources.LoadAll("icons", typeof(Sprite));
		sprite = new Sprite[loadedIcons.Length];

		for (int i = 0; i < loadedIcons.Length; i++)
		{
			sprite[i] = (Sprite)loadedIcons[i];
		}

		//GameObject image = GameObject.Find("Image");
		//image.GetComponent<Image>().sprite = sprite[0];

		StartCoroutine(LoadMultiple());
	}

	IEnumerator LoadMultiple()
    {
		int j = 0;
		GameObject image = GameObject.Find("Image");

		while (true)
        {
			yield return new WaitForSeconds(0.1f);
			if(j<sprite.Length)
            {				
				image.GetComponent<Image>().sprite = sprite[j];
				j++;
			}
			
			Debug.Log(sprite.Length);
		}
	}


}
