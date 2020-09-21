using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputScript : MonoBehaviour {
  float steer1d;
  Vector2 steer2d;
  float acc;
  float brake;

  public void OnSteer1D(InputAction.CallbackContext context) {
    steer1d = context.ReadValue<float>();
  }

  public void OnSteer2D(InputAction.CallbackContext context) {
    steer2d = context.ReadValue<Vector2>();
  }

  public void OnAccelerate(InputAction.CallbackContext context) {
    acc = context.ReadValue<float>();
  }

  public void OnHandbrake(InputAction.CallbackContext context) {
    brake = context.ReadValue<float>();
  }

  public DrivingActions GetDrivingActions() {
    return new DrivingActions {
      steer1d = steer1d,
      steer2d = steer2d,
      acc = acc,
      brake = brake,
    };
  }
}
