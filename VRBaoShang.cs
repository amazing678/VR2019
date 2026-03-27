using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
public class VRBaoShang : MonoBehaviour
{
    public Renderer[] meshRenderers;
    [NonSerialized]public Color color = Color.black;
    [NonSerialized]public float time;
    [NonSerialized]public bool isOpen;
    [NonSerialized]public Animator animator;
    [NonSerialized]public GameObject model;
    public Transform middle;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }
    VRBaoShang temp;
    public AudioClip clip;

    float timeX = 2;
    // Update is called once per frame
    void Update()
    {
        if ((timeX -= Time.deltaTime) > 0) return;

        Ray ray = VRPlayer.instance.RayCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("BaoXiang")))
        {
            //temp = this;
            temp = hit.collider.GetComponentInParent<VRBaoShang>();
            if (temp.isOpen == true) return;

            temp.color = Color.Lerp(temp.color, Color.red, Time.deltaTime * 0.5f);
            foreach (Renderer item in temp.meshRenderers) item.material.SetColor("_EmissionColor", temp.color);
            if ((temp.time += Time.deltaTime) > 3)
            {
                //关掉中心点
                VRPlayer.instance.isClosePoint = true;

                //关掉自己的特效
                temp.transform.Find("CFX3_MagicAura_A").gameObject.SetActive(false);
                temp.animator.SetBool("IsOpen", true);
                VRPlayer.instance.aSource.PlayOneShot(clip);
                temp.color = Color.black;
                foreach (Renderer item in temp.meshRenderers) item.material.SetColor("_EmissionColor", temp.color);
                temp.isOpen = true;

                //生成自己的宝物
                VRModels obj = Instantiate(temp.model, temp.middle.position, temp.middle.rotation).GetComponent<VRModels>();
                Vector3 tempScale = obj.transform.localScale;
                obj.transform.localScale = Vector3.zero;
                //往上升起
                obj.transform.DOScale(tempScale, 1f);
                obj.transform.DOLocalMoveY(obj.transform.position.y + 1, 1f).OnComplete(()=> {
                    //等待0.5秒
                    DOTween.To(() => Vector2.zero, x => { }, Vector2.zero, 0.5f).OnComplete(()=> {

                        //关掉特效
                        obj.particle.SetActive(false);
                        //计算目的地
                        float temp1 = 0;
                        if (VRForestPlayer.instance.disSlider.value <= 0) temp1 = 0.01f;
                        else temp1 = VRForestPlayer.instance.disSlider.value;

                        temp1 =  (0.4f / 0.25f) * temp1;

                        Vector3 distance = obj.transform.position - VRPlayer.instance.RayCamera.transform.position;
                        distance = Vector3.Normalize(distance) * temp1 + VRPlayer.instance.RayCamera.transform.position;
                        obj.transform.DOMove(distance, 7).SetEase(Ease.OutQuint).OnComplete(()=> {

                            //之后再等待1秒钟， 消失即可 
                            DOTween.To(() => Vector2.zero, x => { }, Vector2.zero, 1).OnComplete(()=> {

                                //双双消失
                                temp.transform.DOScale(Vector3.zero, 0.5f);
                                obj.transform.DOScale(Vector3.zero, 0.5f).OnComplete(()=> {

                                    Destroy(obj.gameObject);
                                    Destroy(temp.gameObject);
                                    //继续前行

                                    if (VRForestPlayer.instance.isFixed == false)
                                    {
                                        VRPlayer.instance.KeepPlaying();
                                    }
                                    else
                                    {
                                        VRForestPlayer.instance.OpenBack();
                                    }

                                    //打开中心点
                                    VRPlayer.instance.isClosePoint = false;
                                });
                                

                            });
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
