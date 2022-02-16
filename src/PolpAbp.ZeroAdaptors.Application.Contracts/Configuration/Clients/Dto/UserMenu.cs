using System;
using System.Collections.Generic;

namespace PolpAbp.ZeroAdaptors.Configuration.Clients.Dto
{

	/// <summary>
	/// Represents a menu shown to the user.
	/// </summary>
	public class UserMenu
	{
		/// <summary>
		/// Unique name of the menu in the application. 
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Display name of the menu.
		/// </summary>
		public string DisplayName
		{
			get;
			set;
		}

		/// <summary>
		/// A custom object related to this menu item.
		/// </summary>
		public object CustomData
		{
			get;
			set;
		}

		/// <summary>
		/// Menu items (first level).
		/// </summary>
		public IList<UserMenuItem> Items
		{
			get;
			set;
		}

		/// <summary>
		/// Creates a new <see cref="T:Abp.Application.Navigation.UserMenu" /> object.
		/// </summary>
		public UserMenu() {
			Items = new List<UserMenuItem>();
		}

		/// <summary>
		/// Creates a new <see cref="T:Abp.Application.Navigation.UserMenu" /> object from given <see cref="T:Abp.Application.Navigation.MenuDefinition" />.
		/// </summary>
		// internal UserMenu(MenuDefinition menuDefinition, ILocalizationContext localizationContext) {
		// }
	}
}
