using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mario
{
	public class Rotator : MonoBehaviour
	{
		public float speed = 1f;

		void Update()
		{
			transform.Rotate(new Vector3(0, speed * Time.deltaTime, 0), Space.World);
		}
	}
}
