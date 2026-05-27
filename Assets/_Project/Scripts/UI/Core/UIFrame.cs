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
    private Queue<UIWindow> windowQueue = new Queue<UIWindow>();
    private Dictionary<UIWindow,WindowNode> windowNodeMap = new Dictionary<UIWindow, WindowNode>();
    private WindowNode head;          // 栈顶
    private int windowCount = 0;      // 用于快速判断是否为空

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
        if(windowNodeMap.ContainsKey(window))
        {
            BringWindowToFront(window);
            return;
        }
            
        WindowNode windowNode = new WindowNode(window);
        windowNodeMap[window] = windowNode;
        if (head != null)
        {
            head.window.OnPause();
            windowNode.next = head;
            head.prev = windowNode;
            head = windowNode;
        }
        else
        {
            head = windowNode;
        }
        windowCount++;
        window.gameObject.SetActive(true);
        window.isActive = true;
        window.OnEnter();
        if (pageName == "SettingsWindow")
        {
            window.transform.SetAsLastSibling();
        }
    }
    private static int dynamicWindowCounter = 0;
    public UIWindow OpenDynamicWindow(UIWindow prefab)
    {
        if (prefab == null) return null;

        // 生成唯一名称（使用时间戳+随机数避免冲突）
        dynamicWindowCounter++;
        string uniqueName = $"DynamicWin_{dynamicWindowCounter}";
        // 实例化并设置父物体
        UIWindow instance = Instantiate(prefab, windowLayer);
        instance.pageName = uniqueName;
        instance.name = uniqueName;
        instance.isDynamicWindow = true;

        // 注册到字典（唯一名，不会冲突）
        RegisterPage(instance);

        // 压入栈并激活
        WindowNode windowNode = new WindowNode(instance);
        windowNodeMap[instance] = windowNode;
        if (head != null)
        {
            head.window.OnPause();
            head.prev = windowNode;
            windowNode.next = head;
            head = windowNode;
        }
        else
        {
            head = windowNode;
        }
        windowCount++;
        instance.gameObject.SetActive(true);
        instance.isActive = true;
        instance.OnEnter();
        return instance;

    }
    public void BringWindowToFront(UIWindow window)
    {
        if (window == null || windowCount == 0 || head.window == window)
            return; // 已经是栈顶，无需操作
        windowNodeMap.TryGetValue(window, out var windowNode);
        if(windowNode.next != null)
        windowNode.next.prev = windowNode.prev;
        if(windowNode.prev != null)
        windowNode.prev.next = windowNode.next;
        head.prev = windowNode;
        windowNode.next = head;
        windowNode.prev = null;
        head.window.OnPause();
        head = windowNode;
        // 视觉层级置顶
        window.transform.SetAsLastSibling();

        // 如果该窗口之前处于暂停状态，现在恢复它
        window.OnResume();
    }
    public void CloseAllWindow()
    {
        while (head != null)
        {
            CloseSpecificWindow(head.window);
        }
    }
    public void CloseWindow()
    {
        if (windowCount == 0) return;
        CloseSpecificWindow(head.window);
    }
    public void CloseSpecificWindow(UIWindow sWindow)
    {
        if (windowCount == 0) return;
        if (!windowNodeMap.TryGetValue(sWindow,out var node)) return;
        sWindow.OnExit();
        sWindow.gameObject.SetActive(false);
        sWindow.isActive = false;
        bool wasHead = (node == head);
        if (wasHead)
        {
            head = head.next;
            if (head != null)
            {
                head.prev = null;
                head.window.OnResume(); // 新栈顶恢复
                head.window.gameObject.SetActive(true);
                head.window.isActive = true;
            }
            
        }
        else
        {

            if (node.next != null)
                node.next.prev = node.prev;
            if (node.prev != null)
                node.prev.next = node.next;
        }
        windowNodeMap.Remove(sWindow);
        windowCount--;
        if (sWindow.isDynamicWindow)
        {
            pageDict.Remove(sWindow.pageName);
            Destroy(sWindow.gameObject);
        }


    }
    public int WindowCount => windowCount;

    // 获取当前栈顶窗口（仍保留 CurrentPanel 是 Panel 专属，这里提供 CurrentWindow）
    public UIWindow CurrentWindow => head?.window;

    // 检查某个窗口是否在显示中
    public bool IsWindowActive(UIWindow window)
    {
        return windowNodeMap.ContainsKey(window);
    }

    // 获取所有窗口列表（从栈顶到栈底）
    public List<UIWindow> GetAllWindows()
    {
        var list = new List<UIWindow>();
        var cur = head;
        while (cur != null)
        {
            list.Add(cur.window);
            cur = cur.next;
        }
        return list;
    }
    private class WindowNode
    {
        public UIWindow window;
        public WindowNode prev;
        public WindowNode next;
        public WindowNode(UIWindow window)
        {
            this.window = window;
        }
    }
}
