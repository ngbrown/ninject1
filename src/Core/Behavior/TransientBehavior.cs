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
using Ninject.Core.Activation;
using Ninject.Core.Infrastructure;
#endregion

namespace Ninject.Core.Behavior
{
	/// <summary>
	/// A behavior that causes a new instance of the type to be created each time one is requested.
	/// </summary>
	public class TransientBehavior : BehaviorBase
	{
		/*----------------------------------------------------------------------------------------*/
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="TransientBehavior"/> class.
		/// </summary>
		public TransientBehavior()
		{
			SupportsEagerActivation = false;
			ShouldTrackInstances = false;
		}
		#endregion
		/*----------------------------------------------------------------------------------------*/
		#region Public Methods
		/// <summary>
		/// Resolves an instance of the type based on the rules of the behavior.
		/// </summary>
		/// <param name="context">The context in which the instance is being activated.</param>
		/// <returns>An instance of the type associated with the behavior.</returns>
		public override object Resolve(IContext context)
		{
			Ensure.NotDisposed(this);
			context.Binding.Components.Activator.Activate(context);

			return context.Instance;
		}
		/*----------------------------------------------------------------------------------------*/
		/// <summary>
		/// Releases an instance of the type based on the rules of the behavior.
		/// </summary>
		/// <param name="context">The context in which the instance was activated.</param>
		public override void Release(IContext context)
		{
			Ensure.NotDisposed(this);
			context.Binding.Components.Activator.Destroy(context);
		}
		#endregion
		/*----------------------------------------------------------------------------------------*/
	}
}