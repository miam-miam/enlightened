using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEvent
{

	public event Action OnRaised;

	public void Raise()
	{
		foreach (var delegateFunction in OnRaised.GetInvocationList())
		{
			try
			{
				delegateFunction.DynamicInvoke();
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}
		}
	}

}

public class GlobalEvent<T>
{

    public event Action<T> OnRaised;

    public void Raise(T value)
    {
		foreach (var delegateFunction in OnRaised.GetInvocationList())
		{
			try
			{
				delegateFunction.DynamicInvoke(value);
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}
		}
    }
    
}

public class GlobalEvent<T1, T2>
{

	public event Action<T1, T2> OnRaised;

	public void Raise(T1 v1, T2 v2)
	{
		foreach (var delegateFunction in OnRaised.GetInvocationList())
		{
			try
			{
				delegateFunction.DynamicInvoke(v1, v2);
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}
		}
	}

}

public class GlobalEvent<T1, T2, T3>
{

	public event Action<T1, T2, T3> OnRaised;

	public void Raise(T1 v1, T2 v2, T3 v3)
	{
		foreach (var delegateFunction in OnRaised.GetInvocationList())
		{
			try
			{
				delegateFunction.DynamicInvoke(v1, v2, v3);
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}
		}
	}

}

