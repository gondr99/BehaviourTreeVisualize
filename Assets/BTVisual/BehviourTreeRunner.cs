using System;
using UnityEngine;

namespace BTVisual
{
    public class BehviourTreeRunner : MonoBehaviour
    {
        public BehaviourTree tree;

        private void Start()
        {
            tree = tree.Clone(); //복제해서 시작함.
        }

        private void Update()
        {
            tree.Update();
        }
    }
}
