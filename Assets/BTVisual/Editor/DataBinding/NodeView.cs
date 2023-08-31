using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BTVisual
{
    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        public Node node;
        public Port input;
        public Port output;
        public Action<NodeView> OnNodeSelected;
        public NodeView(Node node)
        {
            this.node = node;
            this.title = node.name;
            this.viewDataKey = node.guid; //고유 아이디 통일
            style.left = node.position.x;
            style.top = node.position.y;

            CreateInputPorts();
            CreateOutputPorts();
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            node.position.x = newPos.xMin;
            node.position.y = newPos.yMin; //좌측 상단을 저장함. 좌표는 좌상부터 0, 0임
        }

        public override void OnSelected()
        {
            base.OnSelected();
            OnNodeSelected?.Invoke(this);
        }

        private void CreateInputPorts()
        {
            //각 노드별로 만들어지는 포트가 달라야한다. 
            if (node is ActionNode)
            {
                input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            }else if (node is CompositeNode)
            {
                input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            }else if (node is DecoratorNode)
            {
                input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            }

            if (input != null)
            {
                input.portName = "";
                inputContainer.Add(input);
            }
        }
        private void CreateOutputPorts()
        {
            //각 노드별로 만들어지는 포트가 달라야한다. 
            if (node is ActionNode)
            {
                //액션노드는 아웃풋이 없다.
            }else if (node is CompositeNode)
            {
                //컴포짓 노드는 여러개의 아웃풋을 가진다.
                output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
            }else if (node is DecoratorNode)
            {
                output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            }
            
            if (output != null)
            {
                output.portName = "";
                outputContainer.Add(output);
            }
        }
    }
}
