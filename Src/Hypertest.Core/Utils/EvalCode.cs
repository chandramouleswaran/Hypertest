#region License

// Copyright (c) 2014 Chandramouleswaran Ravichandran
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.Text;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Text.RegularExpressions;
using Hypertest.Core.Interfaces;
using Hypertest.Core.Runners;

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
				catch (Exception ex)
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
				Variable v = WebScenarioRunner.Current.GetVariable(m.Value.Replace("%", ""));
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