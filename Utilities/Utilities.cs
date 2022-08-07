using System;
using System.Collections.Generic;
using UnityEngine;

namespace YourVRExperience.Utils
{
	public static class Utilities
	{
		public static Vector3 GetCollisionMouse(params string[] _masksToIgnore)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			int layerMask = Physics.IgnoreRaycastLayer;
			if (_masksToIgnore != null)
			{
				for (int i = 0; i < _masksToIgnore.Length; i++)
				{
					layerMask |= (1 << LayerMask.NameToLayer(_masksToIgnore[i]));
				}
				layerMask = ~layerMask;
			}
			if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
			{
				return hit.point;
			}
			else
			{
				return Vector3.zero;
			}
		}

		public static Vector3 GetDirection(Vector3 target, Vector3 origin)
		{
			return (target - origin).normalized;
		}

		public static float IsInsideCone(GameObject _source, float _angle, GameObject _objective, float _rangeDetection, float _angleDetection)
		{
			float distance = Vector3.Distance(new Vector3(_source.transform.position.x, 0, _source.transform.position.z),
											 new Vector3(_objective.transform.position.x, 0, _objective.transform.position.z));
			if (distance < _rangeDetection)
			{
				float yaw = _angle * Mathf.Deg2Rad;
				Vector2 pos = new Vector2(_source.transform.position.x, _source.transform.position.z);

				Vector2 v1 = new Vector2((float)Mathf.Cos(yaw), (float)Mathf.Sin(yaw));
				Vector2 v2 = new Vector2(_objective.transform.position.x - pos.x, _objective.transform.position.z - pos.y);

				// Angle detection
				float moduloV2 = v2.magnitude;
				if (moduloV2 == 0)
				{
					v2.x = 0.0f;
					v2.y = 0.0f;
				}
				else
				{
					v2.x = v2.x / moduloV2;
					v2.y = v2.y / moduloV2;
				}
				float angleCreated = (v1.x * v2.x) + (v1.y * v2.y);
				float angleResult = Mathf.Cos(_angleDetection * Mathf.Deg2Rad);

				if (angleCreated > angleResult)
				{
					return (distance);
				}
				else
				{
					return (-1);
				}
			}
			else
			{
				return (-1);
			}
		}

		public static void DrawAreaVision(Vector3 _posOrigin, float _orientation, GameObject _planeAreaVision, int _checkRadiusInstances, float _viewDistance, float _angleView, Material _material, float _heightToFloor)
		{
			List<Vector3> areaDetection = new List<Vector3>();
			Vector3 posOrigin = _posOrigin;
			posOrigin.y += _heightToFloor;
			areaDetection.Add(posOrigin);

			float totalAngle = 2 * _angleView * Mathf.Deg2Rad;
			float entryAngle = (_orientation + _angleView) * Mathf.Deg2Rad;
			float x = _viewDistance * Mathf.Cos(entryAngle);
			float z = _viewDistance * Mathf.Sin(entryAngle);

			Vector3 posTarget = new Vector3(posOrigin.x + x, posOrigin.y, posOrigin.z + z);
			areaDetection.Add(posTarget);

			float thetaScale = totalAngle / _checkRadiusInstances;
			for (int i = 0; i < _checkRadiusInstances; i++)
			{
				entryAngle -= thetaScale;
				x = _viewDistance * Mathf.Cos(entryAngle);
				z = _viewDistance * Mathf.Sin(entryAngle);

				Vector3 posTargetRadius = new Vector3(posOrigin.x + x, posOrigin.y, posOrigin.z + z);
				areaDetection.Add(posTargetRadius);
			}

			float endAngle = (_orientation - _angleView) * Mathf.Deg2Rad;
			x = _viewDistance * Mathf.Cos(endAngle);
			z = _viewDistance * Mathf.Sin(endAngle);
			Vector3 posTargetEnd = new Vector3(posOrigin.x + x, posOrigin.y, posOrigin.z + z);
			areaDetection.Add(posTargetEnd);
			areaDetection.Add(posOrigin);

			_planeAreaVision.GetComponent<PlaneFromPoly>().Init(areaDetection.ToArray(), _material);
			_planeAreaVision.GetComponent<PlaneFromPoly>().Logic(new Vector3(posOrigin.x, posOrigin.y, posOrigin.z), posOrigin.y);
		}


