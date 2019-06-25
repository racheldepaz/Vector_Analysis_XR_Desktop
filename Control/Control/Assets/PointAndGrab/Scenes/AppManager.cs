using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LesBird;

/// <summary>
/// AppManager.cs
/// 
/// Demo app for the PointerSystem. We register for callbacks and log them to the console.
/// 
/// 2018 Les Bird
/// </summary>
public class AppManager : MonoBehaviour
{
	private bool triggerIsDown;

	void Start()
	{
		PointerSystem.Instance.OnGrabObject += HandleOnGrabObject;
		PointerSystem.Instance.OnDropObject += HandleOnDropObject;
		PointerSystem.Instance.OnDeleteObject += HandleOnDeleteObject;
		PointerSystem.Instance.OnTriggerClicked += HandleOnTriggerClicked;
		PointerSystem.Instance.OnTriggerDown += HandleOnTriggerDown;
		PointerSystem.Instance.OnTriggerUp += HandleOnTriggerUp;
		PointerSystem.Instance.OnBumperClicked += HandleOnBumperClicked;
	}

	void OnDestroy()
	{
		PointerSystem.Instance.OnGrabObject -= HandleOnGrabObject;
		PointerSystem.Instance.OnDropObject -= HandleOnDropObject;
		PointerSystem.Instance.OnDeleteObject -= HandleOnDeleteObject;
		PointerSystem.Instance.OnTriggerClicked -= HandleOnTriggerClicked;
		PointerSystem.Instance.OnTriggerDown -= HandleOnTriggerDown;
		PointerSystem.Instance.OnTriggerUp -= HandleOnTriggerUp;
		PointerSystem.Instance.OnBumperClicked -= HandleOnBumperClicked;
	}

	void HandleOnGrabObject(GrabObject obj)
	{
		Debug.Log(obj.name + " picked up");
	}

	void HandleOnDropObject(GrabObject obj)
	{
		Debug.Log(obj.name + " dropped");
	}

	void HandleOnDeleteObject(GrabObject obj)
	{
		Debug.Log(obj.name + " deleted");
	}

	void HandleOnTriggerClicked()
	{
		Debug.Log("Trigger clicked");
	}

	void HandleOnTriggerDown()
	{
		if (!triggerIsDown)
		{
			Debug.Log("Trigger down");
			triggerIsDown = true;
		}
	}

	void HandleOnTriggerUp()
	{
		Debug.Log("Trigger up");
		triggerIsDown = false;
	}

	void HandleOnBumperClicked()
	{
		Debug.Log("Bumper clicked");
	}
}
