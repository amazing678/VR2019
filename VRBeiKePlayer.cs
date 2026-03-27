using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using TMPro;
using SWS;

public class VRBeiKePlayer : MonoBehaviour
{
    public InputField input;
    public Slider slider;
    public CanvasGroup groupText;

    public Transform cameraRig;
    public void OpenTip(int i,  Action action = null)
    {
        float temp = 0;
        float temp2 = 0;
        if (i == 1)
        {
            temp = 0.2f;
            temp2 = -12.78f;
        }
        if (i == 2)
        {
            temp = 0.4f;
            temp2 = -4.97f;
        }
        if (i == 3)
        {
            temp = 0.6f;
            temp2 = 4.35f;
        }
        if (i == 4)
        {
            temp = 0.8f;
            temp2 = 13.73f;
        }
        if (i == 5)
        {
            temp = 1f;
            temp2 = 20.69f;
        }

        groupText.transform.Find("Back").GetComponent<Image>().fillAmount = temp;
        groupText.transform.Find("Pos").transform.localPosition = new Vector3(temp2, -0.23f, 0);


        if (i < 5)
        {
            groupText.GetComponentInChildren<TextMeshProUGUI>().text = "已收集" + i + "次, 共5次";
        }
        else {
            groupText.GetComponentInChildren<TextMeshProUGUI>().text = "已收集完成!";
        }
        DOTween.To(() => groupText.alpha, x => { groupText.alpha = x; }, 1, 0.5f).OnComplete(() => {
            DOTween.To(() => Vector2.zero, x => { }, Vector2.zero, 2f).OnComplete(() => {
                DOTween.To(() => groupText.alpha, x => { groupText.alpha = x; },  0, 0.5f).OnComplete(() => {
                    action?.Invoke();
                });
            });
        });

    }

    // Start is called before the first frame update
    void Start()
    {

        groupText = VRMain.instance.transform.Find("NibiruXRSDK/MainCamera/Canvas1/Image").GetComponent<CanvasGroup>();
        count = 0;
        //QualitySettings.antiAliasing = 4;
        new BaseButton(VRPlayer.instance.allCanvas.Find("Next")).onClick = (GameObject go) =>
        {
            ChangeScene();
        };
    }

    private void Awake()
    {
        if (VRMain.instance == null) return;
        VRMain.instance.transform.position = cameraRig.position;
        VRMain.instance.transform.eulerAngles = cameraRig.eulerAngles;
    }
    public void ChangeScene()
    {
        if (VRMain.instance == null) return;

        VRPlayer.instance.IsOpenAllGroup1(true, false, () =>
        {
            VRMain.instance.text = "乘坐热气球";
            VRMain.instance.loadingText = "带着战利品继续游山玩水~";
            VRMain.instance.CloseScene();
            VRMain.instance.ChangeSence("Balloons", "17");//收集5个贝壳之后跳转热气球场景
        });
    }

    public void SetInput(string text) {

        float value = 1.5f;
        if (float.TryParse(text, out value))
        {
            if (value > 2) value = 2;
            if (value < 0) value = 0;
            input.text = value.ToString();
            slider.value = value;
        }
        else
        {
            input.text = "1.5";
            slider.value = value;
        }
    }
    public void SetSlider(float value) {

        value = (float)Math.Round(value, 2);
        input.text = value.ToString();
    }

    VRBeiKe temp;
    bool isOpen;
    int count;
    // Update is called once per frame
    void Update()
    {

        if (VRPlayer.instance.RayCamera == null) return;
        Ray ray = VRPlayer.instance.RayCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red);
        RaycastHit hit;
        if (isOpen == true) return;


        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("SongShu")))
        {
            temp = hit.collider.GetComponentInParent<VRBeiKe>();
            temp.color = Color.Lerp(temp.color, Color.red, Time.deltaTime * 0.5f);
            foreach (Renderer item in temp.meshRenderers) item.material.SetColor("_EmissionColor", temp.color);


            if ((temp.time += Time.deltaTime) > 3)
            {
                if (temp.gameObject.name == "Chuan001") {
                    //temp.gameObject.GetComponentInParent<splineMove>()?.Pause();
                    temp.transform.parent = null;
                    temp.transform.DORotate(Vector3.zero, 2);
                }



                //关掉自己的特效
                temp.color = Color.black;
                foreach (Renderer item in temp.meshRenderers) item.material.SetColor("_EmissionColor", temp.color);
                isOpen = true;
                VRPlayer.instance.isClosePoint = true;

                //计算目的地
                Vector3 distance = temp.transform.position - VRPlayer.instance.RayCamera.transform.position;

                if(temp.isDistance == false)
                    distance = Vector3.Normalize(distance) * ((2.5f/1.5f) * slider.value + 0.5f) + VRPlayer.instance.RayCamera.transform.position;
                else
                    distance = Vector3.Normalize(distance) * temp.distance + VRPlayer.instance.RayCamera.transform.position;

                if (temp.gameObject.name.Contains("Chuan")) temp.transform.DOScale(Vector3.one * 0.0481f, 5f).SetEase(Ease.OutQuint);
                temp.transform.DOMove(distance, 5).SetEase(Ease.OutQuint).OnComplete(() =>
                {

                    //之后再等待1秒钟， 消失即可 
                    DOTween.To(() => Vector2.zero, x => { }, Vector2.zero, 1).OnComplete(() =>
                    {
                        //消失
                        temp.transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
                        {
                            
                            count++;
                            Destroy(temp.gameObject);
                            if (count == 5)
                            {
                                OpenTip(count, () => {
                                    if (VRMain.instance != null && VRMain.instance.isAllPlayer == true)
                                    {
                                        isOpen = false;
                                        VRPlayer.instance.isClosePoint = false;
                                        ChangeScene();
                                    }
                                });
                                return;
                            }

                            //继续前行
                            OpenTip(count, ()=> {
                                isOpen = false;
                                VRPlayer.instance.isClosePoint = false;
                            });

                            if (temp.isIns == true)
                            {
                                VRPlayer.instance.InstanceBeiKe();
                            }
                        });

                    });
                });
            }
        }
        else
        {
            if (temp != null)
            {
                temp.color = Color.black;
                foreach (Renderer item in temp.meshRenderers) item.material.SetColor("_EmissionColor", temp.color);
                temp.time = 0;
            }
        }
    }
}
