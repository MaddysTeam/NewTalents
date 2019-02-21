using System.Collections.Generic;

namespace Business
{

	public partial class Picklist
	{

		private readonly List<PicklistItem> _items = new List<PicklistItem>();

		public List<PicklistItem> Items
			=> _items;

	}

}