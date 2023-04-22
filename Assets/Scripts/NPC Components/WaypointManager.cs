using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    // Nodes
    // contains empty GameObjects that act as waypoints for the room the guards are a part of
    private Queue<Transform> Nodes;
    public Queue<Transform> nodes
    {
        get
        {
            if (Nodes == null)
            {
                Nodes = new Queue<Transform>(GetComponentsInChildren<Transform>());
                Nodes = new Queue<Transform>(Nodes.Where(x => x.parent == transform));
            }
            return Nodes;
        }
        private set { }
    }
}