		public static bool IsThereObstacleBetweenPosition(Vector3 origin, Vector3 target, params string[] _masksToIgnore)
		{
			Ray ray = new Ray(origin, (target - origin).normalized);
			RaycastHit hit;
			int layerMask = Physics.IgnoreRaycastLayer;
			if (_masksToIgnore != null)
			{
				for (int i = 0; i < _masksToIgnore.Length; i++)
				{
					layerMask |= (1 << LayerMask.NameToLayer(_masksToIgnore[i]));
				}
				layerMask = ~layerMask;
			}
			if (Physics.Raycast(ray, out hit, Vector3.Distance(origin, target), layerMask))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static void ActivatePhysics(GameObject _target, bool _activated)
		{
			if (_target.GetComponent<Rigidbody>() != null)
			{
				_target.GetComponent<Rigidbody>().isKinematic = !_activated;
				_target.GetComponent<Rigidbody>().useGravity = _activated;
			}
		}


		public static GameObject GetCollisionMouseWithLayers(params string[] _masksToConsider)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			int layerMask = 0;
			if (_masksToConsider != null)
			{
				for (int i = 0; i < _masksToConsider.Length; i++)
				{
					layerMask |= (1 << LayerMask.NameToLayer(_masksToConsider[i]));
				}
			}
			if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
			{
				return hit.collider.gameObject;
			}
			else
			{
				return null;
			}
		}

		public static float DistanceXZ(Vector3 _one, Vector3 _two)
		{
			float x = (_one.x - _two.x);
			float z = (_one.z - _two.z);
			return Mathf.Sqrt((x * x) + (z * z));
		}

		public static List<Vector3> GetBoundaryPoints(Vector3 _origin, Vector3 _target)
		{
			List<Vector3> pointsPlane = new List<Vector3>();

			float topX = ((_origin.x < _target.x) ? _target.x : _origin.x);
			float topY = ((_origin.z < _target.z) ? _target.z : _origin.z);

			float bottomX = ((_origin.x > _target.x) ? _target.x : _origin.x);
			float bottomY = ((_origin.z > _target.z) ? _target.z : _origin.z);

			pointsPlane.Add(new Vector3(bottomX, 0, bottomY));
			pointsPlane.Add(new Vector3(bottomX, 0, topY));
			pointsPlane.Add(new Vector3(topX, 0, topY));
			pointsPlane.Add(new Vector3(topX, 0, bottomY));

			return pointsPlane;
		}

		public static void DebugLogColor(string _message, Color _color)
		{
			if (_color == Color.red)
			{
				Debug.Log("<color=red>" + _message + "</color>");
			}
			if (_color == Color.blue)
			{
				Debug.Log("<color=blue>" + _message + "</color>");
			}
			if (_color == Color.green)
			{
				Debug.Log("<color=green>" + _message + "</color>");
			}
			if (_color == Color.yellow)
			{
				Debug.Log("<color=yellow>" + _message + "</color>");
			}
		}

		public static string RandomCodeGeneration(string _idUser)
		{
			string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			var stringChars = new char[8];
			var random = new System.Random();

			for (int i = 0; i < stringChars.Length; i++)
			{
				stringChars[i] = chars[random.Next(chars.Length)];
			}

			string finalString = new String(stringChars) + "_" + _idUser;
			return finalString;
		}

		public static string SerializeVector3(Vector3 _data)
		{
			return _data.x + ";" + _data.y + ";" + _data.z;
		}

		public static Vector3 DeserializeVector(string _data)
		{
			string[] buffer = _data.Split(';');
			return new Vector3(float.Parse(buffer[0]), float.Parse(buffer[1]), float.Parse(buffer[2]));
		}
	}
}
