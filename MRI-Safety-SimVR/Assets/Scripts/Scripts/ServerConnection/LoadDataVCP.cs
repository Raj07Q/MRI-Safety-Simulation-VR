using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using MyImage;
using System.Text;
using UnityEngine.UI;
using System.IO;
using UnityEditor;
using System.Threading.Tasks;
using System;
using System.Net;
using System.Threading;
//using Windows.UI.Core;
//using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;


namespace WpfClient
{
    public class LoadDataVCP : MonoBehaviour
    {
        public Texture2D[] sourceTexture;
        

        public TMP_InputField inputField;
        public GameObject image_icon;
        //public Texture2D[] texture;
        private string url;
        private int imagecount;
        private int imageLimit = 100;
        private bool streamActive;
        private const int chunkSize = Constants.CHUNK_SIZE;
        private readonly byte[] jpegHeader = new byte[] { 0xff, 0xd8 };
        HttpWebRequest request;
        HttpWebResponse response;
        public IBuffer currentFrame { get; private set; }

        private string previousPath;
        private string currentPath;
        int index;

        private void Start()
        {
            Debug.Log("Index value " +index);
        }

        public void OnLoad()
        {
            Debug.Log("CLICKED ONLOAD BUTTON!!!!");

#if ENABLE_WINMD_SUPPORT
                Debug.Log("Windows Runtime Support enabled");
                // Put calls to your custom .winmd API here
#endif

            OnLoadData();


            imagecount = 0;
            PlayerPrefs.SetInt("lastDeletedIndex", 0);

            string folderPath = (Application.persistentDataPath + "/Screens/");
            if (Directory.Exists(folderPath))
            {
                DirectoryInfo dir = new DirectoryInfo(folderPath);
                FileInfo[] info = dir.GetFiles("*.png");
                foreach (FileInfo f in info)
                {
                    f.Delete();
                    Debug.Log("Delete All files from Directory!");

                }
            }
            
        }



