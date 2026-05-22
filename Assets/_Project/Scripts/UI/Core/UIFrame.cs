using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class UIFrame : MonoBehaviour
{
    public static UIFrame Instance { get; private set; }

    [SerializeField] private Transform panelLayer;
    [SerializeField] private Transform windowLayer;

    private Dictionary<string, UIBase> pageDict = new Dictionary<string, UIBase>();
    private UIBase currentPanel;
    public UIBase CurrentPanel => currentPanel;
    private Stack<UIWindow> windowStack = new Stack<UIWindow>();
    private Queue<UIWindow> windowQueue = new Queue<UIWindow>();

    private void Awake()
    {
        if (Instance) Destroy(gameObject);
        else Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterPage(UIBase page)
    {
        if (!pageDict.ContainsKey(page.pageName))
            pageDict.Add(page.pageName, page);
        page.gameObject.SetActive(false);//入字典加隐藏
    }

    public void OpenPanel(string pageName)
    {
        if (!pageDict.TryGetValue(pageName, out var page)) return;//判断是否有这个页面
        if (currentPanel != null && currentPanel != page)
        {
            ClosePanel();
        }
        currentPanel = page;
        page.gameObject.SetActive(true);
        currentPanel.isActive = true;
        page.OnEnter();
    }
    public void ClosePanel()
    {
        if (currentPanel != null)
        {
            currentPanel.OnExit();
            currentPanel.gameObject.SetActive(false);
            currentPanel.isActive = false;
            currentPanel = null;
        }
    }
    public void OpenWindow(string pageName)
    {
        if (!pageDict.TryGetValue(pageName, out var page)) return;
        var window = page as UIWindow;
        if (window == null) return;

        if (windowStack.Count > 0)
            windowStack.Peek().OnPause();

        windowStack.Push(window);
        window.gameObject.SetActive(true);
        window.isActive = true;
        window.OnEnter();
    }
    public UIWindow OpenDynamicWindow(UIWindow prefab)
    {
        if (prefab == null) return null;

        // 生成唯一名称（使用时间戳+随机数避免冲突）
        string uniqueName = $"DynamicWin_{System.DateTime.Now.Ticks}_{Random.Range(0, 10000)}";

        // 实例化并设置父物体
        UIWindow instance = Instantiate(prefab, windowLayer);
        instance.pageName = uniqueName;
        instance.name = uniqueName;
        instance.isDynamicWindow = true;

        // 注册到字典（唯一名，不会冲突）
        RegisterPage(instance);

        // 压入栈并激活
        if (windowStack.Count > 0)
            windowStack.Peek().OnPause();

        windowStack.Push(instance);
        instance.gameObject.SetActive(true);
        instance.isActive = true;
        instance.OnEnter();
        return instance;

    }
    public void BringWindowToFront(UIWindow window)
    {
        if (window == null || windowStack.Count == 0 || windowStack.Peek() == window)
            return; // 已经是栈顶，无需操作

        // 从栈中移除该窗口
        var tempList = new List<UIWindow>(windowStack);
        if (!tempList.Contains(window)) return;
        tempList.Remove(window);

        // 暂停当前栈顶（将被压到下面）
        if (windowStack.Count > 0)
            windowStack.Peek().OnPause();

        // 重建栈：先压入原来的窗口（顺序不变），再把目标窗口压入栈顶
        windowStack.Clear();
        for (int i = tempList.Count - 1; i >= 0; i--) // 反向遍历：先A，再B，最后C
        {
            windowStack.Push(tempList[i]);
        }
        windowStack.Push(window);

        // 视觉层级置顶
        window.transform.SetAsLastSibling();

        // 如果该窗口之前处于暂停状态，现在恢复它
        window.OnResume();
    }

    public void CloseWindow()
    {
        if (windowStack.Count == 0) return;
        CloseSpecificWindow(windowStack.Peek());
    }
    public void CloseSpecificWindow(UIWindow sWindow)
    {
        if (windowStack.Count == 0) return;
        if (!windowStack.Contains(sWindow)) return;
        sWindow.OnExit();
        sWindow.gameObject.SetActive(false);
        sWindow.isActive = false;
        if (windowStack.Peek() != sWindow)
        {

            var tempList = new List<UIWindow>(windowStack);
            if (!tempList.Contains(sWindow)) return;
            tempList.Remove(sWindow);
            windowStack.Clear();
            for (int i = tempList.Count - 1; i >= 0; i--) // 反向遍历：先A，再B，最后C
            {
                windowStack.Push(tempList[i]);
            }

        }
        else
        {
            windowStack.Pop();
            if (windowStack.Count > 0)
            windowStack.Peek().OnResume();
        }
        if (sWindow.isDynamicWindow)
        {
            pageDict.Remove(sWindow.pageName);
            Destroy(sWindow.gameObject);
        }


    }
}
