using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using UnityEngine;

namespace XFramework
{
	public abstract class XMonoSingleton<T> : MonoBehaviour where T : XMonoSingleton<T>
	{
		protected static T instance = null;

		protected XMonoSingleton()
		{

		}

		public static T Instance()
		{
			if (null == instance)
			{
				instance = FindObjectOfType<T>();
				if (FindObjectOfType<T>().Length > 1)
				{
					
				}
				// 获取所有的非public构造函数
				ConstructorInfo[] ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
				// 从ctors里面获取无参的构造函数
				ConstructorInfo ctor = System.Array.Find(ctors, c => 0 == c.GetParameters().Length);
				if (null == ctor)
				{
					throw new System.Exception("Non-public ctor() not found!");
				}
				// 调用构造函数
				instance = ctor.Invoke(null) as T;
			}
			return instance;
		}
	}
}

