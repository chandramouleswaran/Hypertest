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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Hypertest.Core.Interfaces
{
	/// <summary>
	/// The post run pairs class
	/// </summary>
	[DataContract]
	[Serializable]
	public class PostRunPairs : INotifyPropertyChanged
	{
		#region Members
		private string _variableName;
		private string _propertyName; 
		#endregion

		#region CTOR
		public PostRunPairs()
		{
			this.VariableName = "";
			this.PropertyName = "";
		} 
		#endregion

		#region Properties
        [DataMember]
		public string VariableName
		{
			get { return _variableName; }
			set { _variableName = value; RaisePropertyChanged(); }
		}

        [DataMember]
        public string PropertyName
		{
			get { return _propertyName; }
			set { _propertyName = value; }
		} 
		#endregion

		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		} 
		#endregion
	}
}