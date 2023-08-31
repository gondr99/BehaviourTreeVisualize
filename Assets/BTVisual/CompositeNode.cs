using System.Collections.Generic;
using UnityEngine.Serialization;

namespace BTVisual
{
    public abstract class CompositeNode : Node
    {
        public List<Node> children = new List<Node>();
    }
}