#region License
//
// Author: Nate Kohari <nkohari@gmail.com>
// Copyright (c) 2007, Enkari, Ltd.
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
using System.Collections.Generic;
using Ninject.Core.Binding;
using Ninject.Core.Infrastructure;
using Ninject.Core.Planning.Strategies;
#endregion

namespace Ninject.Core.Planning
{
	/// <summary>
	/// The baseline implemenation of a planner with no strategies installed. This type can be
	/// extended to customize the planning process.
	/// </summary>
	public abstract class PlannerBase : KernelComponentBase, IPlanner
	{
		/*----------------------------------------------------------------------------------------*/
		#region Fields
		private StrategyChain<IPlanner, IPlanningStrategy> _strategies;
		private Dictionary<Type, IActivationPlan> _plans = new Dictionary<Type, IActivationPlan>();
		#endregion
		/*----------------------------------------------------------------------------------------*/
		#region Properties
		/// <summary>
		/// The chain of strategies that contribute to the creation and destruction of activation plans.
		/// </summary>
		/// <value></value>
		public IStrategyChain<IPlanner, IPlanningStrategy> Strategies
		{
			get { return _strategies; }
		}
		#endregion
		/*----------------------------------------------------------------------------------------*/
		#region Event Sources
		/// <summary>
		/// Called when the component is connected to its environment.
		/// </summary>
		/// <param name="args">The event arguments.</param>
		protected override void OnConnected(EventArgs args)
		{
			_strategies.Kernel = Kernel;
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

			if (_strategies != null)
				_strategies.Kernel = null;
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
				DisposeCollection(_strategies);
				DisposeDictionary(_plans);

				_strategies = null;
				_plans = null;
			}

			base.Dispose(disposing);
		}
		#endregion
		/*----------------------------------------------------------------------------------------*/
		#region Constructors
		/// <summary>
		/// Creates a new PlannerBase.
		/// </summary>
		protected PlannerBase()
		{
			_strategies = new StrategyChain<IPlanner, IPlanningStrategy>(this);
		}
		#endregion
		/*----------------------------------------------------------------------------------------*/
		#region Public Methods
		/// <summary>
		/// Builds a new activation plan by inspecting the specified type.
		/// </summary>
		/// <param name="binding">The binding that was used to resolve the type being activated.</param>
		/// <param name="type">The type to examine.</param>
		/// <returns>An activation plan that will be used to build instances type.</returns>
		public IActivationPlan GetPlan(IBinding binding, Type type)
		{
			Ensure.ArgumentNotNull(binding, "binding");
			Ensure.ArgumentNotNull(type, "type");
			Ensure.NotDisposed(this);

			lock (_plans)
			{
				if (_plans.ContainsKey(type))
					return _plans[type];

				IActivationPlan plan = CreateEmptyPlan(type);
				_plans.Add(type, plan);

				foreach (IPlanningStrategy strategy in _strategies)
				{
					if (strategy.BeforeBuild(binding, type, plan) == StrategyResult.Stop)
						break;
				}

				foreach (IPlanningStrategy strategy in _strategies)
				{
					if (strategy.Build(binding, type, plan) == StrategyResult.Stop)
						break;
				}

				foreach (IPlanningStrategy strategy in _strategies)
				{
					if (strategy.AfterBuild(binding, type, plan) == StrategyResult.Stop)
						break;
				}

				return plan;
			}
		}
		/*----------------------------------------------------------------------------------------*/
		/// <summary>
		/// Releases the activation plan for the specified type, if one was created.
		/// </summary>
		/// <param name="binding">The binding which points to the type that should be released.</param>
		/// <param name="type">The type whose activation plan should be released.</param>
		public void ReleasePlan(IBinding binding, Type type)
		{
			Ensure.ArgumentNotNull(binding, "binding");
			Ensure.ArgumentNotNull(type, "type");
			Ensure.NotDisposed(this);

			lock (_plans)
			{
				if (!_plans.ContainsKey(type))
					return;

				IActivationPlan plan = _plans[type];

				foreach (IPlanningStrategy strategy in _strategies)
				{
					if (strategy.BeforeRelease(binding, type, plan) == StrategyResult.Stop)
						break;
				}

				foreach (IPlanningStrategy strategy in _strategies)
				{
					if (strategy.Release(binding, type, plan) == StrategyResult.Stop)
						break;
				}

				foreach (IPlanningStrategy strategy in _strategies)
				{
					if (strategy.AfterRelease(binding, type, plan) == StrategyResult.Stop)
						break;
				}

				_plans.Remove(type);
			}
		}
		#endregion
		/*----------------------------------------------------------------------------------------*/
		#region Protected Methods
		/// <summary>
		/// Creates a new empty activation plan, that will subsequently be built up by the inspector.
		/// </summary>
		/// <returns>The new empty binding.</returns>
		protected virtual IActivationPlan CreateEmptyPlan(Type type)
		{
			return new StandardActivationPlan(type);
		}
		#endregion
		/*----------------------------------------------------------------------------------------*/
	}
}