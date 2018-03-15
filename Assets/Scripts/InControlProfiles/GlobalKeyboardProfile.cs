using System;
using System.Collections;
using UnityEngine;
using InControl;


public class GlobalKeyboardProfile : UnityInputDeviceProfile
{
	public GlobalKeyboardProfile()
	{
		Name = "GlobalKeyboard";
		Meta = "The global keyboard profile.";

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
				Handle = "Fire",
				Target = InputControlType.Action2,
				Source = KeyCodeButton(KeyCode.Escape)
			},
			new InputControlMapping
			{
				Handle = "Start",
				Target = InputControlType.Action1,
				Source = KeyCodeButton(KeyCode.Return)
			},
			new InputControlMapping
			{
				Handle = "Start",
				Target = InputControlType.Action1,
				Source = KeyCodeButton(KeyCode.Space)
			}
		};
	}
}