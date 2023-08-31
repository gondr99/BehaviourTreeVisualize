using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEditor;

namespace BTVisual
{
    [CreateAssetMenu(menuName = "BehviourTree/Tree")]
    public class BehaviourTree : ScriptableObject
    {
        public Node treeRoot;
        public Node.State treeState = Node.State.RUNNING;
        
        public List<Node> nodes = new List<Node>();

        public Node.State Update()
        {
            if (treeRoot.state == Node.State.RUNNING)
            {
                treeState = treeRoot.Update();
            }
            return treeState;
        }

        /// <summary>
        /// 해당 타입의 노드를 생성함. 생성된 노드는 타입을 이름으로 받고, GUID를 생성하여 받음.
        /// </summary>
        /// <param name="type">노드 타입을 받도록 되어있음.</param>
        /// <returns>생성된 노드객체를 반환</returns>
        public Node CreateNode(System.Type type)
        {
            var node = ScriptableObject.CreateInstance(type) as Node;
            node.name = type.Name;
            node.guid = GUID.Generate().ToString();
            nodes.Add(node);
            
            AssetDatabase.AddObjectToAsset(node, this);
            AssetDatabase.SaveAssets();
            return node;
        }
        
        /// <summary>
        /// 지정된 노드를 삭제함
        /// </summary>
        /// <param name="node">삭제하고자 하는 노드</param>
        public void DeleteNode(Node node)
        {
            nodes.Remove(node);
            AssetDatabase.RemoveObjectFromAsset(node);
            AssetDatabase.SaveAssets();
        }

        public void AddChild(Node parent, Node child)
        {
            //패런트의 타입에 따라 다르게 넣어줘야 한다.
            var decorator = parent as DecoratorNode;
            if (decorator != null) //데코레이터 노드라면
            {
                decorator.child = child;
                return;
            }

            var rootNode = parent as RootNode;
            if (rootNode != null)
            {
                rootNode.child = child;
            }
            
            var composite = parent as CompositeNode;
            if (composite != null) //콤포짓 노드라면
            {
                composite.children.Add(child);
                return;
            }
        }

        public void RemoveChild(Node parent, Node child)
        {
            //패런트의 타입에 따라 다르게 삭제
            var decorator = parent as DecoratorNode;
            if (decorator != null) //데코레이터 노드라면
            {
                decorator.child = null;
                return;
            }
            
            var rootNode = parent as RootNode;
            if (rootNode != null)
            {
                rootNode.child = null;
                return;
            }
            
            var composite = parent as CompositeNode;
            if (composite != null) //콤포짓 노드라면
            {
                composite.children.Remove(child);
                return;
            }
        }

        public List<Node> GetChildren(Node parent)
        {
            List<Node> children = new List<Node>();
            
            var composite = parent as CompositeNode;
            if (composite != null) //콤포짓 노드라면
            {
                return composite.children;
            }
            
            var rootNode = parent as RootNode;
            if (rootNode != null && rootNode.child != null)
            {
                children.Add(rootNode.child);
            }
            
            var decorator = parent as DecoratorNode;
            if (decorator != null && decorator.child != null) //데코레이터 노드라면
            {
                children.Add(decorator.child);
            }
            return children;
        }
        
        public BehaviourTree Clone()
        {
            var tree = Instantiate(this);
            tree.treeRoot = tree.treeRoot.Clone();
            return tree;
        }
    }
}