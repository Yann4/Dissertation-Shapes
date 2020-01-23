namespace Dissertation.Narrative
{
	public static class ActionFunctionLibrary
	{
		public enum Actions
		{
			NONE,
		}

		public static bool PerformAction(Actions action)
		{
			switch(action)
			{
				case Actions.NONE:
					return true;
				default:
					return false;
			}
		}
	}
}