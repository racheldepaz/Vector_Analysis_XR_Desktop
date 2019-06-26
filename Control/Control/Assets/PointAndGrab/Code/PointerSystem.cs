using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;

/// <summary>
/// PointerSystem.cs
/// 
/// Basic pointer system for Magic Leap. With this system you can grab objects and place them anywhere in your space. This code
/// is based on the pointer system that I used in Universe Creator.
/// 
/// Objects that have the GrabObject.inLibrary set to true are considered to be part of a library and a new copy of the object will
/// be created when you point and click it.
/// 
/// To use it do the following:
/// 1. Create an empty game object and attach this script to it.
/// 2. Attach the GrabObject.cs script to any object that you want to interact with.
/// 3. Define a root transform node that will contain all the interactable objects and assign it to the contentRoot variable.
/// 
/// That is all. Any object in the contentRoot that has GrabObject can be picked up and moved anywhere in your space. Additionally
/// objects can be defined to be scalable by setting GrabObject.canScale to true. When an object is scalable you can use the touchpad
/// left/right to increase/decrease the size of the object. Objects that are not scalable can be rotated using touchpad left/right.
/// 
/// For all objects you can use the touchpad up/down to move the object further or nearer to you.
/// 
/// You can adjust pointerAccuracy to fine tune the pointer. This is dot product based so it uses a cone to find the objects. The
/// higher the number the more exact the pointer will be. Never set this higher than 1.0. The default setting is 0.995 which will
/// work well for most apps. The pointer will snap to the most direct object that is being pointed to.
/// 
/// 2018 Les Bird
/// </summary>
namespace LesBird
{
	public class PointerSystem : MonoBehaviour
	{
		[Tooltip("1.0 = exact, 0.99 = near - this is a dot product result")]
		public float pointerAccuracy;
		[Tooltip("speed to push/pull objects using trackpad - meters per second")]
		public float pushRate;
		[Tooltip("rotate object around Y axis if grabbed - angles per second")]
		public float rotateRate;
		[Tooltip("scale rate - meters per second")]
		public float scaleRate;

		[Tooltip("transform node where all the content resides - the pointer can interact with child objects of this node")]
		public Transform contentRoot;

        public ControllerConnectionHandler handler; 
        public Text debug;

		// updated array of grabbable objects
		private GrabObject[] grabObjectArray;
		// pointer line renderer
		private LineRenderer pointerLineRenderer;

		// holds the distance from the pointer source to the grabbed object - can be adjusted to offset the object using the controller trackpad
		private float grabbedDist;
		// the object we are holding
		private GrabObject grabbedObject;

		// flags and controller variables
		private bool triggerIsDown;
		private bool lastTriggerWasUp;
		private bool bumperIsDown;
		private bool lastBumperWasUp;
		private float trackPadHor;
		private float trackPadVer;

		// object was grabbed
		public delegate void GrabObjectCallback(GrabObject obj);
		public GrabObjectCallback OnGrabObject;
		// object was dropped
		public delegate void DropObjectCallback(GrabObject obj);
		public DropObjectCallback OnDropObject;
		// object was deleted
		public delegate void DeleteObjectCallback(GrabObject obj);
		public DeleteObjectCallback OnDeleteObject;
		// trigger is held down
		public delegate void TriggerDownCallback();
		public TriggerDownCallback OnTriggerDown;
		// trigger is released
		public delegate void TriggerUpCallback();
		public TriggerUpCallback OnTriggerUp;
		// trigger was clicked (not pressed to pressed)
		public delegate void TriggerClicked();
		public TriggerClicked OnTriggerClicked;
		// bumper was clicked
		public delegate void BumperClicked();
		public BumperClicked OnBumperClicked;

		// singleton accessor
		public static PointerSystem Instance;

		void Awake()
		{
			Instance = this;
		}

