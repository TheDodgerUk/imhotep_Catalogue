using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Museum
{
	public class MuseumLayoutItemListLoader : MonoBehaviour 
	{
		public string itemListName;
		public MuseumLayoutItemListLoadEvent onLoadEvent;

		void Start()
		{
			if (onLoadEvent == null)
            	onLoadEvent = new MuseumLayoutItemListLoadEvent();

        	onLoadEvent.AddListener(OnLoadEvent);

			if(MuseumLayoutItemListsManager.instance != null)
				onLoadEvent.Invoke(MuseumLayoutItemListsManager.instance.GetList(itemListName));
		}

		void OnLoadEvent(MuseumFloorData _itemList)
		{	

		}

	}
}