using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using SWS;
using UnityEngine.SceneManagement;

public class VRShongShu : MonoBehaviour
{
    public splineMove spline;
    public Animator animator;
    public Renderer[] meshRenderers;
    public Texture texture;
    public VRPlayer player;
    public AudioClip clip;
    public void Awake() {
        player = VRPlayer.instance;
    }
    public void Stay()
    {
        if (isOpen == true) return;
        spline.Pause();
        animator.SetBool("IsIdle", true);
    }

    
    public void Close()
    {
        VRSongShuPlayer.instance.count++;
        
        if (VRSongShuPlayer.instance.count == 5)
        {
            //VRMain.instance.sceneName = "Hudie";
            //SceneManager.LoadScene(1);
            if (VRMain.instance != null && VRMain.instance.isAllPlayer == true) {
                VRSongShuPlayer.instance.ChangeScene();
                return;
            }
        }
        VRSongShuPlayer.instance.testCount++;
        if (VRSongShuPlayer.instance.testCount >= 3) VRSongShuPlayer.instance.testCount = 0;
        VRSongShuPlayer.instance.InstancePoint(VRSongShuPlayer.instance.testCount);

        Destroy(gameObject);

        
    }

    Color color = Color.black;
    bool isOpen = false;
    VRShongShu temp;
    float time;

    float timeX = 3;
    void Update()
    {
        if (VRPlayer.instance.RayCamera == null) return;
        if ((timeX -= Time.deltaTime) > 0) return;
        Ray ray = VRPlayer.instance.RayCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("SongShu")))
        {
            temp = hit.collider.GetComponentInParent<VRShongShu>();
            if (temp.isOpen == true) return;

            temp.color = Color.Lerp(temp.color, Color.red, Time.deltaTime * 0.5f);
            foreach (Renderer item in temp.meshRenderers) item.material.SetColor("_EmissionColor", temp.color);
            if ((temp.time += Time.deltaTime) > 3)
            {
                //关掉自己的特效
                temp.color = Color.black;
                foreach (Renderer item in temp.meshRenderers) {
                    item.material.SetColor("_EmissionColor", temp.color);
                    item.material.mainTexture = temp.texture;
                }
                temp.isOpen = true;

                VRPlayer.instance.aSource.PlayOneShot(clip);
                temp.animator.SetBool("IsRun", true);
                temp.spline.Resume();
                temp.spline.ChangeSpeed (3);
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
