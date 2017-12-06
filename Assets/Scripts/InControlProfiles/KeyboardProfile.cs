using System;
using System.Collections;
using UnityEngine;
using InControl;


public class KeyboardProfile : UnityInputDeviceProfile
{
	public KeyboardProfile()
	{
		Name = "Keyboard";
		Meta = "A keyboard profile.";

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
				Source = KeyCodeButton(KeyCode.N)
			},
			new InputControlMapping
			{
				Handle = "Fire",
				Target = InputControlType.Action2,
				Source = KeyCodeButton( KeyCode.B)
			},
			new InputControlMapping
			{
				Handle = "Start",
				Target = InputControlType.Start,
				Source = KeyCodeButton( KeyCode.F )
			},
			new InputControlMapping
			{
				Handle = "DPadLeft",
				Target = InputControlType.DPadLeft,
				Source = KeyCodeButton( KeyCode.A )
			},
			new InputControlMapping
			{
				Handle = "DPadRight",
				Target = InputControlType.DPadRight,
				Source = KeyCodeButton( KeyCode.D )
			},
			new InputControlMapping
			{
				Handle = "DPadUp",
				Target = InputControlType.DPadUp,
				Source = KeyCodeButton( KeyCode.W )
			},
			new InputControlMapping
			{
				Handle = "DPadDown",
				Target = InputControlType.DPadDown,
				Source = KeyCodeButton( KeyCode.S )
			}
		};

		AnalogMappings = new[]
		{
			new InputControlMapping
			{
				Handle = "Move X",
				Target = InputControlType.LeftStickX,
				// KeyCodeAxis splits the two KeyCodes over an axis. The first is negative, the second positive.
				Source = KeyCodeAxis( KeyCode.A, KeyCode.D )
			},
			new InputControlMapping
			{
				Handle = "Move Y",
				Target = InputControlType.LeftStickY,
				// Notes that up is positive in Unity, therefore the order of KeyCodes is down, up.
				Source = KeyCodeAxis( KeyCode.S, KeyCode.W )
			}
		};
	}
}