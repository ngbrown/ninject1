#region License
//
// Author: Nate Kohari <nkohari@gmail.com>
// Copyright (c) 2007-2008, Enkari, Ltd.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion
#region Using Directives
using System;
using System.Reflection;
#endregion

namespace Ninject.Core.Injection
{
	/// <summary>
	/// An injector that can read from and write to fields.
	/// </summary>
	public interface IFieldInjector : IInjector<FieldInfo>
	{
		/*----------------------------------------------------------------------------------------*/
		/// <summary>
		/// Gets the value of the field associated with the injector.
		/// </summary>
		/// <param name="target">The instance on which the field should be read.</param>
		/// <returns>The value stored in the field.</returns>
		object Get(object target);
		/*----------------------------------------------------------------------------------------*/
		/// <summary>
		/// Sets the value of the field associated with the injector.
		/// </summary>
		/// <param name="target">The instance on which the field should be written.</param>
		/// <param name="value">The value to store in the field.</param>
		void Set(object target, object value);
		/*----------------------------------------------------------------------------------------*/
	}
}