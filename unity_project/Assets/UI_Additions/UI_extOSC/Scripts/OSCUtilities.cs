/* Copyright (c) 2020 ExT (V.Sigalkin) */

using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;


#if UNITY_WSA && !UNITY_EDITOR
using System.Reflection;
#endif

namespace extOSC
{
	public static class OSCUtilities
	{
		#region Static Private Vars

		private static readonly Dictionary<string, List<string>> _cachedAddress = new Dictionary<string, List<string>>();

		#endregion

		#region Static Public Methods

		public static int ClampPort(int port)
		{
			return Mathf.Clamp(port, 1, ushort.MaxValue);
		}

		public static string GetLocalHost()
		{
#if !UNITY_WSA
			try
			{
				var hostName = Dns.GetHostName();
				var host = Dns.GetHostEntry(hostName);

				foreach (var address in host.AddressList)
				{
					if (address.AddressFamily == AddressFamily.InterNetwork)
					{
						return address.ToString();
					}
				}
			}
			catch
			{ }
#endif

			return "127.0.0.1";
		}

		public static float Map(float value, float inputMin, float inputMax, float outputMin, float outputMax, bool clamp = true)
		{
			if (Mathf.Abs(inputMin - inputMax) < Mathf.Epsilon) return outputMin;

			var outputValue = ((value - inputMin) / (inputMax - inputMin) * (outputMax - outputMin) + outputMin);

			if (!clamp) return outputValue;
			return outputMax < outputMin ? Mathf.Clamp(outputValue, outputMax, outputMin) : Mathf.Clamp(outputValue, outputMin, outputMax);
		}

		public static bool CompareAddresses(string bindAddress, string messageAddress)
		{
			if (bindAddress == "*")
				return true;

			if (!bindAddress.Contains("*"))
				return bindAddress == messageAddress;

			if (!_cachedAddress.ContainsKey(bindAddress))
				_cachedAddress.Add(bindAddress, new List<string>());

			if (_cachedAddress[bindAddress].Contains(messageAddress))
				return true;

			var regular = new Regex("^" + bindAddress.Replace("*", "(.+)") + "$");
			if (regular.IsMatch(messageAddress))
			{
				_cachedAddress[bindAddress].Add(messageAddress);
				return true;
			}

			return false;
		}

		public static byte[] StructToByte<T>(T structure) where T : struct
		{
			var structureSize = StructSizeOf(structure);
			var data = new byte[structureSize];
			var pointer = Marshal.AllocHGlobal(structureSize);

			Marshal.StructureToPtr(structure, pointer, true);
			Marshal.Copy(pointer, data, 0, structureSize);
			Marshal.FreeHGlobal(pointer);

			return data;
		}

		public static T ByteToStruct<T>(byte[] data) where T : struct
		{
			var structureSize = StructSizeOf<T>();
			var pointer = Marshal.AllocHGlobal(structureSize);

			Marshal.Copy(data, 0, pointer, structureSize);
			var structure = PtrToStructure<T>(pointer);
			Marshal.FreeHGlobal(pointer);

			return structure;
		}


		#endregion

		#region Public Extensions Static Methods








		public static bool IsSubclassOf(Type targetType, Type sourceType)
		{
#if !UNITY_WSA || UNITY_EDITOR
			return (targetType.IsSubclassOf(sourceType) || targetType == sourceType);
#else
            return (targetType.GetTypeInfo().IsSubclassOf(sourceType) || targetType == sourceType);
#endif
		}

		public static bool IsSubclassOf(object target, Type targetType)
		{
			return IsSubclassOf(target.GetType(), targetType);
		}

		public static int IndexOf<T>(this T[] array, T target) where T : class
		{
			for (var i = 0; i < array.Length; ++i)
			{
				if (array[i] == null && target == null)
					return i;

				if (array[i] != null && array[i].Equals(target))
					return i;
			}

			return -1;
		}

		#endregion

		#region Private Static Methods

		private static int StructSizeOf<T>(T structure) where T : struct
		{
#if !UNITY_WSA
			return Marshal.SizeOf(structure);
#else
            return Marshal.SizeOf<T>(structure);
#endif
		}

		private static int StructSizeOf<T>() where T : struct
		{
#if !UNITY_WSA
			return Marshal.SizeOf(typeof(T));
#else
            return Marshal.SizeOf<T>();
#endif
		}

		private static T PtrToStructure<T>(IntPtr pointer) where T : struct
		{
#if !UNITY_WSA
			return (T) Marshal.PtrToStructure(pointer, typeof(T));
#else
            return Marshal.PtrToStructure<T>(pointer);
#endif
		}

		#endregion
	}
}