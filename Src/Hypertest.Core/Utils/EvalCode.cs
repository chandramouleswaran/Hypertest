/*
    Hypertest - A web testing framework using Selenium
    Copyright (C) 2012  Chandramouleswaran Ravichandran

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Text;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Text.RegularExpressions;

namespace Hypertest.Core.Utils
{
    /// <summary>
    /// Code evaluator
    /// </summary>
    public class EvalCode
    {
        public static object Evaluate(string code, bool replace = true)
        {
            if (replace)
            {
                code = ReplaceVariables(code);
            }

            if (!string.IsNullOrEmpty(code))
            {
                object val;
                try
                {
                    //Most probably a complete string
                    val = CompileCode("\"" + code + "\"");
                }
                catch(Exception ex)
                {
                    val = CompileCode(code);
                }
                return val;
            }
            throw new Exception("The expression needing evaluation is empty.");
        }

        private static string ReplaceVariables(string code)
        {
            Regex r = new Regex("%.*?%");
            MatchCollection matches = r.Matches(code);
            foreach (Match m in matches)
            {
                Variable v = Runner.Instance.GetVariable(m.Value.Replace("%",""));
                if (v != null)
                {
                    switch (v.Type)
                    {
                        case DataType.Number:
                            code = code.Replace(m.Value, (v.Value as double?).ToString());
                            break;
                        case DataType.String:
                            code = code.Replace(m.Value, "\"" + v.Value + "\"");
                            break;
                        default:
                            code = code.Replace(m.Value, v.Value.ToString());
                            break;
                    }
                }
                else
                {
                    throw new Exception("Unable to find variable " + m.Value.Replace("%", "") + " set during this session.");
                }
            }
            if (matches.Count == 0)
            {
                //Add double quotes if there are no variables - code evaluator needs double quotes
                if (code.Contains("+") || code.Contains("-") || code.Contains("*") || code.Contains("/") || code.Contains("."))
                {
                    return code;
                }
                code = "\"" + code + "\"";
            }
            return code;
        }

        private static object CompileCode(string code)
        {
            CSharpCodeProvider c = new CSharpCodeProvider();
            CompilerParameters cp = new CompilerParameters();

            cp.ReferencedAssemblies.Add("system.dll");
            cp.ReferencedAssemblies.Add("system.xml.dll");
            cp.ReferencedAssemblies.Add("system.data.dll");
            cp.ReferencedAssemblies.Add("system.drawing.dll");
            cp.ReferencedAssemblies.Add("system.windows.forms.dll");

            cp.CompilerOptions = "/t:library";
            cp.GenerateInMemory = true;

            StringBuilder sb = new StringBuilder("");
            sb.Append("using System;\n");
            sb.Append("using System.IO;\n");
            sb.Append("using System.Xml;\n");
            sb.Append("using System.Data;\n");
            sb.Append("using System.Windows.Forms;\n");
            sb.Append("using System.Windows;\n");
            sb.Append("namespace Evaler{ \n");
            sb.Append("public class CodeEvaler{ \n");
            sb.Append("public object EvalCode(){\n");
            sb.Append("return ");
            sb.Append(code);
            sb.Append(";}\n");
            sb.Append("}\n");
            sb.Append("}\n");
            CompilerResults cr = c.CompileAssemblyFromSource(cp, sb.ToString());
            if (cr.Errors.Count > 0)
            {
                throw new ArgumentException("The expression '" + code + "' does not compile to C#, or does not return bool");
            }

            Assembly a = cr.CompiledAssembly;
            object o = a.CreateInstance("Evaler.CodeEvaler");

            if (o != null)
            {
                Type t = o.GetType();
                MethodInfo mi = t.GetMethod("EvalCode");

                return mi.Invoke(o, null);
            }
            return null;
        }
    }
}
