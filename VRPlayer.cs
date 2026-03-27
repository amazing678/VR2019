using SWS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using System;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using HVRCORE;

public class VRPlayer : MonoBehaviour
{
    HVRCamCore cameraCore;
    HVRRenderMgrCore mgrCore;
    public String sceneName;
    splineMove spline;
    float minTime;
    float secondTime;
    public Transform[] points;
    public Transform baoXiangPoint;
    public VRAnim[] agents;
    public GameObject baoxiang;
    public GameObject quad;
    bool isPause;
    public GameObject[] models;
    public GameObject medium;
    public Transform mediumPoint;
    public static VRPlayer instance;

    public GameObject image;
    public BezierPathManager bezier;

    public bool isShongShu;
    public GameObject songShu;

    public bool isBeiKe;
    public GameObject beiKe;
    public Transform[] beiKePoints;

    public bool isBallon;
    public GameObject ballon;
    public Transform ballonTarget;
    public Transform ballonTarget2;
    public Transform ballonIns;
    public GameObject ballonButton;
    public AudioSource aSource;
    public Transform testGame;

    public Transform allCanvas;

    public bool isHalfAuto;

    public CanvasGroup group;
    public Camera RayCamera;
    public GameObject RayCameraObj;
    public CanvasGroup allGroup;

    public bool isHudie;
    public bool isLoading;
    void Awake() {
        if (VRMain.instance == null)
        {
            SceneManager.LoadScene("Main");
            return;
        }
        Application.targetFrameRate = 60;

        instance = this;
        if (image != null)
            image.transform.localScale = Vector3.zero;


        bool isHiddle = false;
        if (VRMain.instance != null) isHiddle = VRMain.instance.isAllPlayer;
        if (allCanvas != null && allCanvas.Find("Next") != null)
            allCanvas.Find("Next").gameObject.SetActive(isHiddle);

        allGroup = VRMain.instance.transform.Find("NibiruXRSDK/MainCamera/Canvas/AllBack").GetComponent<CanvasGroup>();
        allGroup.alpha = 1;
        aSource = GetComponent<AudioSource>();

        bool isOpen22 = true;
        if (VRMain.instance != null)
        {
            if (isLoading == false)
            {
                if (VRMain.instance.isClip1)
                    aSource.clip = VRMain.instance.clip1;
                else
                    aSource.clip = VRMain.instance.clip2;
                aSource.Play();
            }
            //isOpen22 = VRMain.instance.isHudie;
        }

        IsOpenAllGroup1(true, true);
    }

    public Transform leftCam01;
    public Transform leftCam02;
    public Transform rightCam01;
    public Transform rightCam02;

    public Transform leftCam;
    public Transform rightCam;
    public void IsOpenAllGroup1(bool isAlpha, bool isOpen, Action action = null) {

        if (isAlpha == true) allGroup.alpha = isOpen ? 1 : 0;
        else allGroup.alpha = isOpen ? 0 : 1;

        aSource.volume = isOpen ? 0 : 1;

        if (isAlpha == true) DOTween.To(() => allGroup.alpha, x => allGroup.alpha = x, isOpen ? 0 : 1, 2);
        DOTween.To(() => aSource.volume, x => aSource.volume = x, isOpen ? 1 : 0, 2).OnComplete(() => {

            action?.Invoke();

        });
    }
    public void IsOpenAllGroup(CanvasGroup cg, bool isOpen, Action action = null)
    {
        //cg.alpha = isOpen ? 1 : 0;
        aSource.volume = isOpen ? 0 : 1;

        //DOTween.To(() => cg.alpha, x => cg.alpha = x, isOpen ? 0 : 1, 2);
        DOTween.To(() => aSource.volume, x => aSource.volume = x, isOpen ? 1 : 0, 2).OnComplete(() => {

            action?.Invoke();

        });
    }


    public bool isBlueLoading;

    public bool isFixed;

