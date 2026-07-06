using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using UnityEngine;

/// <summary>
/// UI框架管理器，负责管理所有UI页面的注册、打开、关闭和层级管理
/// 采用单例模式，确保全局唯一
/// </summary>
public class UIFrame : MonoBehaviour
{
    /// <summary>
    /// UI框架单例实例
    /// </summary>
    public static UIFrame Instance { get; private set; }

    /// <summary>
    /// 面板层Transform（用于放置全屏面板）
    /// </summary>
    [SerializeField] private Transform panelLayer;

    /// <summary>
    /// 窗口层Transform（用于放置弹出窗口）
    /// </summary>
    [SerializeField] private Transform windowLayer;

    /// <summary>
    /// 页面名称到页面实例的映射字典
    /// </summary>
    private Dictionary<string, UIBase> pageDict = new Dictionary<string, UIBase>();

    /// <summary>
    /// 当前打开的面板
    /// </summary>
    private UIBase currentPanel;

    /// <summary>
    /// 获取当前打开的面板
    /// </summary>
    public UIBase CurrentPanel => currentPanel;

    /// <summary>
    /// 窗口队列（未使用，保留）
    /// </summary>
    private Queue<UIWindow> windowQueue = new Queue<UIWindow>();

    /// <summary>
    /// 窗口到链表节点的映射字典
    /// </summary>
    private Dictionary<UIWindow, WindowNode> windowNodeMap = new Dictionary<UIWindow, WindowNode>();

    /// <summary>
    /// 窗口链表头节点（栈顶）
    /// </summary>
    private WindowNode head;

    /// <summary>
    /// 当前窗口数量（用于快速判断是否为空）
    /// </summary>
    private int windowCount = 0;

    /// <summary>
    /// 初始化单例，确保场景切换时不被销毁
    /// </summary>
    private void Awake()
    {
        if (Instance) Destroy(gameObject);
        else Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 注册UI页面到框架
    /// </summary>
    /// <param name="page">要注册的页面</param>
    public void RegisterPage(UIBase page)
    {
        if (!pageDict.ContainsKey(page.pageName))
            pageDict.Add(page.pageName, page);
        page.gameObject.SetActive(false);
    }

    /// <summary>
    /// 打开指定名称的面板
    /// </summary>
    /// <param name="pageName">面板名称</param>
    public void OpenPanel(string pageName)
    {
        if (!pageDict.TryGetValue(pageName, out var page)) return;
        if (currentPanel != null && currentPanel != page)
        {
            ClosePanel();
        }
        currentPanel = page;
        page.gameObject.SetActive(true);
        currentPanel.isActive = true;
        page.OnEnter();
    }

    /// <summary>
    /// 关闭当前面板
    /// </summary>
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

    /// <summary>
    /// 打开指定名称的窗口
    /// </summary>
    /// <param name="pageName">窗口名称</param>
    public void OpenWindow(string pageName)
    {
        if (!pageDict.TryGetValue(pageName, out var page)) return;
        var window = page as UIWindow;
        if (window == null) return;
        if (windowNodeMap.ContainsKey(window))
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

    /// <summary>
    /// 动态窗口计数器，用于生成唯一名称
    /// </summary>
    private static int dynamicWindowCounter = 0;

    /// <summary>
    /// 打开动态窗口（运行时创建，关闭时销毁）
    /// </summary>
    /// <param name="prefab">窗口预制件</param>
    /// <returns>创建的窗口实例</returns>
    public UIWindow OpenDynamicWindow(UIWindow prefab)
    {
        if (prefab == null) return null;

        dynamicWindowCounter++;
        string uniqueName = $"DynamicWin_{dynamicWindowCounter}";

        UIWindow instance = Instantiate(prefab, windowLayer);
        instance.pageName = uniqueName;
        instance.name = uniqueName;
        instance.isDynamicWindow = true;

        RegisterPage(instance);

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

    /// <summary>
    /// 将指定窗口置顶显示
    /// </summary>
    /// <param name="window">要置顶的窗口</param>
    public void BringWindowToFront(UIWindow window)
    {
        if (window == null || windowCount == 0 || head.window == window)
            return;

        windowNodeMap.TryGetValue(window, out var windowNode);
        if (windowNode.next != null)
            windowNode.next.prev = windowNode.prev;
        if (windowNode.prev != null)
            windowNode.prev.next = windowNode.next;
        head.prev = windowNode;
        windowNode.next = head;
        windowNode.prev = null;
        head.window.OnPause();
        head = windowNode;

        window.transform.SetAsLastSibling();

        window.OnResume();
    }

    /// <summary>
    /// 关闭所有窗口
    /// </summary>
    public void CloseAllWindow()
    {
        while (head != null)
        {
            CloseSpecificWindow(head.window);
        }
    }

    /// <summary>
    /// 关闭栈顶窗口
    /// </summary>
    public void CloseWindow()
    {
        if (windowCount == 0) return;
        CloseSpecificWindow(head.window);
    }

    /// <summary>
    /// 关闭指定窗口
    /// </summary>
    /// <param name="sWindow">要关闭的窗口</param>
    public void CloseSpecificWindow(UIWindow sWindow)
    {
        if (windowCount == 0) return;
        if (!windowNodeMap.TryGetValue(sWindow, out var node)) return;

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
                head.window.OnResume();
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

    /// <summary>
    /// 获取当前窗口数量
    /// </summary>
    public int WindowCount => windowCount;

    /// <summary>
    /// 获取当前栈顶窗口
    /// </summary>
    public UIWindow CurrentWindow => head?.window;

    /// <summary>
    /// 检查某个窗口是否在显示中
    /// </summary>
    /// <param name="window">要检查的窗口</param>
    /// <returns>是否在显示中</returns>
    public bool IsWindowActive(UIWindow window)
    {
        return windowNodeMap.ContainsKey(window);
    }

    /// <summary>
    /// 获取所有窗口列表（从栈顶到栈底）
    /// </summary>
    /// <returns>窗口列表</returns>
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

    /// <summary>
    /// 窗口链表节点类，用于维护窗口的层级关系
    /// </summary>
    private class WindowNode
    {
        /// <summary>
        /// 窗口实例
        /// </summary>
        public UIWindow window;

        /// <summary>
        /// 上一个节点
        /// </summary>
        public WindowNode prev;

        /// <summary>
        /// 下一个节点
        /// </summary>
        public WindowNode next;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="window">窗口实例</param>
        public WindowNode(UIWindow window)
        {
            this.window = window;
        }
    }
}
