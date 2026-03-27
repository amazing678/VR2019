using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Collections;

public class VRBeiKe:MonoBehaviour
{
    public bool isIns;
    public Transform points;
    public Renderer[] meshRenderers;
    public bool isDistance;
    public float distance = 1;
    public void Start() {

        if (this.name == "Chuan001") {
            GetComponent<MeshCollider>().enabled = false;
            StartCoroutine(Test());
        }

        if (points == null) return;
        transform.DOMove(points.position, 5).SetEase( Ease.InOutBack);
        transform.DORotate(points.eulerAngles, 3);

    }
    public IEnumerator Test() {
        yield return new WaitForSeconds(3);
        if (this.name == "Chuan001") GetComponent<MeshCollider>().enabled = true;
    }

    public bool isOpen;
    public Color color = Color.black;
    public float time;
    static int count = 0;

    public void Update() {


    }
}
