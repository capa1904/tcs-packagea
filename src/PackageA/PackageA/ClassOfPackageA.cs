using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PackageA
{
    public class ClassOfPackageA
    {
        public string GetName()
        {
            return "PackageA";
        }

        public string GetVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string productVersion = fileVersionInfo.ProductVersion;
			//return productVersion
            return productVersion;
        }

        public string GetSummary()
        {            
            return $"{GetName()} v. {GetVersion()}";
        }
    }
}
