using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

public class VRBallon:MonoBehaviour
{
    public VRGizmo gim;
    public GameObject test;

    public GameObject obj01;
    public GameObject obj02;
    public GameObject obj03;
    public GameObject obj04;
    public GameObject obj05;
    public GameObject obj06;


    public Collider[] colliders;

    public void OpenObj(int i)
    {
        obj01.SetActive(i == 1);
        obj02.SetActive(i == 2);
        obj03.SetActive(i == 3);
        obj04.SetActive(i == 4);
        obj05.SetActive(i == 5);
        obj06.SetActive(i == 6);
    }

    
    public void Start() {

        VRPlayer.instance.isClosePoint = true;

        colliders = GetComponentsInChildren<Collider>();
        meshRenderers = GetComponentsInChildren<Renderer>();
        foreach (Collider item in colliders) item.enabled = false;

        Vector3 temp = (gim.transform.position - VRPlayer.instance.testGame.position).normalized 
            * VRBallonPlayer.instance.nearDistanceSlider.value
            + VRPlayer.instance.testGame.position;
        //test.transform.position = temp;
        //test = new GameObject();

        //  (最大距离  - 最小距离)  /  速度;

        if (VRBallonPlayer.instance.speedSlider.value == 0) VRBallonPlayer.instance.speedSlider.value = 0.001f;


        float time = (VRBallonPlayer.instance.farDistanceSlider.value - VRBallonPlayer.instance.nearDistanceSlider.value) 
            / (VRBallonPlayer.instance.speedSlider.value);

        //print(time);

        transform.DOMove(temp, time).SetEase(Ease.OutQuint).OnComplete(() =>
        {
            Vector3 temps = Vector2.zero;
            if (VRBallonPlayer.instance.count < 1 * 1) temps = gim.targetLeftTop.position;
            else if (VRBallonPlayer.instance.count < 1 * 2) temps = gim.targetLeftBottom.position;
            else if (VRBallonPlayer.instance.count < 1 * 3) temps = gim.targetRightTop.position;
            else if (VRBallonPlayer.instance.count < 1 * 4) temps = gim.targetRightBottom.position;

            //要距离人眼10m 
            temps = (temps - VRBallonPlayer.instance.middlePoint.position).normalized * 10 + VRBallonPlayer.instance.middlePoint.position;



            DOTween.To(() => Vector3.zero, x => { }, Vector3.zero, 5).OnComplete(() => {
                foreach (Collider item in colliders) item.enabled = true;
                isOpenBallon = true;
                VRPlayer.instance.isClosePoint = false;
            });

            transform.DOMove(temps, 8).SetEase(Ease.InQuint).OnComplete(() =>{
                
            });
        });
    }

    bool isOpenBallon;
    bool isOpen;
    public void DestroyThis() {
        Destroy(gameObject);
        VRPlayer.instance.isClosePoint = true;
        //出现文字
        #region 
        //DOTween.To(() => Vector3.zero, X => { }, Vector3.zero, 2).OnComplete(() => {
        VRBallonPlayer.instance.OpenTip(VRBallonPlayer.instance.count == 3 ? "请向前看!" : "请向前看!", true, () => {
                DOTween.To(() => Vector2.zero, x => { }, Vector2.zero, VRBallonPlayer.instance.count == 3 ? 4 : 2).OnComplete(() => {
                    VRBallonPlayer.instance.OpenTip(VRBallonPlayer.instance.count == 3 ? "请向前看!" : "请向前看!", false, () => {
                        if (VRBallonPlayer.instance.count == 3)
                        {
                            if (VRMain.instance != null && VRMain.instance.isAllPlayer == true)
                            {
                                VRBallonPlayer.instance.ChangeScene();
                            }
                            return;
                        }
                        VRBallonPlayer.instance.count++;
                        VRBallonPlayer.instance.InstanceBallon(x => { gim.obj = x; }, gim.transform, 0, VRBallonPlayer.instance.count + 1);
                    });

                });

            });
        //});
        #endregion
    }


    VRBallon temp;
    Color color = new Color(0.1f, 0.1f, 0.1f);
    Renderer[] meshRenderers;
    float time;
    public void Update() {

        if (isOpenBallon == false) return;
        if (VRPlayer.instance.RayCamera == null) return;
        Ray ray = VRPlayer.instance.RayCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("BaoXiang")))
        {
            //temp = this;
            temp = hit.collider.GetComponentInParent<VRBallon>();
            temp.color = Color.Lerp(temp.color, Color.red, Time.deltaTime * 0.5f);
            foreach (Renderer item in temp.meshRenderers)
            {
                item.materials[0].SetColor("_EmissionColor", temp.color);
                item.materials[1].SetColor("_EmissionColor", temp.color);
            }
            if ((temp.time += Time.deltaTime) > 3)
            {
                DestroyThis();
            }
        }
        else
        {
            if (temp != null)
            {
                temp.color = new Color(0.1f,0.1f,0.1f);
                foreach (Renderer item in temp.meshRenderers)
                {
                    item.materials[0].SetColor("_EmissionColor", temp.color);
                    item.materials[1].SetColor("_EmissionColor", temp.color);
                }
                temp.time = 0;
            }
        }

    }
}