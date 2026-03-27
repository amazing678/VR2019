using RenderHeads.Media.AVProVideo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RenderHeads.Media.AVProVideo.MediaPlayer;

public class VRVideoPlayer : MonoBehaviour
{
    public MediaPlayer player;
    // Start is called before the first frame update
    void Awake()
    {
        //QualitySettings.antiAliasing = 0;
        if (VRMain.instance == null) return;
        if (String.IsNullOrWhiteSpace(VRMain.instance.videoName)) return;

        String path = "D:/Unity Work/VR2019_2_Phone/VRVideo/";
        if (Application.platform == RuntimePlatform.Android)
            path = "/sdcard/VRVideo/";

        player.OpenVideoFromFile(FileLocation.AbsolutePathOrURL, path + VRMain.instance.videoName + ".mp4", true);
    }


    public void IsFinishing()
    {
        if (VRMain.instance == null) return;
        if (VRMain.instance.isAllPlayer == false) return;

        VRPlayer.instance.IsOpenAllGroup1(true, false, ()=> {

            VRMain.instance.isLookVideo = true;
            VRMain.instance.ToChangeSence();//视频播放完跳转

        });

       

    }
    
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
