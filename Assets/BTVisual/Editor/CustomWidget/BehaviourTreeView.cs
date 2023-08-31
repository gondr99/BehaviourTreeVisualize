using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BTVisual
{
    public class BehaviourTreeView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<BehaviourTreeView, UxmlTraits>
        { }

        public new class UxmlTraits : GraphView.UxmlTraits
        { }

        public Action<NodeView> OnNodeSelected;
        private BehaviourTree _tree;
        
        public BehaviourTreeView()
        {
            Insert(0, new GridBackground());
            
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
        }

        public void PopulateView(BehaviourTree tree)
        {
            _tree = tree;
            
            //기존 그래프에서 구독했던 액션을 삭제
            graphViewChanged -= OnGraphViewChanged;
            //기존에 그려졌던 그래프 엘레멘트를 삭제
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;
            
            //트리에 루트노드가 없다면 만들어준다.
            if (_tree.treeRoot == null)
            {
                tree.treeRoot = tree.CreateNode(typeof(RootNode)) as RootNode;
                EditorUtility.SetDirty(tree);
                AssetDatabase.SaveAssets();
            }
            
            //트리에 있는 모든 노드들을 노드뷰에 만들어준다.
            tree.nodes.ForEach(n => CreateNodeView(n));
            
            //트리에 있는 모든 엣지들을 만들어준다.
            tree.nodes.ForEach(n =>
            {
                var children = tree.GetChildren(n);
                NodeView parent = FindNodeView(n);
                children.ForEach(c =>
                {
                    NodeView child = FindNodeView(c);

                    //연결
                    Edge edge = parent.output.ConnectTo(child.input);
                    AddElement(edge);
                });
            });
        }

        private NodeView FindNodeView(Node node)
        {
            //Guid기반으로 그래프 뷰 상에 있는 노드뷰를 찾아온다.
            return GetNodeByGuid(node.guid) as NodeView;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            //드래깅이 시작된 노드인 startPort와 맞물릴 수 있는 노드들의 리스트를 전부 가져온다.
            // 같은 노드가 아니고 같은 입력입력 또는 아웃아웃이 아닌경우만 가져옴.
            return ports.ToList().Where(endPort =>
                endPort.direction != startPort.direction
                && endPort.node != startPort.node
            ).ToList();
        }

        /// <summary>
        /// 그래프 뷰에서 변화하는 것을 찾아서 작업해주기 위한 변화 콜백
        /// </summary>
        /// <param name="graphViewChange">변화된 내용이 들어오는 구조체(삭제, 생성, 이동)</param>
        /// <returns>입력값을 그대로 돌려주거나 변형하여 돌려준다.</returns>
        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            //삭제된 엘레멘트가 있다면
            if (graphViewChange.elementsToRemove != null)
            {
                graphViewChange.elementsToRemove.ForEach(elem =>
                {
                    var nv = elem as NodeView; //우린 전부 NodeView밖에없으니
                    if (nv != null)
                    {
                        _tree.DeleteNode(nv.node); //트리에서 해당 노드 삭제
                    }

                    var edge = elem as Edge;
                    if (edge != null)
                    {
                        NodeView parent = edge.output.node as NodeView; //나가는 쪽이 부모
                        NodeView child = edge.input.node as NodeView; //입력되는 쪽이 자식
                        _tree.RemoveChild(parent.node, child.node);
                    }
                });
            }

            if (graphViewChange.edgesToCreate != null)
            {
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    NodeView parent = edge.output.node as NodeView; //나가는 쪽이 부모
                    NodeView child = edge.input.node as NodeView; //입력되는 쪽이 자식
                    
                    _tree.AddChild(parent.node, child.node);
                });
            }
            
            return graphViewChange;
        }

        /// <summary>
        /// Node를 받아서 그래프 뷰의 노드로 만들어주는 매서드
        /// </summary>
        /// <param name="node">그래프 뷰로 만들어줄 노드</param>
        private void CreateNodeView(Node node)
        {
            NodeView nodeView = new NodeView(node);
            nodeView.OnNodeSelected = OnNodeSelected; //노드 선택시 이벤트 발동
            AddElement(nodeView);
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (_tree == null)
            {
                evt.StopPropagation();
                return;
            }
            
            {
                //ActionNode를 상속받은 모든 타입을 가져온다.
                var types = TypeCache.GetTypesDerivedFrom<ActionNode>();
                foreach (var type in types)
                {
                    evt.menu.AppendAction($"[{type.BaseType.Name} {type.Name}] ", (a) => CreateNode(type));
                }    
            }
            
            {
                //CompositeNode를 상속받은 모든 타입을 가져온다.
                var types = TypeCache.GetTypesDerivedFrom<CompositeNode>();
                foreach (var type in types)
                {
                    evt.menu.AppendAction($"[{type.BaseType.Name} {type.Name}] ", (a) => CreateNode(type));
                }    
            }
            
            {
                //Decorator 를 상속받은 모든 타입을 가져온다.
                var types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
                foreach (var type in types)
                {
                    evt.menu.AppendAction($"[{type.BaseType.Name} {type.Name}] ", (a) => CreateNode(type));
                }    
            }
        }

        private void CreateNode(System.Type type)
        {
            //트리에다가 새로운 노드 만들고 그걸 뷰에다가도 만들어주고
            Node node = _tree.CreateNode(type);
            CreateNodeView(node);
        }
    }    
}