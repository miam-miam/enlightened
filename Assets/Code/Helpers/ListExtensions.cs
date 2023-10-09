using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{

	public static void ExpandingAdd<T>(this List<T> list, int point, T element)
		where T : class
	{
		if (point >= list.Count)
		{
			for (int i = list.Count; i < point; i++)
				list.Add(null);
			list.Add(element);
		}
		else
		{
			list[point] = element;
		}
	}

}