    void Start()
    {
        QualitySettings.shadowDistance = 80;
        spline = GetComponent<splineMove>();
        //if (isShongShu == true) InstanceShongShu();

        if (isBeiKe == true) InstanceBeiKe();

        if (isBallon == true)
        {
            //InstanceBallon();
            ballonButton.SetActive(false);
        }

        if (isHalfAuto == true)
        {
            if (spline != null) spline.Pause();
        }
    }


    public void ReStart() {
        //SceneManager.LoadScene(0);
    }

    void OnDrawGizmos() {

        if (baoXiangPoint != null) {

            Ray ray = new Ray(baoXiangPoint.position, Vector3.down);
            RaycastHit hit;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(baoXiangPoint.position, baoXiangPoint.position + Vector3.down * 100);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Default")))
            {
                Gizmos.DrawSphere(hit.point, 1);
            }
            Gizmos.DrawSphere(baoXiangPoint.position, 0.5f);

        }



        if (points == null || points.Length == 0) return;
        foreach (Transform item in points) {
            if (item == null) continue;
            Ray ray = new Ray(item.position, Vector3.down);
            RaycastHit hit;
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(item.position, item.position + Vector3.down * 100);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Default"))) {
                Gizmos.DrawSphere(hit.point, 3);
            }

            Gizmos.DrawSphere(item.position, 1);
        }
    }
    public Transform hvrCamera;
    [NonSerialized] public int i;
    void Update()
    {

        if (VRForestPlayer.instance != null)
        {
            if (VRForestPlayer.instance.isFixed == false) AnimRun();
            else Anim2Update();
        }

        if (isHudie == false)
        {
            Point();
        }
        else
        {
            if (isHudie02 == false)
                Point1();
            else
                Point2();
        }


        if (spline == null) return;

        if (isHalfAuto && isPause == false) {
            //if (Input.GetKeyDown(KeyCode.Z)) spline.Resume();
            //if (Input.GetKeyUp(KeyCode.Z)) spline.Pause();

            //if (teleport.GetStateDown(pose.inputSource))
            //{
            //    spline.Resume();
            //}
            //if (teleport.GetStateUp(pose.inputSource))
            //{
            //    spline.Pause();
            //}
        }


        if (isPause == true) return;

        if (i >= 5) {
            //说明已经收集完成了
            OpenMedium();
            return;
        }

        //1分钟 为进度
        if ((minTime += Time.deltaTime) > 60) {

            //停下全部
            isPause = true;
            spline.Pause();
            InsBaoXiang();

        }

    }
    Ray getRay() {

      
        return new Ray(RayCamera.transform.position, RayCamera.transform.forward);
    }

    void LateUpdate()
    {
        if (RayCamera == null)
        {
            RayCamera = Camera.main;//.transform.Find("LeftCamera1").GetComponent<Camera>();
            
            //RayCamera.depthTextureMode = DepthTextureMode.DepthNormals;
            return;

            //HV
            //leftCam = hvrCamera.Find("Left Camera");
            //rightCam = hvrCamera.Find("Right Camera");
            //if (leftCam == null) return;
            //RayCamera = leftCam.GetComponent<Camera>();
            
            //leftCam.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("TT"));
            //leftCam.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("NUI"));
            //rightCam.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("TT"));
            //rightCam.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("NUI"));
            //leftCam.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
            //rightCam.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;

            //if (isBlueLoading == true) {
            //    leftCam.GetComponent<Camera>().clearFlags = CameraClearFlags.Color;
            //    rightCam.GetComponent<Camera>().clearFlags = CameraClearFlags.Color;
            //    leftCam.GetComponent<Camera>().backgroundColor = new Color(0.1921f, 0.4745f, 0.2745f);
            //    rightCam.GetComponent<Camera>().backgroundColor = new Color(0.1921f, 0.4745f, 0.2745f);
            //}

           
        }


        
    }
    public void Home() {
        SceneManager.LoadScene("Main_2");
    }

    public void InsBaoXiang() {
        //出现宝箱
        Ray ray = new Ray(baoXiangPoint.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Default")))
        {
            
            VRBaoShang obj = Instantiate(baoxiang, hit.point, baoXiangPoint.rotation).GetComponent<VRBaoShang>();
            obj.model = models[i];
            obj.transform.localScale = Vector2.zero;
            obj.transform.DOScale(Vector3.one, 0.5f);
            i++;
        }
    }

    public void KeepPlaying() {
        isPause = false;
        minTime = 0;
        spline.Resume();
    }


    public void AnimRun() {

        if (points == null || points.Length <= 0) return;

        for (int i = 0; i < points.Length; i++)
        {
            if (agents[i] == null) continue;
            bool isRun = agents[i].agent.velocity != Vector3.zero;
            agents[i].anim.SetBool("IsRun", isRun);
        }

        if (isPause) return;

        //10秒 为进度
        if ((secondTime += Time.deltaTime) > 10)
        {
            secondTime = 0;
            for (int i = 0; i < points.Length; i++)
            {
                Ray ray = new Ray(points[i].position, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Default")))
                {
                    agents[i].SetPoint(hit.point);
                }
            }
        }
    }


    public void Anim2Update()
    {
        
        if (points == null || points.Length <= 0) return;

        for (int i = 0; i < points.Length; i++)
        {
            if (agents[i] == null) continue;
            bool isRun = agents[i].agent.velocity != Vector3.zero;
            agents[i].anim.SetBool("IsRun", isRun);
        }

    }
    public void AnimRun2()
    {
        
        if (points == null || points.Length <= 0) return;
     
       
        for (int i = 0; i < points.Length; i++)
        {
            if (agents[i] == null) continue;
            Ray ray = new Ray(points[i].position, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Default")))
            {
                agents[i].SetPoint(hit.point);
            }
        }
        
    }

    public bool isOpenHudiePoint;

    public bool isHudie02;

    public bool isClosePoint;
    public void Point() {
        //QualitySettings.antiAliasing = 4;
      
        if (quad == null) return;
        if (isClosePoint == true) {
            quad.SetActive(false);
            return;
        }
        if (VRPlayer.instance.RayCamera == null) return;

        //Debug.Log(RayCamera.transform.forward);
        //Ray ray = new Ray(RayCameraObj.transform.position, RayCameraObj.transform.forward);
        Ray ray = VRPlayer.instance.RayCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        
        //Debug.DrawRay(ray.origin , ray.direction, Color.yellow);

        //Debug.DrawRay(ray.origin, ray.direction, Color.green);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 
              1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("NUI")
            | 1 << LayerMask.NameToLayer("SongShu") | 1 << LayerMask.NameToLayer("TT")
            | 1 << LayerMask.NameToLayer("BaoXiang")) )
        {
            quad.SetActive(true);
            quad.transform.position = hit.point;
            quad.transform.forward = (quad.transform.position - VRPlayer.instance.RayCamera.transform.position).normalized;
            float x = Vector3.Distance(VRPlayer.instance.RayCamera.transform.position, quad.transform.position);
            float y = x * ((0.054f - 0.116f) / (2.95f - 14.82f)) + (0.116f - (14.82f * (0.054f - 0.116f)) / (2.95f - 14.82f));
            quad.transform.localScale = Vector3.one * y;
        }
        else
        {
            quad.SetActive(false);
        }
    }

    public void Point1()
    {
        if (quad == null) return;
        if (isClosePoint == true)
        {
            quad.SetActive(false);
            return;
        }
        if (VRPlayer.instance.RayCamera == null) return;
        Ray ray = getRay(); //VRPlayer.instance.RayCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

       

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity,
              1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("NUI")
            | 1 << LayerMask.NameToLayer("SongShu") | 1 << LayerMask.NameToLayer("TT")
            | 1 << LayerMask.NameToLayer("BaoXiang")| 1 << LayerMask.NameToLayer("Cube")))
        {
            if(isOpenHudiePoint) quad.SetActive(true);
            quad.transform.position = hit.point;
            quad.transform.forward = (quad.transform.position - VRPlayer.instance.RayCamera.transform.position).normalized;
            float x = Vector3.Distance(VRPlayer.instance.RayCamera.transform.position, quad.transform.position);
            float y = x * ((0.054f - 0.116f) / (2.95f - 14.82f)) + (0.116f - (14.82f * (0.054f - 0.116f)) / (2.95f - 14.82f));
            quad.transform.localScale = Vector3.one * y;
        }
        else
        {
            quad.SetActive(false);
        }
    }

    public void Point2()
    {
        if (quad == null) return;
        if (isClosePoint == true)
        {
            quad.SetActive(false);
            return;
        }
        if (VRPlayer.instance.RayCamera == null) return;
        Ray ray = getRay();//VRPlayer.instance.RayCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity,
              1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("NUI")
            | 1 << LayerMask.NameToLayer("SongShu") | 1 << LayerMask.NameToLayer("TT")
            | 1 << LayerMask.NameToLayer("BaoXiang") | 1 << LayerMask.NameToLayer("Cube")))
        {
            if (isOpenHudiePoint) quad.SetActive(true);
            //quad.SetActive(true);

            quad.transform.position = hit.point;
            quad.transform.forward = (quad.transform.position - VRPlayer.instance.RayCamera.transform.position).normalized;
            float x = Vector3.Distance(VRPlayer.instance.RayCamera.transform.position, quad.transform.position);
            float y = x * ((0.054f - 0.116f) / (2.95f - 14.82f)) + (0.116f - (14.82f * (0.054f - 0.116f)) / (2.95f - 14.82f));


            if (hit.collider.name == "hudie")
            {
                quad.transform.localScale = Vector3.one * y * 0.15f;
                quad.transform.GetChild(0).localScale = Vector3.one * (((1.565f - 0.963f) / (1.654f - 0.338f)) * x + (0.963f - (((1.565f - 0.963f) * 0.338f) / (1.654f - 0.388f))));
            }
            else
            {
                quad.transform.localScale = Vector3.one * y;
                quad.transform.GetChild(0).localScale = Vector3.one * 1.024552f;
            }
        }
        else
        {
            quad.SetActive(false);
        }
    }

    public void OpenMedium()
    {
        isPause = true;
        if(spline != null) spline.Pause();

        image.transform.DOScale(Vector3.one * 2.4489f, 0.2f);
        DOTween.To(() => Vector3.zero, x => { }, Vector3.zero, 2).OnComplete(() => {

            image.transform.DOScale(Vector3.zero,0.2f);

            VRMedium temp = Instantiate(medium, mediumPoint.position, mediumPoint.rotation).GetComponent<VRMedium>();
            Vector3 tempScale = temp.transform.localScale;
            temp.transform.localScale = Vector3.zero;
            temp.transform.DOScale(tempScale, 1).OnComplete(() => {

                //一秒钟之后进行拼合
                DOTween.To(() => Vector3.zero, x => { }, Vector3.zero, 1).OnComplete(() => {


                    DOTween.To(() => Vector3.zero, x => { }, Vector3.zero, 0.5f).OnComplete(()=> {
                        VRForestPlayer.instance.isPin = true;
                    });
                    return;
                    //temp.obj1.DOLocalMove(Vector3.zero, 1);
                    //temp.obj2.DOLocalMove(Vector3.zero, 1);
                    //temp.obj3.DOLocalMove(Vector3.zero, 1);
                    //temp.obj4.DOLocalMove(Vector3.zero, 1);
                    //temp.obj5.DOLocalMove(Vector3.zero, 1).OnComplete(() => {
                    //    temp.isRotate = true;
                    //    DOTween.To(() => Vector3.zero, x => { }, Vector3.zero, 5).OnComplete(()=> {
                    //        //VRMain.instance.sceneName = "Squirrel";
                    //        //SceneManager.LoadScene(1);
                    //    });

                    //});

                });

            });

        });
        


    }


    int beikeI = 0;
    public void InstanceBeiKe()
    {
        VRBeiKe test = Instantiate(beiKe,
            beiKePoints[beikeI].position,
            beiKePoints[beikeI].rotation).GetComponent<VRBeiKe>();
        
        test.points = beiKePoints[beikeI].Find("1");

        beikeI++;
        if (beikeI >= beiKePoints.Length) beikeI = 0;
    }
    
}
