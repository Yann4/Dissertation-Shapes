using System;

namespace Dissertation.Util
{
	public static class Extensions
	{
		public static void InvokeSafe(this Action action)
		{
			if(action != null)
			{
				action.Invoke();
			}
		}

		public static void InvokeSafe<T>(this Action<T> action, T val)
		{
			if(action != null)
			{
				action.Invoke(val);
			}
		}
	}
}