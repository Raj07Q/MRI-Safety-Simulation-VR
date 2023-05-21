using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using MyImage;
using System.Text;
using UnityEngine.UI;
using System.IO;
using UnityEditor;

public class LoadData : MonoBehaviour
{
    public GameObject image_icon;

    void Start()
    {
        StartCoroutine(OnLoadData());
    }

    //Get data from json
    IEnumerator OnLoadData()
    {

        var dataRequest = new UnityWebRequest("https://rahul91s3.s3.ap-south-1.amazonaws.com/Image-data.json", "GET");        
        dataRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        dataRequest.SetRequestHeader("Content-Type", "application/json");       
        yield return dataRequest.SendWebRequest();
        if (dataRequest.isNetworkError || dataRequest.isHttpError)
        {
            Debug.LogError(dataRequest.error);
            if (dataRequest.error == "HTTP/1.1 401 Unauthorized")
            {
                Debug.Log(dataRequest.error);
                
            }
            yield break;
        }


        SampleImageJson jsonObj = new SampleImageJson();//parse json data
        jsonObj = SampleImageJson.FromJson(dataRequest.downloadHandler.text);

        Debug.Log(dataRequest.downloadHandler.text);

        Debug.Log("url" + jsonObj.ItemUrl[0]);

        for(int i=0; i<jsonObj.ItemUrl.Length; i++)
        {
            StartCoroutine(LoadAdImage((jsonObj.ItemUrl[i]), i));
        }
       
    }


    //Store image file from server and load to resource folder
    IEnumerator LoadAdImage(string link, int id)
    {
        Texture2D tex = new Texture2D(8, 8);
        UnityWebRequest spriteRequest = UnityWebRequestTexture.GetTexture(link);

        yield return spriteRequest.SendWebRequest();
        if (spriteRequest.isNetworkError || spriteRequest.isHttpError)
        {
            Debug.LogError(spriteRequest.error);
            yield break;
        }


        tex = DownloadHandlerTexture.GetContent(spriteRequest);
        byte[] bytes = tex.EncodeToJPG();
        
        
        File.WriteAllBytes(Application.dataPath + "/Resources/" + "Image"+id+ ".png", bytes); //Save data to resource folder
        Debug.Log(Application.dataPath); 
        //AssetDatabase.Refresh(); // Refresh the unity editor

        //image_icon.GetComponent<Image>().sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect); // Load image to UI image from server directly

    }
}