		void Start()
		{
            MLInput.Start();

            //ml input controller input handlers
            MLInput.OnControllerButtonDown += HandleOnControllerButtonDown;
            MLInput.OnControllerButtonUp += HandleOnControllerButtonUp;
            MLInput.OnTriggerDown += HandleOnTriggerDown;
            MLInput.OnTriggerUp += HandleOnTriggerUp;

            // assign some defaults if needed
            if (pointerAccuracy == 0)
			{
				pointerAccuracy = 0.995f;
			}
			if (pushRate == 0)
			{
				pushRate = 1;
			}
			if (rotateRate == 0)
			{
				rotateRate = 180;
			}
			if (scaleRate == 0)
			{
				scaleRate = 1;
			}

			pointerLineRenderer = GetComponent<LineRenderer>();

			UpdateGrabObjectArray();
		}

		void OnDestroy()
		{
			if (MagicLeapDevice.IsReady())
			{
				MLInput.OnControllerButtonDown -= HandleOnControllerButtonDown;
				MLInput.OnControllerButtonUp -= HandleOnControllerButtonUp;
				MLInput.OnTriggerDown -= HandleOnTriggerDown;
				MLInput.OnTriggerUp -= HandleOnTriggerUp;
			}
		}

		void HandleOnControllerButtonDown(byte idx, MLInputControllerButton button)
		{
			if (button == MLInputControllerButton.HomeTap)
			{
				Application.Quit();
			}
			if (button == MLInputControllerButton.Bumper)
			{
				bumperIsDown = true;
			}
		}

		void HandleOnControllerButtonUp(byte idx, MLInputControllerButton button)
		{
			if (button ==MLInputControllerButton.HomeTap)
			{
				Application.Quit();
			}
			if (button == MLInputControllerButton.Bumper)
			{
				bumperIsDown = false;
			}
		}

		void HandleOnTriggerDown(byte idx, float v)
		{
			triggerIsDown = true;
			if (OnTriggerDown != null)
			{
				OnTriggerDown();
			}
		}

		void HandleOnTriggerUp(byte idx, float v)
		{
			triggerIsDown = false;
			if (OnTriggerUp != null)
			{
				OnTriggerUp();
			}
		}

