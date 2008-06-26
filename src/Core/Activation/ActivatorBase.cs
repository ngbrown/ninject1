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
using Ninject.Core.Activation.Strategies;
using Ninject.Core.Infrastructure;
#endregion

namespace Ninject.Core.Activation
{
	/// <summary>
	/// The baseline implemenation of an activator with no strategies installed. This type can be
	/// extended to customize the activation process.
	/// </summary>
	public abstract class ActivatorBase : KernelComponentBase, IActivator
	{
		/*----------------------------------------------------------------------------------------*/
		#region Properties
		/// <summary>
		/// The chain of activation strategies.
		/// </summary>
		public IStrategyChain<IActivator, IActivationStrategy> Strategies { get; private set; }
		#endregion
		/*----------------------------------------------------------------------------------------*/
		#region Event Sources
		/// <summary>
		/// Called when the component is connected to its environment.
		/// </summary>
		/// <param name="args">The event arguments.</param>
		protected override void OnConnected(EventArgs args)
		{
			Strategies.Kernel = Kernel;

			base.OnConnected(args);
		}
		/*----------------------------------------------------------------------------------------*/
		/// <summary>
		/// Called when the component is disconnected from its environment.
		/// </summary>
		/// <param name="args">The event arguments.</param>
		protected override void OnDisconnected(EventArgs args)
		{
			base.OnDisconnected(args);

			if (Strategies != null)
				Strategies.Kernel = null;
		}
		#endregion
		/*----------------------------------------------------------------------------------------*/
		#region Disposal
		/// <summary>
		/// Releases all resources held by the object.
		/// </summary>
		/// <param name="disposing"><see langword="True"/> if managed objects should be disposed, otherwise <see langword="false"/>.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && !IsDisposed)
			{
				DisposeCollection(Strategies);
				Strategies = null;
			}

			base.Dispose(disposing);
		}
		#endregion
		/*----------------------------------------------------------------------------------------*/
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="ActivatorBase"/> class.
		/// </summary>
		protected ActivatorBase()
		{
			Strategies = new StrategyChain<IActivator, IActivationStrategy>(this);
		}
		#endregion
		/*----------------------------------------------------------------------------------------*/
		#region Public Methods
		/// <summary>
		/// Activates an instance by executing the chain of activation strategies.
		/// </summary>
		/// <param name="context">The activation context.</param>
		public void Activate(IContext context)
		{
			Ensure.ArgumentNotNull(context, "context");
			Ensure.NotDisposed(this);

			if (context.Instance == null)
			{
				// Execute the "before create" phase.
				foreach (IActivationStrategy strategy in Strategies)
				{
					if (strategy.BeforeCreate(context) == StrategyResult.Stop)
						break;
				}

				// Request a new instance from the binding's provider.
				context.Instance = context.Binding.Provider.Create(context);

				if (context.Instance == null)
					throw new ActivationException(ExceptionFormatter.ProviderCouldNotCreateInstance(context));

				// Execute the "after create" phase.
				foreach (IActivationStrategy strategy in Strategies)
				{
					if (strategy.AfterCreate(context) == StrategyResult.Stop)
						break;
				}
			}

			// Execute the "initialize" phase.
			foreach (IActivationStrategy strategy in Strategies)
			{
				if (strategy.Initialize(context) == StrategyResult.Stop)
					break;
			}

			// Execute the "after initialize" phase.
			foreach (IActivationStrategy strategy in Strategies)
			{
				if (strategy.AfterInitialize(context) == StrategyResult.Stop)
					break;
			}
		}
		/*----------------------------------------------------------------------------------------*/
		/// <summary>
		/// Destroys an instance by executing the chain of destruction strategies.
		/// </summary>
		/// <param name="context">The context in which the instance was requested.</param>
		public void Destroy(IContext context)
		{
			Ensure.ArgumentNotNull(context, "context");
			Ensure.NotDisposed(this);

			// TODO
			if (context.Instance == null)
				throw new InvalidOperationException("Context does not contain an instance to destroy");

			// Execute the "before destroy" phase.
			foreach (IActivationStrategy strategy in Strategies)
			{
				if (strategy.BeforeDestroy(context) == StrategyResult.Stop)
					break;
			}

			// Execute the "destroy" phase.
			foreach (IActivationStrategy strategy in Strategies)
			{
				if (strategy.Destroy(context) == StrategyResult.Stop)
					break;
			}

			// Execute the "after destroy" phase.
			foreach (IActivationStrategy strategy in Strategies)
			{
				if (strategy.AfterDestroy(context) == StrategyResult.Stop)
					break;
			}
		}
		#endregion
		/*----------------------------------------------------------------------------------------*/
	}
}