using UnityEngine;

namespace YourVRExperience.Utils
{
    public interface ICameraPlayer
    {
        Vector3 PositionCamera { get; }
        Vector3 ForwardCamera { get; set; }
        GameObject GetGameObject();
        bool IsOwner();
    }
}