        public async void OnLoadData()
        {
            

            url = inputField.text;
            //url = "http://192.168.1.9:5001/";
            Debug.Log("PATH______________"+url);

            HttpWebRequest dataRequest = (HttpWebRequest)WebRequest.Create(url);
            dataRequest.Method = "GET";

            await Task.Yield();

            dataRequest.BeginGetResponse(OnGetResponse, dataRequest);

            void OnGetResponse(IAsyncResult asyncResult)
            {

                try
                {
                    Debug.Log("MD OGR IN");
                    byte[] imageBuffer = new byte[Constants.MAX_IMAGE_BUFFER_SIZE];

                    HttpWebRequest _request = (HttpWebRequest)asyncResult.AsyncState;
                    if (_request == null)
                    {
                        return;
                    }

                    response = (HttpWebResponse)_request.EndGetResponse(asyncResult);

                    if (null == response)
                    {
                        Debug.Log("INFORMATION:- MjpegDecoder.cs::OnGetResponse() - " + "response is null");
                        return;
                    }

                    if (HttpStatusCode.OK != response.StatusCode)
                    {
                        Debug.Log("INFORMATION:- MjpegDecoder.cs::OnGetResponse() - " + "response status code is not OK");
                        return;
                    }

                    string contentType = response.Headers["Content-Type"];
                    if (!string.IsNullOrEmpty(contentType) && !contentType.Contains("="))
                    {
                        throw new Exception("Invalid content-type header.");
                    }

                    string boundary = response.Headers["Content-Type"].Split('=')[1].Replace("\"", "");
                    byte[] boundaryBytes = Encoding.UTF8.GetBytes(boundary.StartsWith("--") ? boundary : "--" + boundary);

                    Stream responseStream = response.GetResponseStream();
                    BinaryReader streamReader = new BinaryReader(responseStream);
                    streamActive = true;
                    byte[] buffer = streamReader.ReadBytes(chunkSize);
                    while (streamActive)
                    {                        

                        lock (imageBuffer)
                        {
                            try
                            {
                                
                                int imageStart = buffer.Find(jpegHeader);
                                if (-1 != imageStart)
                                {
                                    int size = buffer.Length - imageStart;
                                    Array.Copy(buffer, imageStart, imageBuffer, 0, size);
                                    while (true)
                                    {
                                        buffer = streamReader.ReadBytes(chunkSize);
                                        int imageEnd = buffer.Find(boundaryBytes);
                                        if (-1 != imageEnd)
                                        {
                                            Array.Copy(buffer, 0, imageBuffer, size, imageEnd);
                                            size += imageEnd;
                                            byte[] frame = new byte[size];
                                            Array.Copy(imageBuffer, 0, frame, 0, size);
                                            //ProcessFrame(frame);
                                            LoadResourceFile(frame);
                                            Array.Copy(buffer, imageEnd, buffer, 0, buffer.Length - imageEnd);
                                            byte[] temp = streamReader.ReadBytes(imageEnd);
                                            Array.Copy(temp, 0, buffer, buffer.Length - imageEnd, temp.Length);
                                            break;
                                        }
                                        Array.Copy(buffer, 0, imageBuffer, size, buffer.Length); // Exception.
                                        size += buffer.Length;
                                    }
                                }
                            }
                            catch (ArgumentException error)
                            {
                                Array.Clear(imageBuffer, 0, Constants.MAX_IMAGE_BUFFER_SIZE);
                                Array.Clear(buffer, 0, chunkSize);
                                buffer = streamReader.ReadBytes(chunkSize);
                                Debug.Log("Argument Exception" + error.Message + error.StackTrace);
                            }
                        }
                    }
                }
                catch (WebException WebException_i)
                {
                    var HTTPResponse = WebException_i.Response as HttpWebResponse;
                    Debug.Log("WEBEXCEPTION ______MD OGR IN");
                    Debug.Log("WebException status code" +
                                                              ((null == HTTPResponse) ?
                                                               HttpStatusCode.InternalServerError :
                                                               HTTPResponse.StatusCode));

                    Debug.Log("WebException HResult = " + WebException_i.HResult);
                    Debug.Log("WebException Type = " + WebException_i.GetType().AssemblyQualifiedName.ToString());
                    Debug.Log("WebException Message = " + WebException_i.Message);
                    Debug.Log("WebException Source = " + WebException_i.Source);
                    Debug.Log("WebException StackTrace = " + WebException_i.StackTrace);
                    Debug.Log("WebException Status = " + WebException_i.Status);
                    Debug.Log("WebException String = " + WebException_i.ToString());
                }

            }

            //async void ProcessFrame(byte[] frame)
            //{
            //    if(imagecount < 100)
            //    {
            //        Debug.Log("Image saved");
            //        string folderPath = @"C:\Pictures";                   

            //        if (!Directory.Exists(folderPath))
            //        {
            //            Directory.CreateDirectory(folderPath);
            //        }
            //        string filepath = System.IO.Path.Combine(folderPath, $"Image{imagecount}.jpeg");
            //        File.WriteAllBytes(filepath, frame);

            //        imagecount++;
            //    }
            //    else
            //    {
            //        Debug.Log("Image overflow");
            //    }
                
            //}
        }

        async void LoadResourceFile(byte[] frame)
        {
            await Task.Yield();
            
            ExecuteOnMainThread.RunOnMainThread.Enqueue(() => {

                    
                    File.WriteAllBytes(Application.persistentDataPath + "/Screens/" + $"Image{imagecount}.png", frame); //Save data to FIRST directory on android device.
                    Debug.Log("Imagecount loaded" + imagecount);
                    Debug.Log("Data Path " + Application.persistentDataPath);
                    imagecount++;               
               

            });
            

            
        }

        public void TestLoadAll()
        {
            StartCoroutine(OnLoadAllFromPath());
            Debug.Log("Clicked PLAY BUTTON! ");

            index = PlayerPrefs.GetInt("lastDeletedIndex");
            //Debug.Log("PlayerPrefs Index__ " + index);

        }

        public IEnumerator OnLoadAllFromPath()
        {            

            while (true)
            {
                yield return new WaitForSeconds(0.1f);
                
                currentPath = (Application.persistentDataPath + "/Screens/" + $"Image{index}.png");
                LoadTextureFromPath(currentPath);
                previousPath = currentPath;

                DeleteLoadedFiles(previousPath);
                Debug.Log("CURRENT PATH !"+ currentPath);
                Debug.Log("PREVIOUS PATH !" + previousPath);
                index++;

                //Debug.Log("lastDeletedIndex__ " + index);
                PlayerPrefs.SetInt("lastDeletedIndex", index);
            }

                
        }

