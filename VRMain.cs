using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using NibiruTask;

public class VRMain:MonoBehaviour
{
    [Header("把他设置为true就可以关闭视频---")]
    public bool isOutVideo;
    public static VRMain instance;
    public String sceneName;

    [Header("把他设置为true 就是第二套机器人--")]
    public bool isFixed_2;

    public String videoName;
    public bool isLookVideo;

    public AudioClip clip1;
    public AudioClip clip2;
    public bool isHudie = true;
    public bool isClip1;

    public String text;
    public String loadingText;

    public bool isStart = true;

    public bool isAllPlayer;

    public bool isLiyue;

    //是否为选关模式
    public bool isSingleLevelMode = false;
    public void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Start() {
        NibiruTaskApi.Init();

        VRMain.instance.isAllPlayer = true;
        VRMain.instance.isLiyue = false;

        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log("=== 当前启动的场景名字是: [" + currentScene + "] ===");

        // 【核心逻辑】：如果当前是大门 Main 场景，立刻派发去登录界面
        if (currentScene.ToLower() == "main")
        {
            Debug.Log("游戏启动！准备通过 Loading 前往登录界面...");
            VRMain.instance.text = "加载登录模块...";
            // 这里的 true 表示跳过视频，直接进场景。
            // ChangeSence 底层会自动先跳转到 "Loading" 场景，然后再异步加载 "LoginScene"
            VRMain.instance?.ChangeSence("LoginScene", "", true);
        }
        else
        {
            Debug.Log("当前不是 Main 场景，说明可能是中途跳转或单场景测试，原地待命！");
        }

        if (Application.platform == RuntimePlatform.Android)
        {
            String str = NibiruTaskApi.GetMacAddress();
            if (!String.IsNullOrEmpty(str)) onlyMac = str;
        }

    }
    public void ChangeSence(String sceneName, String videoName, bool isLookVideo = false, bool isNoHudie = true)
    {    
        Resources.UnloadUnusedAssets();
        GC.Collect();

        string currentSceneName = SceneManager.GetActiveScene().name;
        if (this.isSingleLevelMode && isLookVideo == false && sceneName != "LevelSelect")
        {
            this.sceneName = "LevelSelect";//播放完视频后回选关界面
            this.videoName = videoName;//依然播放当前关卡的专属视频
            this.isLookVideo = false;//触发360SphereVideoPlayer
            this.isHudie = isNoHudie;
        }
        else
        {
            this.sceneName = sceneName;
            this.videoName = videoName;
            this.isLookVideo = isLookVideo;
            this.isHudie = isNoHudie;
        }
        SceneManager.LoadScene("Loading");

    }
    public void ToChangeSence() {

        SceneManager.LoadScene("Loading");

    }
    public void ToMain() {
        SceneManager.LoadScene("Main_2");
    }

    public String onlyMac = "Test";

    public String GetMacAddress()
    {
        return onlyMac;

        //String str = "";

        //NetworkInterface[] nice = NetworkInterface.GetAllNetworkInterfaces();
        //if (nice.Length <= 0) return str;

        //byte[] address = nice[0].GetPhysicalAddress().GetAddressBytes();
        //if (address == null) return str;

        //for (int i = 0; i < address.Length; i++)
        //{
        //    String temp = Convert.ToString(address[i], 16).ToUpper();
        //    if (temp.Length == 1) temp = "0" + temp;

        //    str += temp;
        //    if (i < address.Length - 1)
        //        str += "-";
        //}

        //return str;
    }

    public void OpenScene(String sceneName) {

        StartCoroutine(WebOpenScene(sceneName));

    }
    public void CloseScene() {
        StartCoroutine(WebHeart());
    }
    StringBuilder sb = new StringBuilder();

    String machineRecordID = null;

    public IEnumerator WebOpenScene(String name) {


        sb.Clear();
        sb.Append("https://www.haoju.me/interface-server/api/beginOneVRmachineRecord.json?sessionID=0f67cfd7341d4cce826d2a382f9dd88c&macAddress=");
        sb.Append(GetMacAddress());
        sb.Append("&name=");
        sb.Append(name);
        //print(sb.ToString());
        //
        machineRecordID = null;

        
        UnityWebRequest request = UnityWebRequest.Get(sb.ToString());
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError){
            //SceneManager.LoadScene("LookMac");
            yield break;
        }

        try
        {
            JObject dic = JsonConvert.DeserializeObject<JObject>(request.downloadHandler.text);
            machineRecordID = dic["body"]["machineRecordID"].ToString();

        }
        catch {
            machineRecordID = null;
            //SceneManager.LoadScene("LookMac");

        }


    }



    public IEnumerator WebHeart() {

        if (machineRecordID == null) yield break;
        sb.Clear();
        sb.Append("https://www.haoju.me/interface-server/api/beatOneVRmachine.json?sessionID=0f67cfd7341d4cce826d2a382f9dd88c&machineRecordID=");
        sb.Append(machineRecordID);

        UnityWebRequest request = UnityWebRequest.Get(sb.ToString());
        yield return request.SendWebRequest();
        
    }
    public IEnumerator Heart() {
        yield return null;
        //while (true) {

        //    yield return new WaitForSeconds(10);
        //    StartCoroutine(WebHeart());
        //}

    }
}
