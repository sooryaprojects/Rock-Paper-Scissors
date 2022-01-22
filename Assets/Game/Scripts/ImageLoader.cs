using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Firebase.Storage;
using UnityEngine.Networking;
using Firebase.Extensions;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using RCP;

public class ImageLoader : MonoBehaviour
{
    public static ImageLoader Instance;
    private void Awake()
    {
        Instance = this;
    }
    public Texture downloadedTexture;
    public RawImage rawImage;
    public RawImage waitingImage;
    public Image progressBar;
    UnityWebRequest www;

    StorageReference storageReference
    {
        get
        {
            return FirebaseStorage.DefaultInstance.GetReferenceFromUrl($"gs://rock-caesar-paper.appspot.com/Backgrounds/{FirebaseManager.instance.currentMatch.bgNumber}.jpg");
        }
    }

    /*
    //For Uploading Image
    public Texture2D texture;
    [ContextMenu("Upload")]
    public void Upload() => StartCoroutine(Upload(texture));
    IEnumerator Upload(Texture2D image)
    {
        var storage = FirebaseStorage.DefaultInstance;
        var refrance = storage.GetReference("/Backgrounds/Background.png");
        var metadata = new MetadataChange()
        {
            ContentType = "image/jpeg",
            //CustomMetadata = new Dictionary<string, string>()
            //{
            //    { "1", transform.position.ToString() },
            //    { "2", transform.position.ToString() }
            //},

            CacheControl = "public,max-age=10"

        };
        var bytes = image.EncodeToPNG();
        var uploaTask = refrance.PutBytesAsync(bytes, metadata);
        yield return new WaitUntil(() => uploaTask.IsCompleted);
        if (uploaTask.Exception != null)
        {
            Debug.LogError($" faild because {uploaTask.Exception}");
            yield break;
        }
        var getURL = refrance.GetDownloadUrlAsync();
        yield return new WaitUntil(() => getURL.IsCompleted);

        if (getURL.Exception != null)
        {
            Debug.LogError($"get Error Because {getURL.Exception}");
            yield break;
        }
        Debug.Log($"download From<color=blue> {getURL.Result}</color>");
    }

    //*/

    // For Download
    //[ContextMenu("Download")]
    //public void Download() => StartCoroutine(Download("https://firebasestorage.googleapis.com/v0/b/battleship-c4dcf.appspot.com/o/Backgrounds%2FBackground.png?alt=media&token=781d5e21-64f9-4190-8cdc-b063f859acf8"));
    //IEnumerator Download(string url)
    //{
    //    www = UnityWebRequestTexture.GetTexture(url);
    //    UpdateProgress();
    //    yield return www.SendWebRequest();
    //    if (www.isNetworkError || www.isHttpError)
    //    {
    //        Debug.Log(www.error);
    //    }
    //    else
    //    {
    //        downloadedTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
    //        //rawImage.texture = myTexture;
    //        //downloadedTexture = myTexture;
    //    }
    //}

    //// For Download Progress
    //private void UpdateProgress()
    //{
    //    float progress = www.downloadProgress;
    //    if (progressBar)
    //        progressBar.fillAmount = progress;
    //    if (progress != 1)
    //        Invoke("UpdateProgress", .1f);
    //    else
    //    {
    //        Debug.Log(progress + " ____");
    //        //SceneManager.LoadSceneAsync("MainGame");
    //    }
    //}

    public void SetBG()
    {
        StartCoroutine(DownloadBytes());
    }

    IEnumerator DownloadBytes()
    {
        progressBar.fillAmount = 0;
        progressBar.transform.parent.gameObject.SetActive(true);
        Debug.Log(String.Format("Downloading {0} ...", storageReference.Path));
        var task = storageReference.GetBytesAsync(
          0, new StorageProgress<DownloadState>(DisplayDownloadState),
          cancellationTokenSource.Token);
        yield return new WaitForTaskCompletion(this, task);
        if (!(task.IsFaulted || task.IsCanceled))
        {
            byte[] fileContents = task.Result;
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(fileContents);
            rawImage.texture = texture;
            waitingImage.texture = texture;
            //if you need sprite for SpriteRenderer or Image
            downloadedTexture = texture;
            Debug.Log("Finished downloading!");
            progressBar.transform.parent.gameObject.SetActive(false);
            //fileContents = System.Text.Encoding.Default.GetString(task.Result);
            //DebugLog(String.Format("File Size {0} bytes\n", fileContents.Length));
        }
    }

    //Debug.Log(Application.persistentDataPath);
    public Task previousTask;
    public bool operationInProgress;
    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();



    void DisplayDownloadState(DownloadState downloadState)
    {
        if (operationInProgress)
        {
            float progrss = (float)downloadState.BytesTransferred / (float)downloadState.TotalByteCount;
            progressBar.fillAmount = progrss;
        }
    }

    class WaitForTaskCompletion : CustomYieldInstruction
    {
        Task task;
        ImageLoader uiHandler;

        // Create an enumerator that waits for the specified task to complete.
        public WaitForTaskCompletion(ImageLoader uiHandler, Task task)
        {
            uiHandler.previousTask = task;
            uiHandler.operationInProgress = true;
            this.uiHandler = uiHandler;
            this.task = task;
        }

        // Wait for the task to complete.
        public override bool keepWaiting
        {
            get
            {
                if (task.IsCompleted)
                {
                    uiHandler.operationInProgress = false;
                    uiHandler.cancellationTokenSource = new CancellationTokenSource();
                    if (task.IsFaulted)
                    {
                        //uiHandler.DisplayStorageException(task.Exception);
                    }
                    return false;
                }
                return true;
            }
        }
    }

}



