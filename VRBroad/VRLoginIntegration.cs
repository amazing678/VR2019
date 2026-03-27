using UnityEngine;
using TMPro;
using VRKeys; // 引入 VRKeys 命名空间

public class VRLoginIntegration : MonoBehaviour
{
    [Header("VRKeys 核心大键盘")]
    public Keyboard vrKeyboard;

    [Header("场景中的输入框 (仅供初始化使用)")]
    public TMP_InputField accountInput;
    public TMP_InputField passwordInput;

    // 当前正在接收键盘输入的框
    private TMP_InputField currentField;

    void Start()
    {
        // 监听 VRKeys 键盘的打字和回车事件
        if (vrKeyboard != null)
        {
            vrKeyboard.OnUpdate.AddListener(HandleKeyboardUpdate);
            vrKeyboard.OnSubmit.AddListener(HandleKeyboardSubmit);
        }
        else
        {
            Debug.LogError("VRLoginIntegration: 未绑定 VR Keyboard！请在面板中拖入。");
        }

        // 游戏刚启动时，默认激活账号框
        if (accountInput != null)
        {
            SelectField(accountInput.gameObject);
        }
    }

    /// <summary>
    /// 利用凝视按钮(VRGazeButton)的 OnClick 事件调用此方法
    /// 参数改为 GameObject，彻底解决 Unity 面板拖拽时的类型转换报错
    /// </summary>
    public void SelectField(UnityEngine.Object fieldObj)
    {
        if (fieldObj == null) return;

        // 【剥洋葱魔法】：不管面板里拖进来的是 GameObject 还是 Component，统统转换成 GameObject
        GameObject targetGo = null;

        if (fieldObj is GameObject)
        {
            targetGo = (GameObject)fieldObj;
        }
        else if (fieldObj is Component)
        {
            // 如果 Unity 偷偷传了个组件过来，我们就顺藤摸瓜找到它挂载的本体
            targetGo = ((Component)fieldObj).gameObject;
        }

        // 找到了本体后，再去安全地获取输入框组件
        if (targetGo != null)
        {
            TMP_InputField targetField = targetGo.GetComponent<TMP_InputField>();

            if (targetField != null)
            {
                currentField = targetField;

                // 把框里已有的字同步给键盘
                if (vrKeyboard != null)
                {
                    vrKeyboard.Enable();
                    vrKeyboard.SetText(currentField.text);
                }

                Debug.Log("✅ 已成功切换输入焦点到：" + targetGo.name);
            }
            else
            {
                Debug.LogError("❌ 物体 [" + targetGo.name + "] 身上没有找到 TMP_InputField 组件！");
            }
        }
    }

    /// <summary>
    /// 当在 VRKeys 上敲击任意字母退格键时，会自动触发这里
    /// </summary>
    private void HandleKeyboardUpdate(string text)
    {
        if (currentField != null)
        {
            // 实时同步打出的字到当前的输入框里
            currentField.text = text;
        }
    }

    /// <summary>
    /// 当在 VRKeys 上凝视 Enter 回车键时，会触发这里
    /// </summary>
    private void HandleKeyboardSubmit(string text)
    {
        Debug.Log("🚀 【系统提示】用户点击了回车键！");
        Debug.Log("当前输入框的最终内容：" + text);

        // TODO: 在这里衔接你真正的登录/注册核对逻辑
        // 例如：
        // string acc = accountInput.text;
        // string pwd = passwordInput.text;
        // 验证账号密码...
    }

    void OnDestroy()
    {
        // 良好的 TA 习惯：脚本销毁时注销事件监听，防止内存泄漏或报错
        if (vrKeyboard != null)
        {
            vrKeyboard.OnUpdate.RemoveListener(HandleKeyboardUpdate);
            vrKeyboard.OnSubmit.RemoveListener(HandleKeyboardSubmit);
        }
    }
}