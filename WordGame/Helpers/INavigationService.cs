using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordGame.Helpers
{
    public interface INavigationService
    {
        void NavigateTo(string viewName, object parameter = null);
        void GoBack();
    }
}
