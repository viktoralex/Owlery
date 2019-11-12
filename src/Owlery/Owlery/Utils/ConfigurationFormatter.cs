using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

namespace Owlery.Utils
{
    public class ConfigurationFormatter
    {
        public static string FormatWithConfig(string format, IConfiguration source)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            Regex r = new Regex(@"(?<start>\{)+(?<property>.+)(?<end>\})+",
                RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

            List<object> values = new List<object>();
            string rewrittenFormat = r.Replace(format, delegate (Match m)
            {
                Group startGroup = m.Groups["start"];
                Group propertyGroup = m.Groups["property"];
                Group endGroup = m.Groups["end"];

                values.Add(source.GetValue<string>(propertyGroup.Value));

                return new string('{', startGroup.Captures.Count) + (values.Count - 1) + new string('}', endGroup.Captures.Count);
            });

            return string.Format(rewrittenFormat, values.ToArray());
        }
    }
}