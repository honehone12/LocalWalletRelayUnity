using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

public class RelayServerLauncher : MonoBehaviour
{
    [SerializeField]
    private bool launchWindow;

    const string ExeFileName = "LocalWalletRelay/wallet_server.exe";
    Process process;

    string ExePath
    {
        get
        {
            var streamingAssetsDir = Application.streamingAssetsPath;
            return Path.Combine(streamingAssetsDir, ExeFileName);
        }
    }
    
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        Launch();
    }

    void Launch()
    {
        try
        {
            process = new Process();
            process.StartInfo.FileName = ExePath;
            process.StartInfo.UseShellExecute = launchWindow;
            process.StartInfo.CreateNoWindow = !launchWindow;
            process.Start();
        }
        catch (System.Exception e)
        {
            UnityDebug.LogException(e);
        }
    }

    IEnumerator GetAddress()
    {
        yield return null;
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
