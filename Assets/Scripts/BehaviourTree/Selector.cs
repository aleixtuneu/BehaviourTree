using System.Collections.Generic;
using UnityEngine;

public class Selector : Node
{
    private List<Node> _children;
    public Selector(List<Node> children) => _children = children;

    public override NodeState Evaluate()
    {
        foreach (var child in _children)
        {
            var result = child.Evaluate();
            if (result != NodeState.Failure)
            {
                return result;
            }           
        }
        return NodeState.Failure;
    }
}