        private void LoadTextureFromPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("Texture path is null or empty");
          
            }

            if (!File.Exists(path))
            {
                Debug.LogError($"File not found: {path}");
        
            }

            if (System.IO.File.Exists(path))
            {
                byte[] bytes = File.ReadAllBytes(path);
                Texture2D texture = new Texture2D(1936, 1056, TextureFormat.RGB24, false);
                texture.filterMode = FilterMode.Trilinear;
                texture.LoadImage(bytes);

                image_icon.GetComponent<RawImage>().texture = texture;

            }
            

            
        }

        private void DeleteLoadedFiles(string oldPath)
        {
            if (System.IO.File.Exists(oldPath))
            {
                File.Delete(oldPath);
                Debug.Log("DELETED_" + oldPath);
            }
        }
             
       

        public void OnClickStop()
        {
            try
            {
                response.Dispose();
                streamActive = false;
                if (request != null)
                {
                    request.Abort();
                    request = null;
                }
            }
            catch (Exception error)
            {
                Debug.Log("Request Abort Error:" + error.Message);
            }
        }
    }
    static class Extensions
    {
        public static int Find(this byte[] buff, byte[] search)
        {
            for (int start = 0; start < buff.Length - search.Length; start++)
            {
                if (buff[start] == search[0])
                {
                    int next;
                    for (next = 1; next < search.Length; next++)
                    {
                        if (buff[start + next] != search[next])
                        {
                            break;
                        }
                    }

                    if (next == search.Length)
                    {
                        return start;
                    }
                }
            }
            return -1;
        }
    }
    public sealed class FrameReadyEventArgs2
    {
        public IBuffer FrameBuffer { get; set; }
    }


    public sealed class ErrorEventArgs2
    {
        public string Message { get; set; }
        public int ErrorCode { get; set; }
    }
}


       


    //Get data from json
    //public async void OnLoadData()
    //{

    //    var dataRequest = new UnityWebRequest("https://rahul91s3.s3.ap-south-1.amazonaws.com/Image-data.json", "GET");
    //    dataRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
    //    dataRequest.SetRequestHeader("Content-Type", "application/json");
    //    dataRequest.SendWebRequest();

    //    while(!dataRequest.isDone)
    //    {
    //        await Task.Yield();
    //    }

    //    if (dataRequest.result == UnityWebRequest.Result.ConnectionError)
    //    {
    //        Debug.LogError(dataRequest.error);
    //        if (dataRequest.error == "HTTP/1.1 401 Unauthorized")
    //        {
    //            Debug.Log(dataRequest.error);

    //        }

    //    }
    //    else
    //    {
    //        SampleImageJson jsonObj = new SampleImageJson();//parse json data
    //        jsonObj = SampleImageJson.FromJson(dataRequest.downloadHandler.text);

    //        Debug.Log(dataRequest.downloadHandler.text);

    //        Debug.Log("url" + jsonObj.ItemUrl[0]);

    //        for (int i = 0; i < jsonObj.ItemUrl.Length; i++)
    //        {
    //            StartCoroutine(LoadAdImage((jsonObj.ItemUrl[i]), i));
    //        }
    //    }






    //}


    //Store image file from server and load to resource folder
    //IEnumerator LoadAdImage(string link, int id)
    //{
    //    Debug.Log("Entered load image_________");

    //    Texture2D tex = new Texture2D(8, 8);
    //    UnityWebRequest spriteRequest = UnityWebRequestTexture.GetTexture(link);

    //    yield return spriteRequest.SendWebRequest();
    //    if (spriteRequest.isNetworkError || spriteRequest.isHttpError)
    //    {
    //        Debug.LogError(spriteRequest.error);
    //        yield break;
    //    }


    //    tex = DownloadHandlerTexture.GetContent(spriteRequest);
    //    byte[] bytes = tex.EncodeToJPG();
        
        
    //    File.WriteAllBytes(Application.dataPath + "/Resources/" + "Image"+id+ ".png", bytes); //Save data to resource folder
    //    Debug.Log(Application.dataPath); 
    //    AssetDatabase.Refresh(); // Refresh the unity editor

    //    //image_icon.GetComponent<Image>().sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect); // Load image to UI image from server directly

    //}

