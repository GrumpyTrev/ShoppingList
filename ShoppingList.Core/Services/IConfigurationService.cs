using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingList.Core.Services
{
    interface IConfigurationService
    {
		string DatabasePath
		{
			set;
		}
    }
}
