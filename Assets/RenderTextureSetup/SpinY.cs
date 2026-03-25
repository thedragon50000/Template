using UnityEngine;
using System.Collections;

// @kurtdekker

public class SpinY : MonoBehaviour
{
	void Update ()
	{
		transform.rotation = Quaternion.Euler (0, 100 * Time.time, 0);
	}
}
