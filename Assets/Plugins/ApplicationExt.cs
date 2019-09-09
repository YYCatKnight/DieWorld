using UnityEngine;
using System.Collections;

namespace GTech
{
	public class ApplicationExt : MonoBehaviour 
	{
		public static string scriptDataPath
		{
			get
			{
				//return Application.temporaryCachePath;
				return Application.persistentDataPath;
			}
		}

		public static string user
		{
			get
			{
				return "example20166";
			}
		}

		public static string license
		{
			get
			{
				return "72893225174b10750151179ec802d0d6";
			}
		}

		public static uint GetVersion()
		{
			return 0;
		}

		public static uint GetEncrypt()
		{
			return 0;
		}
	}
}
