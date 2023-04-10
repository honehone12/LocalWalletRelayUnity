using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

using UnityDebug = UnityEngine.Debug;

public class RelayServerLauncher : MonoBehaviour
{
    [SerializeField]
    private bool launchWindow;
    [SerializeField]
    private int timeout;
    [SerializeField]
    private RpcRequest payload;

    const string ExeFileName = "wallet_server.exe";
    const string WorkingDir = "LocalWalletRelay" ;
    const string LocalServerPort = "8081";
    const string LoopbackAddress = "http://127.0.0.1:";
    const string Post = "POST";
    const string ContentType = "Content-Type";
    const string ApplicationJson = "application/json";

    Process process;
    Encoding utf8Encoder;

    string ExePath
    {
        get
        {
            var streamingAssetsDir = Application.streamingAssetsPath;
            return Path.Combine(streamingAssetsDir, WorkingDir, ExeFileName);
        }
    }

    string ChildProcessWorkingDir
    {
        get
        {
            var streamingAssetsDir = Application.streamingAssetsPath;
            return Path.Combine(streamingAssetsDir, WorkingDir);
        }
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        utf8Encoder = Encoding.UTF8;
        Launch();
    }

    void Launch()
    {
        try
        {
            process = new Process();
            process.StartInfo.FileName = ExePath;
            process.StartInfo.WorkingDirectory = ChildProcessWorkingDir;
            process.StartInfo.UseShellExecute = launchWindow;
            process.StartInfo.CreateNoWindow = !launchWindow;
            process.Start();
        }
        catch (System.Exception e)
        {
            UnityDebug.LogException(e);
        }
    }

    public void GetAddres()
    {
        _ = StartCoroutine(GetAddressCoroutine());
    }

    IEnumerator GetAddressCoroutine()
    {
        var webRequest = UnityWebRequest.Get(LoopbackAddress + LocalServerPort);
        webRequest.timeout = timeout;
        yield return webRequest.SendWebRequest();
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            UnityDebug.LogError(webRequest.error);
        }
        else
        {
            var raw = webRequest.downloadHandler.text;
            var addr = JsonUtility.FromJson<RpcResponse>(raw);
            UnityDebug.Log(addr.message);
        }
    }

    public void PostPayload()
    {
        _ = StartCoroutine(PostPayloadCroutine());
    }

    IEnumerator PostPayloadCroutine()
    {
        var payloadSer = JsonUtility.ToJson(payload);
        var payloadRaw = utf8Encoder.GetBytes(payloadSer);
        var webRequest = new UnityWebRequest(LoopbackAddress + LocalServerPort, Post);
        webRequest.SetRequestHeader(ContentType, ApplicationJson);
        webRequest.uploadHandler = new UploadHandlerRaw(payloadRaw);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        yield return webRequest.SendWebRequest();
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            UnityDebug.LogError(webRequest.error);
        }
    }

    void OnDisable()
    {
        try
        {
            if (process != null)
            {
                process.Kill();
                process.Dispose();
            }
        }
        catch (System.Exception e)
        {
            UnityDebug.LogException(e);
        }
    }
}
