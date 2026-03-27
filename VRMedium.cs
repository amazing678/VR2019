using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class VRMedium : MonoBehaviour
{
    public Transform obj1;
    public Transform obj2;
    public Transform obj3;
    public Transform obj4;
    public Transform obj5;

    public Transform obj1_1;
    public Transform obj2_1;
    public Transform obj3_1;
    public Transform obj4_1;
    public Transform obj5_1;

    public Color modelColor = new Color(0.3962f, 0.1925f, 0.1925f);
    public Color transColor = new Color(0.1509f, 0.1509f, 0.1509f);

    public bool isRotate;
    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer[] renders = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer item in renders) {
            item.sortingOrder = 32766;
        }
    }

    GameObject temp;


    // Update is called once per frame

    Vector3 fristVector;
    Vector3 dir;

    int ij;
    float ttt;
    bool isOver;

    void Update()
    {

        if (ij >= 5) {
            if (isOver == true) return;
            isOver = true;

            VRPlayer.instance.isClosePoint = true;

            DOTween.To(() => Vector3.zero, x => { }, Vector3.zero, 2).OnComplete(() => {
                if (VRMain.instance != null && VRMain.instance.isAllPlayer == true)
                {
                    VRForestPlayer.instance.ChangeScene();
                }
            });

            //transform.Find("Medium").DOLocalRotate(new Vector3(0,90,0), 5).OnComplete(()=> {

            //    DOTween.To(() => Vector3.zero, x => { }, Vector3.zero, 1).OnComplete(() => {
            //        transform.Find("Medium").DOLocalRotate(new Vector3(0, 0, 0), 5).OnComplete(()=> {
            //            transform.Find("Medium").DOLocalRotate(new Vector3(0, -90, 0), 5).OnComplete(() => {

            //                DOTween.To(() => Vector3.zero, x =>{ }, Vector3.zero, 1).OnComplete(() => {

            //                    transform.Find("Medium").DOLocalRotate(new Vector3(0, 0, 0), 5).OnComplete(() => {
            //                        DOTween.To(()=> Vector3.zero, x=> { }, Vector3.zero, 2).OnComplete(()=> {
            //                            if (VRMain.instance != null && VRMain.instance.isAllPlayer == true)
            //                            {
            //                                VRForestPlayer.instance.ChangeScene();
            //                            }
            //                        });
            //                    });
            //                });
            //            });
            //        });
            //    });
            //});
            //if((ttt += Time.deltaTime) > 1)
            //    transform.Rotate(0,0.3f, 0);

            //if ((ttt += Time.deltaTime) > 5) {

            //    if (VRMain.instance != null && VRMain.instance.isAllPlayer == true) {
            //        VRForestPlayer.instance.ChangeScene();
            //     }
            // }
            return;
        }


        if (VRForestPlayer.instance.isPin == false) return;


        Ray ray1 = VRPlayer.instance.RayCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        RaycastHit hit1;
        if (Physics.Raycast(ray1, out hit1, Mathf.Infinity, 1 << LayerMask.NameToLayer("Cube")))
        {
            dir = hit1.point;
        }

        if (temp != null){

            temp.transform.parent.position = dir;
            print(Vector3.Distance(temp.transform.position, temp.transform.parent.parent.position));
            if (Vector3.Distance(temp.transform.position, temp.transform.parent.parent.position) < 0.5f) {
                temp.transform.position = temp.transform.parent.parent.position;
                Destroy(temp.GetComponent<Collider>());
                temp.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.black);
                Destroy(temp.transform.parent.parent.GetComponent<MeshRenderer>());
                temp = null;
                ij++;
            }

            

            return;

        }else{
            Ray ray = VRPlayer.instance.RayCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("TT"))){
                temp = hit.collider.gameObject;
                temp.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", modelColor);
                temp.transform.parent.parent.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", transColor);
            }
        }
        
    }
}
