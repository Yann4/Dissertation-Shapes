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

		public static void InvokeSafe<T1, T2>(this Action<T1, T2> action, T1 val1, T2 val2)
		{
			if (action != null)
			{
				action.Invoke(val1, val2);
			}
		}
	}
}