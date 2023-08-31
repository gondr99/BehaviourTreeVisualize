using System;
using BTVisual;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

public class BTEditor : EditorWindow
{
    
        
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    private BehaviourTreeView _treeView;
    private InspectorView _inspectorView;
    

    [MenuItem("Window/BTEditor")]
    public static void OpenWindow()
    {
        BTEditor wnd = GetWindow<BTEditor>();
        wnd.titleContent = new GUIContent("BTEditor");
    }

    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceId, int line)
    {
        if (Selection.activeObject is BehaviourTree)
        {
            OpenWindow();
            return true;
        }

        return false;
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        
        // Instantiate UXML
        var template = m_VisualTreeAsset.Instantiate();
        template.style.flexGrow = 1;
        root.Add(template);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/BTVisual/Editor/BTEditor.uss");
        root.styleSheets.Add(styleSheet);

        _treeView = root.Q<BehaviourTreeView>("TreeView"); //이름은 생략해도 동작한다.
        _inspectorView = root.Q<InspectorView>("Inspector");

        _treeView.OnNodeSelected += OnSelectionNodeChanged;
        OnSelectionChange();
    }

    /// <summary>
    /// 그래프에서 선택된 노드가 변경되었을 때
    /// </summary>
    /// <param name="nodeView">그래프 노드</param>
    private void OnSelectionNodeChanged(NodeView nodeView)
    {
        _inspectorView.UpdateSelection(nodeView);
    }

    private void OnSelectionChange()
    {
        //마우스로 클릭한 오브젝트가 BehaviourTree라면
        BehaviourTree tree = Selection.activeObject as BehaviourTree;

        if (tree != null && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
        {
            _treeView.PopulateView(tree);
        }
    }
    
    
}
