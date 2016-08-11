using AuthorizeNetTest.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AuthorizeNetTest.Models.ViewModels
{
    public class ViewModelBase
    {
        public bool IsLive { get { return !ConfigHelper.UseApiSandbox; } }
    }
}