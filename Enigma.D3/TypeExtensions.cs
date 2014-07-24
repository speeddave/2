﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Enigma.D3.UI.Controls;

namespace Enigma
{
	public static class TypeExtensions
	{
		private static Dictionary<Type, int> _cachedSizes = new Dictionary<Type, int>();

		private static HashSet<Type> _cachedMemoryObjectTypes = new HashSet<Type>();
		private static HashSet<Type> _cachedNonMemoryObjectTypes = new HashSet<Type>();

		public static bool IsMemoryObjectType(this Type type)
		{
			if (_cachedMemoryObjectTypes.Contains(type))
				return true;
			if (_cachedNonMemoryObjectTypes.Contains(type))
				return false;

			if (type.IsSubclassOf(typeof(MemoryObject)) || type.Equals(typeof(MemoryObject)))
			{
				_cachedMemoryObjectTypes.Add(type);
				return true;
			}
			else
			{
				_cachedNonMemoryObjectTypes.Add(type);
				return false;
			}
		}

		public static bool IsUXControlType(this Type type)
		{
			return type.IsSubclassOf(typeof(UXControl)) || type.Equals(typeof(UXControl));
		}

		public static int SizeOf(this Type type)
		{
			int sizeOf;
			if (!_cachedSizes.TryGetValue(type, out sizeOf))
			{
				if (type.Equals(typeof(MemoryObject)))
				{
					return 0;
				}
				else if (type.IsSubclassOf(typeof(MemoryObject)))
				{
					var field = type.GetField("SizeOf", BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public);
					if (field == null)
						throw new InvalidOperationException("Could not find 'SizeOf' constant on type '" + type.Name + "'");
					sizeOf = field.IsLiteral ? (int)field.GetRawConstantValue() : (int)field.GetValue(null);
				}
				else
				{
					sizeOf = Marshal.SizeOf(type);
				}
				_cachedSizes.Add(type, sizeOf);
			}
			return sizeOf;
		}
	}
}
