using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigCommon
{
    public static class AppCommon
    {
        public static bool IsDate(string Value)
        {
            DateTime dt;
            bool boolRet;

            try
            {
                dt = Convert.ToDateTime(Value);
                boolRet = true;
            }
            catch
            {
                boolRet = false;
            }

            return boolRet;
        }
    }
}
