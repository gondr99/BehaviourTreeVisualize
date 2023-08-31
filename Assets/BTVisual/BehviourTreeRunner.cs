using UnityEngine;

namespace BTVisual
{
    public class BehviourTreeRunner : MonoBehaviour
    {
        private BehaviourTree _tree;
        
        void Start()
        {
            _tree = ScriptableObject.CreateInstance<BehaviourTree>();

            var wait1 = ScriptableObject.CreateInstance<WaitNode>();
            wait1.duration = 2;
            
            var debug1 = ScriptableObject.CreateInstance<DebugNode>();
            debug1.message = "Hello GGM1";
            var debug2 = ScriptableObject.CreateInstance<DebugNode>();
            debug2.message = "Hello GGM2";
            var debug3 = ScriptableObject.CreateInstance<DebugNode>();
            debug3.message = "Hello GGM3";

            var seq = ScriptableObject.CreateInstance<SequenceNode>();
            seq.children.Add(wait1);
            seq.children.Add(debug1);
            seq.children.Add(debug2);
            seq.children.Add(debug3);
            
            var repeatNode = ScriptableObject.CreateInstance<RepeatNode>();
            repeatNode.child = seq;

            _tree.rootNode = repeatNode;
        }
        
        void Update()
        {
            _tree.Update();
        }
    }
}
