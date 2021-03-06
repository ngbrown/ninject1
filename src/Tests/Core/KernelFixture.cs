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
using Ninject.Core;
using Ninject.Core.Activation;
using Ninject.Core.Injection;
using Ninject.Core.Planning;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
#endregion

namespace Ninject.Tests
{
	[TestFixture]
	public class KernelFixture
	{
		/*----------------------------------------------------------------------------------------*/
		[Test]
		public void KernelComponentsInstalledAndConnected()
		{
			using (var kernel = new StandardKernel())
			{
				var activator = kernel.Components.Activator;
				Assert.That(activator, Is.Not.Null);
				Assert.That(activator.IsConnected);

				var planner = kernel.Components.Planner;
				Assert.That(planner, Is.Not.Null);
				Assert.That(planner.IsConnected);

				var injectorFactory = kernel.Components.InjectorFactory;
				Assert.That(injectorFactory, Is.Not.Null);
				Assert.That(injectorFactory.IsConnected);
			}
		}
		/*----------------------------------------------------------------------------------------*/
		[Test]
		public void StandardKernelUsesDefaultOptions()
		{
			using (var kernel = new StandardKernel())
			{
				Assert.That(kernel.Options, Is.SameAs(KernelOptions.Default));
			}
		}
		/*----------------------------------------------------------------------------------------*/
		[Test, ExpectedException(typeof(InvalidOperationException))]
		public void MissingComponentsThrowsException()
		{
			new InvalidKernel();
		}
		/*----------------------------------------------------------------------------------------*/
	}
}