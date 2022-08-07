using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace YourVRExperience.Utils
{
    public class PatrolWaypoints : StateMachine
    {
        public const string EVENT_PATROLWAYPOINTS_HAS_STARTED = "EVENT_PATROLWAYPOINTS_HAS_STARTED";
        public const string EVENT_PATROLWAYPOINTS_HAS_BEEN_DESTROYED = "EVENT_PATROLWAYPOINTS_HAS_BEEN_DESTROYED";
        public const string EVENT_PATROLWAYPOINTS_WAYPOINT_TO_CERO = "EVENT_PATROLWAYPOINTS_WAYPOINT_TO_CERO";

        public delegate void MovingEvent();
        public delegate void StandingEvent();

        public event MovingEvent MoveEvent;
        public event StandingEvent StandEvent;

        public void DispatchMovingEvent()
        {
            if (MoveEvent != null)
                MoveEvent();
        }
        public void DispatchStandingEvent()
        {
            if (StandEvent != null)
                StandEvent();
        }


        public enum WAYPOINT_ACTIONS { SYNCHRONIZATION = 0, UPDATE_WAYPOINT, GO_TO_WAYPOINT, STAY_IN_WAYPOINT, LOOK_TO_WAYPOINT }

        public Waypoint[] Waypoints;
        public int CurrentWaypoint = 0;
        public float Speed = 5;
        public bool EnableRotationToWaypoint = false;
        public bool AutoStart = false;
        public string[] MasksToIgnore;

        private bool m_activated = false;
        private float m_timeDone = 0;

        private RotateToTarget m_rotateComponent;
        private bool m_hasRigidBody;

        private NavMeshAgent m_navigationComponent;
        private bool m_activatedNavigation = false;

        public float TimeDone
        {
            get { return m_timeDone; }
            set { m_timeDone = value; }
        }

        void Start()
        {
            m_rotateComponent = this.GetComponent<RotateToTarget>();

            m_hasRigidBody = this.GetComponent<Rigidbody>() != null;

            for (int i = 0; i < Waypoints.Length; i++)
            {
                if (Waypoints[i] != null)
                {
                    if (Waypoints[i].Target != null)
                    {
                        Waypoints[i].Position = Waypoints[i].Target.transform.position;
                        GameObject.Destroy(Waypoints[i].Target);
                    }
                }
            }

            m_navigationComponent = this.GetComponent<NavMeshAgent>();
            if (m_navigationComponent != null) m_navigationComponent.enabled = false;

            if (AutoStart)
            {
                ActivatePatrol(10);
            }

            SystemEventController.Instance.DispatchSystemEvent(EVENT_PATROLWAYPOINTS_HAS_STARTED, this, AutoStart);
        }

        void OnDestroy()
        {
            if (SystemEventController.Instance != null) SystemEventController.Instance.DispatchSystemEvent(EVENT_PATROLWAYPOINTS_HAS_BEEN_DESTROYED, this);
        }
        
        public void CopyPatrolWaypoints(List<Waypoint> _waypoints)
        {
            for (int i = 0; i < Waypoints.Length; i++)
            {
                _waypoints.Add(Waypoints[i].Clone());
            }
        }

        public void SetPatrolWaypoints(List<Waypoint> _waypoints)
        {
            Waypoints = new Waypoint[_waypoints.Count];
            for (int i = 0; i < _waypoints.Count; i++)
            {
                Waypoints[i] = _waypoints[i].Clone();
            }
        }

        private Vector3 GetPreviousPositionWaypoint(int _waypointIndex)
        {
            int finalIndexCheck = _waypointIndex;
            do
            {
                finalIndexCheck = finalIndexCheck - 1;
                if (finalIndexCheck < 0)
                {
                    finalIndexCheck = Waypoints.Length - 1;
                }
            } while (Waypoints[finalIndexCheck].Action != Waypoint.ActionsPatrol.GO);

            return Waypoints[finalIndexCheck].Position;
        }

        private void WalkToCurrentWaypoint()
        {
            m_timeDone += Time.deltaTime;
            float duration = Waypoints[CurrentWaypoint].Duration;
            Vector3 origin = GetPreviousPositionWaypoint(CurrentWaypoint);
            Vector3 forwardTarget = (Waypoints[CurrentWaypoint].Position - origin);
            float increaseFactor = m_timeDone / duration;
            Vector3 nextPosition = origin + (increaseFactor * forwardTarget);
            if (m_hasRigidBody)
            {
                transform.GetComponent<Rigidbody>().MovePosition(new Vector3(nextPosition.x, transform.position.y, nextPosition.z));
            }
            else
            {
                transform.position = nextPosition;
            }
        }

        private void WalkWithSpeedToWaypoint()
        {
            Vector3 directionToTarget = Waypoints[CurrentWaypoint].Position - this.transform.position;
            directionToTarget.Normalize();
            Vector3 nextPosition = this.transform.position + (directionToTarget * Speed * Time.deltaTime);
            if (m_hasRigidBody)
            {
                transform.GetComponent<Rigidbody>().MovePosition(new Vector3(nextPosition.x, transform.position.y, nextPosition.z));
            }
            else
            {
                transform.position = nextPosition;
            }

        }

        private bool ReachedCurrentWaypoint()
        {
            if (Vector3.Distance(this.transform.position, Waypoints[CurrentWaypoint].Position) < 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ActivatePatrol(float _speed)
        {
            if (Waypoints.Length > 0)
            {
                Speed = _speed;
                m_activated = true;
                ChangeState((int)WAYPOINT_ACTIONS.SYNCHRONIZATION);
            }
        }

        public bool AreThereAnyWaypoints()
        {
            return Waypoints.Length > 0;
        }

        public void DeactivatePatrol()
        {
            m_activated = false;
            if (m_navigationComponent != null)
            {
                if (m_navigationComponent.enabled) m_navigationComponent.enabled = false;
            }
        }

        private bool IsThereRotationComponent()
        {
            return m_rotateComponent != null;
        }


        protected override void ChangeState(int newState)
        {
            base.ChangeState(newState);

            switch ((WAYPOINT_ACTIONS)m_state)
            {
                case WAYPOINT_ACTIONS.SYNCHRONIZATION:
                    Vector3 previousGoWaypoint = Waypoints[CurrentWaypoint].Position;
                    m_activatedNavigation = false;
                    if (m_navigationComponent != null)
                    {
                        Vector3 origin = this.transform.position;
                        origin.y = transform.position.y;
                        Vector3 target = previousGoWaypoint;
                        target.y = transform.position.y;
                        if (Utilities.IsThereObstacleBetweenPosition(origin, target, MasksToIgnore))
                        {
                            if (m_navigationComponent != null)
                            {
                                m_activatedNavigation = true;
                                m_navigationComponent.enabled = true;
                                m_navigationComponent.SetDestination(target);
                            }
                        }
                        else
                        {
                            if (m_navigationComponent != null)
                            {
                                m_activatedNavigation = false;
                                if (m_navigationComponent.enabled)
                                {
                                    Utilities.ActivatePhysics(this.gameObject, false);
                                    m_navigationComponent.isStopped = true;
                                    m_navigationComponent.enabled = false;
                                }
                            }
                        }
                    }
                    if (!m_activatedNavigation)
                    {
                        if (IsThereRotationComponent())
                        {
                            m_rotateComponent.ActivateRotation(previousGoWaypoint);
                        }
                    }
                    DispatchMovingEvent();
                    break;

                case WAYPOINT_ACTIONS.UPDATE_WAYPOINT:
                    break;

                case WAYPOINT_ACTIONS.GO_TO_WAYPOINT:
                    DispatchMovingEvent();
                    Utilities.ActivatePhysics(this.gameObject, true);
                    break;

                case WAYPOINT_ACTIONS.STAY_IN_WAYPOINT:
                    DispatchStandingEvent();
                    Utilities.ActivatePhysics(this.gameObject, true);
                    break;

                case WAYPOINT_ACTIONS.LOOK_TO_WAYPOINT:
                    DispatchStandingEvent();
                    Utilities.ActivatePhysics(this.gameObject, true);
                    break;
            }
        }

        public void UpdateLogic()
        {
            if (m_activated == false) return;

            switch ((WAYPOINT_ACTIONS)m_state)
            {
                case WAYPOINT_ACTIONS.SYNCHRONIZATION:
                    WalkWithSpeedToWaypoint();
                    if (ReachedCurrentWaypoint() == true)
                    {
                        ChangeState((int)WAYPOINT_ACTIONS.UPDATE_WAYPOINT);
                    }
                    break;

                case WAYPOINT_ACTIONS.UPDATE_WAYPOINT:
                    m_activatedNavigation = false;
                    if (m_navigationComponent != null)
                    {
                        if (m_navigationComponent.enabled)
                        {
                            Utilities.ActivatePhysics(this.gameObject, false);
                            m_navigationComponent.isStopped = true;
                            m_navigationComponent.enabled = false;
                        }
                    }
                    CurrentWaypoint++;
                    if (CurrentWaypoint > Waypoints.Length - 1)
                    {
                        CurrentWaypoint = 0;
                    }
                    if (IsThereRotationComponent())
                    {
                        m_rotateComponent.ActivateRotation(Waypoints[CurrentWaypoint].Position);
                    }
                    m_timeDone = 0;
                    switch (Waypoints[CurrentWaypoint].Action)
                    {
                        case Waypoint.ActionsPatrol.GO:
                            Vector3 origin = GetPreviousPositionWaypoint(CurrentWaypoint);
                            origin.y = transform.position.y;
                            Vector3 target = Waypoints[CurrentWaypoint].Position;
                            target.y = transform.position.y;
                            if (Utilities.IsThereObstacleBetweenPosition(origin, target, MasksToIgnore))
                            {
                                if (m_navigationComponent != null)
                                {
                                    m_activatedNavigation = true;
                                    m_navigationComponent.enabled = true;
                                    m_navigationComponent.SetDestination(target);
                                }
                            }
                            ChangeState((int)WAYPOINT_ACTIONS.GO_TO_WAYPOINT);
                            break;

                        case Waypoint.ActionsPatrol.STAY:
                            ChangeState((int)WAYPOINT_ACTIONS.STAY_IN_WAYPOINT);
                            break;

                        case Waypoint.ActionsPatrol.LOOK:
                            ChangeState((int)WAYPOINT_ACTIONS.LOOK_TO_WAYPOINT);
                            break;
                    }

                    if (AutoStart)
                    {
                        if (CurrentWaypoint == 0)
                        {
                            SystemEventController.Instance.DispatchSystemEvent(EVENT_PATROLWAYPOINTS_WAYPOINT_TO_CERO, this.gameObject);
                        }
                    }
                    break;

                case WAYPOINT_ACTIONS.GO_TO_WAYPOINT:
                    if (!m_activatedNavigation)
                    {
                        WalkToCurrentWaypoint();
                        if (m_timeDone > Waypoints[CurrentWaypoint].Duration)
                        {
                            ChangeState((int)WAYPOINT_ACTIONS.UPDATE_WAYPOINT);
                        }
                    }
                    else
                    {
                        if (IsThereRotationComponent())
                        {
                            m_rotateComponent.ActivateRotation(this.transform.position + m_navigationComponent.velocity.normalized);
                        }
                        if (Vector3.Distance(this.transform.position, Waypoints[CurrentWaypoint].Position) < 1)
                        {
                            ChangeState((int)WAYPOINT_ACTIONS.UPDATE_WAYPOINT);
                        }
                    }
                    break;

                case WAYPOINT_ACTIONS.STAY_IN_WAYPOINT:
                    m_timeDone += Time.deltaTime;
                    if (m_timeDone > Waypoints[CurrentWaypoint].Duration)
                    {
                        ChangeState((int)WAYPOINT_ACTIONS.UPDATE_WAYPOINT);
                    }
                    break;

                case WAYPOINT_ACTIONS.LOOK_TO_WAYPOINT:
                    m_timeDone += Time.deltaTime;
                    if (IsThereRotationComponent())
                    {
                        m_rotateComponent.ActivateRotation(Waypoints[CurrentWaypoint].Position);
                    }
                    if (m_timeDone > Waypoints[CurrentWaypoint].Duration)
                    {
                        ChangeState((int)WAYPOINT_ACTIONS.UPDATE_WAYPOINT);
                    }
                    break;
            }
        }
    }
}