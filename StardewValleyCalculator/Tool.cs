using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace StardewValleyCalculator
{
    public static class JsonDotNetTool
    {
        /// <summary>轉成JArray</summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static JArray ToJArray<T>(this IEnumerable<T> source)
        {
            JArray result = new JArray();
            foreach (T item in source)
            {
                result.Add(item);
            }
            return result;
        }

        /// <summary>首字大寫</summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FirstLetterToUpper(this string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }

        public static void DoubleBuffered(this DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                  BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }
    }
}
