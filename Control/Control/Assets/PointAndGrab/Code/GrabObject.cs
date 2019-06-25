using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LesBird
{
	public class GrabObject : MonoBehaviour
	{
		[Tooltip("When true a copy of this object is instantiated")]
		public bool inLibrary;
		[Tooltip("When true this object is scaled instead of rotated")]
		public bool canScale;
		public float minScaleSize;
		public float maxScaleSize;
		[Tooltip("When true this object cannot be deleted")]
		public bool noDelete;

		// local copy of the rotation offset of this object when object is rotated instead of scaled (canScale = false)
		private float rot;
		public float rotationOffset
		{
			get { return rot; }
			set { rot = value; }
		}

		void Start()
		{
			if (minScaleSize == 0)
			{
				minScaleSize = 0.01f;
			}
			if (maxScaleSize == 0)
			{
				maxScaleSize = float.MaxValue;
			}
		}
	}
}
