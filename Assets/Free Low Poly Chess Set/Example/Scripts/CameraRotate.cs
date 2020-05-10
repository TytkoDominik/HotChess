using UnityEngine;

namespace Free_Low_Poly_Chess_Set.Example.Scripts
{
	public class CameraRotate : MonoBehaviour {

		public float Speed = 8;

		// Update is called once per frame
		void FixedUpdate () {
			transform.Rotate(Vector3.up, Speed * Time.deltaTime, Space.World);
		}
	}
}
