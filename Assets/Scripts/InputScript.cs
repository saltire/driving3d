using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputScript : MonoBehaviour {
  float steer;
  float acc;
  float brake;

  public void OnSteer1D(InputAction.CallbackContext context) {
    steer = context.ReadValue<float>();
  }

  public void OnSteer2D(InputAction.CallbackContext context) {
    // TODO: calculate steering direction from the left stick direction
    Util.Log("Steer2D", context.ReadValue<Vector2>());
  }

  public void OnAccelerate(InputAction.CallbackContext context) {
    acc = context.ReadValue<float>();
  }

  public void OnHandbrake(InputAction.CallbackContext context) {
    brake = context.ReadValue<float>();
  }

  public DrivingActions GetDrivingActions() {
    return new DrivingActions {
      steer = steer,
      acc = acc,
      brake = brake,
    };
  }
}
