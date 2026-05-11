using System;
using UnityEngine;

public class ActionNode : Node
{
    private Func<NodeState> _action;
    public ActionNode(Func<NodeState> action) => _action = action;

    public override NodeState Evaluate() => _action();
}