		void Update()
		{

			HandleMLControllers();

			Vector3 sourcePos = transform.position;
			Vector3 targetPos = sourcePos + transform.forward;

			bool triggerClicked = (lastTriggerWasUp && triggerIsDown) ? true : false;
			if (triggerClicked)
			{
				if (OnTriggerClicked != null)
				{
					OnTriggerClicked();
				}
			}

			bool bumperClicked = (lastBumperWasUp && bumperIsDown) ? true : false;
			if (bumperClicked)
			{
				if (OnBumperClicked != null)
				{
					OnBumperClicked();
				}
			}

			if (grabbedObject != null)
			{
				if (trackPadVer != 0)
				{
                    // push grabbed object in/out
                    grabbedDist += trackPadVer * pushRate;
                    Debug.Log("grabbed dist " + grabbedDist);
				}
				if (trackPadHor != 0)
				{
					if (grabbedObject.canScale)
					{
						// scale object big/small
						float scaleSize = grabbedObject.transform.localScale.x;
						if ((trackPadHor < 0 && scaleSize > grabbedObject.minScaleSize) || (trackPadHor > 0 && scaleSize < grabbedObject.maxScaleSize))
						{
							grabbedObject.transform.localScale += Vector3.one * trackPadHor * scaleRate * Time.deltaTime;
						}
					}
					else
					{ 
						// rotate object around Y axis
						grabbedObject.rotationOffset += trackPadHor * rotateRate * Time.deltaTime;
					}
				}

				// we have an object attached
				targetPos = sourcePos + (transform.forward * grabbedDist);

				grabbedObject.transform.position = targetPos;
				grabbedObject.transform.rotation = transform.rotation * Quaternion.Euler(Vector3.up * grabbedObject.rotationOffset);

				if (triggerClicked)
				{
					// disconnect from the pointer
					if (OnDropObject != null)
					{
						OnDropObject(grabbedObject);
					}

					UpdateGrabObjectArray();

					grabbedObject = null;
				}
				else if (bumperClicked && !grabbedObject.noDelete)
				{
					DestroyGrabObject(grabbedObject);
					grabbedObject = null;
				}
			}
			else
			{
				// we are looking for an object
				GrabObject obj = FindNearestGrabObject();
				if (obj != null)
				{
					// an object has been located so save the distance
					float dist = Vector3.Distance(sourcePos, obj.transform.position);

					// snap to object of interest
					targetPos = obj.transform.position;

					if (triggerClicked)
					{
						if (obj.inLibrary)
						{
							// create a copy of the object
							GameObject g = Instantiate(obj.gameObject, contentRoot);
							grabbedObject = g.GetComponent<GrabObject>();
							grabbedObject.transform.position = obj.transform.position;
							grabbedObject.transform.rotation = obj.transform.rotation;
							grabbedObject.inLibrary = false;
						}
						else
						{
							grabbedObject = obj;
						}

						grabbedDist = dist;

						if (OnGrabObject != null)
						{
							OnGrabObject(grabbedObject);
						}
					}
					else if (bumperClicked && !obj.inLibrary && !obj.noDelete)
					{
						DestroyGrabObject(obj);
					}
				}
			}

            Vector3 temp = targetPos;
            temp.z += trackPadVer * pushRate;
            

			// draw the line pointer
			pointerLineRenderer.SetPosition(0, transform.position);
			pointerLineRenderer.SetPosition(1, temp);

			// flags to track one-time clicks
			lastTriggerWasUp = (triggerIsDown ? false : true);
			lastBumperWasUp = (bumperIsDown ? false : true);
		}

		GrabObject FindNearestGrabObject()
		{
			GrabObject nearestObj = null; 
			float nearestDist = float.MaxValue;
			float nearestDot = pointerAccuracy;

			for (int i = 0; i < grabObjectArray.Length; i++)
			{
				Vector3 dir = grabObjectArray[i].transform.position - transform.position;
				float dot = Vector3.Dot(transform.forward, dir.normalized);
				if (dot > nearestDot)
				{
					float dist = dir.sqrMagnitude;
					if (dist < nearestDist)
					{
						nearestObj = grabObjectArray[i];
						nearestDist = dist;
						nearestDot = dot;
					}
				}
			}

			return nearestObj;
		}

		public void DestroyGrabObject(GrabObject obj)
		{
			if (OnDeleteObject != null)
			{
				OnDeleteObject(obj);
			}

			DestroyImmediate(obj.gameObject);
			UpdateGrabObjectArray();
		}

		public void UpdateGrabObjectArray()
		{
			// update the grab object array - anytime an object is added to the scene (contentRoot) call this function to update the array
			grabObjectArray = contentRoot.GetComponentsInChildren<GrabObject>();
		}

		void HandleMLControllers()
		{
			trackPadHor = 0;
			trackPadVer = 0;

			if (handler != null)
			{
				if (handler.IsControllerValid())
				{
					MLInputController controller = handler.ConnectedController;
					if (controller != null)
					{
                        debug.text = "Connected controller stats: " + controller.Touch1PosAndForce.ToString(); 
						if (controller.Touch1Active && controller.Touch1PosAndForce.z > 0.2f)
						{
							if (controller.Touch1PosAndForce.x > 0.2f || controller.Touch1PosAndForce.x < -0.2f)
							{
								trackPadHor = controller.Touch1PosAndForce.x;
							}

							if (controller.Touch1PosAndForce.y > 0.2f || controller.Touch1PosAndForce.y < -0.2f)
							{
								trackPadVer = controller.Touch1PosAndForce.y;
							}
						}
					}
				}
			}
		}
	}
}
