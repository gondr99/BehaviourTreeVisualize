using System;
using BTVisual;
using UnityEditor;
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
    }

    private void OnSelectionChange()
    {
        //마우스로 클릭한 오브젝트가 BehaviourTree라면
        BehaviourTree tree = Selection.activeObject as BehaviourTree;

        if (tree != null)
        {
            _treeView.PopulateView(tree);
        }
    }
}
