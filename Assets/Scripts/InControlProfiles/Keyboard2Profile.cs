using System;
using System.Collections;
using UnityEngine;
using InControl;


public class Keyboard2Profile : UnityInputDeviceProfile
{
	public Keyboard2Profile()
	{
		Name = "Keyboard2";
		Meta = "A keyboard profile to another player.";

		// This profile only works on desktops.
		SupportedPlatforms = new[]
		{
			"Windows",
			"Mac",
			"Linux"
		};

		Sensitivity = 1.0f;
		LowerDeadZone = 0.0f;
		UpperDeadZone = 1.0f;

		ButtonMappings = new[]
		{
			new InputControlMapping
			{
				Handle = "Jump",
				Target = InputControlType.Action1,
				Source = KeyCodeButton( KeyCode.Keypad3)
			},
			new InputControlMapping
			{
				Handle = "Fire",
				Target = InputControlType.Action2,
				Source = KeyCodeButton( KeyCode.Keypad2)
			},
			new InputControlMapping
			{
				Handle = "Start",
				Target = InputControlType.Start,
				Source = KeyCodeButton( KeyCode.Return )
			},
			new InputControlMapping
			{
				Handle = "DPadLeft",
				Target = InputControlType.DPadLeft,
				Source = KeyCodeButton( KeyCode.LeftArrow )
			},
			new InputControlMapping
			{
				Handle = "DPadRight",
				Target = InputControlType.DPadRight,
				Source = KeyCodeButton( KeyCode.RightArrow )
			},
			new InputControlMapping
			{
				Handle = "DPadUp",
				Target = InputControlType.DPadUp,
				Source = KeyCodeButton( KeyCode.UpArrow )
			},
			new InputControlMapping
			{
				Handle = "DPadDown",
				Target = InputControlType.DPadDown,
				Source = KeyCodeButton( KeyCode.DownArrow )
			}
		};

		AnalogMappings = new[]
		{
			new InputControlMapping {
				Handle = "Move X",
				Target = InputControlType.LeftStickX,
				Source = KeyCodeAxis( KeyCode.LeftArrow, KeyCode.RightArrow )
			},
			new InputControlMapping {
				Handle = "Move Y",
				Target = InputControlType.LeftStickY,
				Source = KeyCodeAxis( KeyCode.DownArrow, KeyCode.UpArrow )
			}
		};
	}
}