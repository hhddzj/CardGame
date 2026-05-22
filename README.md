# Unity UI Framework (UIProject)

> 一套基于 Panel/Window 分层设计的模块化 UI 管理框架，支持栈管理、快捷键控制、动态窗口与拖拽。

## ✨ 特性

- **双层架构**：Panel（独占式面板）与 Window（弹窗栈），职责分离。
- **生命周期管理**：OnEnter / OnPause / OnResume / OnExit 完整回调。
- **字典注册表**：通过名称快速查找页面，无需拖拽引用。
- **弹窗栈**：支持压栈、暂停、恢复、关闭、置顶。
- **动态窗口**：可生成临时窗口并自定义内容，关闭后自动销毁。
- **快捷键系统**：基于 Unity New Input System，可配置 ESC/Tab/M 等映射。
- **拖拽移动**：窗口可拖拽并限制在屏幕内。
- **DOTween 动画**：入场弹性缩放效果。

## 🧱 架构概览
Canvas
├── PanelLayer ← 所有 Panel（主菜单、背包、地图）
│ ├── MainPanel
│ ├── BagPanel
│ └── MapPanel
└── WindowLayer ← 所有 Window（设置、动态弹窗）
└── (动态生成的窗口)

- **UIFrame**：全局单例，统一管理注册、打开、关闭。
- **UIBase → UIPanel / UIWindow**：页面基类与标记类。
- **UIInputHandler**：输入映射集中处理器。

## 🚀 快速开始

1. 克隆仓库：`git clone git@github.com:hhddzj/UIProject.git`
2. 用 Unity 打开项目（版本 2021.3+ 推荐）。
3. 打开场景 `Scenes/MainScene`（或你的主场景）。
4. 确保场景中有：
   - `UIFrame` 物体（挂载 UIFrame 脚本，已配置 Panel/Window Layer）。
   - `InputHandler` 物体（挂载 UIInputHandler，已配置快捷键映射）。
   - Canvas 下存在 `PanelLayer` 和 `WindowLayer` 节点。
5. 运行游戏，按 **ESC** 打开主菜单，**Tab** 打开背包，**M** 打开地图。

## 📖 使用指南

### 添加一个新 Panel
1. 在 `PanelLayer` 下创建 UI 物体，挂载继承自 `UIPanel` 的脚本（如 `NewPanel.cs`）。
2. 设置 `pageName`（如 `"NewPanel"`）。
3. 在 `GameInitializer` 中注册：

   UIFrame.Instance.RegisterPage(newPanelInstance);
📦 依赖
Unity 2021.3+（或你实际版本）

TextMeshPro（通常随 Unity 安装）

DOTween（免费版，用于动画）

Unity Input System（Package Manager 安装）

Unity UI（内置）

项目结构
Assets/
└── _Project/
    ├── Scripts/
    │   └── UI/
    │       ├── Core/
    │       │   ├── UIFrame.cs          ← 框架核心
    │       │   ├── UIBase.cs           ← 页面基类
    │       │   └── DraggableWindow.cs  ← 拖拽组件
    │       ├── Panels/                 ← Panel 脚本
    │       │   ├── MainPanel.cs
    │       │   ├── BagPanel.cs
    │       │   └── MapPanel.cs
    │       └── Windows/                ← Window 脚本
    │           └── TestWindow.cs
    ├── Prefabs/UI/                     ← UI 预制体
    ├── InputActions/                   ← InputSystem 资产
    └── Scenes/                         ← 场景文件
后续计划
加入UI显示队列，让玩家可以通过点击队列中的图标来选择弹窗
加入弹窗的最小化功能
