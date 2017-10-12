using UnityEngine.EventSystems;
using UnityEngine;

class OnlyKeyBoardInputModule : StandaloneInputModule
{
	public override void Process()
	{
		bool usedEvent = SendUpdateEventToSelectedObject();

		if (eventSystem.sendNavigationEvents)
		{
			if (!usedEvent)
				usedEvent |= SendMoveEventToSelectedObject();

			if (!usedEvent)
				SendSubmitEventToSelectedObject();
		}
	}
}