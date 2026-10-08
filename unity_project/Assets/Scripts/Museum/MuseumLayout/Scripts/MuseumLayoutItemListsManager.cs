using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Museum
{
	public class MuseumLayoutItemListsManager : MonoBehaviour 
	{
		[SerializeField]
		private List<MuseumFloorData> _itemLists = new List<MuseumFloorData>();
		public List<MuseumFloorData> itemLists
		{
			get
			{
				return _itemLists;
			}
			private set
			{
				_itemLists = value;
			}
		}

		public static MuseumLayoutItemListsManager instance;

		void Awake()
		{
			if(instance != null)
			{
				Destroy(gameObject);
				return;
			}

			instance = this;
			DontDestroyOnLoad(this);
		}

		public MuseumFloorData GetList(string _name)
		{
			MuseumFloorData _returnIL = null;

			foreach (MuseumFloorData il in itemLists)
			{
				if(il != null)
				{
					if(il.m_Name == _name)
					{
						_returnIL = il;
					}	
				}		
			}

			return _returnIL;
		}
	}
}