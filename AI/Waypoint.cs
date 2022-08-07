using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YourVRExperience.Utils
{
    [Serializable]
    public class Waypoint
    {
        public enum ActionsPatrol { GO = 0, STAY, LOOK }
        public ActionsPatrol Action;
        public GameObject Target;
        public Vector3 Position;
        public float Duration;

        public Waypoint Clone()
        {
            Waypoint output = new Waypoint();
            output.Target = new GameObject();
            if (Target != null)
            {
                output.Target.transform.position = Target.transform.position;
                GameObject.Destroy(output.Target, 2);
            }
            output.Position = Position;
            output.Duration = Duration;
            output.Action = Action;
            return output;
        }

        public void Set(Waypoint _waypoint)
        {
            Target = _waypoint.Target;
            Position = _waypoint.Position;
            Duration = _waypoint.Duration;
            Action = _waypoint.Action;
        }

    }
}