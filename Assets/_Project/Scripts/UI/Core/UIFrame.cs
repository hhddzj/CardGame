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

    public void CloseWindow()
    {
        if (windowStack.Count == 0) return;
        var window = windowStack.Pop();
        window.OnExit();
        window.gameObject.SetActive(false);
        window.isActive = false;
        if (window.isDynamicWindow)
        {
            pageDict.Remove(window.pageName);
            Destroy(window);
        }

        if (windowStack.Count > 0)
            windowStack.Peek().OnResume();
    }
}
