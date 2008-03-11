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
using Ninject.Core.Activation;
using Ninject.Core.Binding;
using Ninject.Core.Tests.Mocks;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
#endregion

namespace Ninject.Core.Tests
{
	[TestFixture]
	public class DebugInfoFixture
	{
		/*----------------------------------------------------------------------------------------*/
		[Test]
		public void DebugInfoCreatedFromStackTrace()
		{
			IModule module = new TestableModule(delegate(TestableModule m)
			{
				m.Bind<IMock>().To<SimpleObject>();
			});

			KernelOptions options = new KernelOptions();
			options.GenerateDebugInfo = true;

			using (IKernel kernel = new StandardKernel(options, module))
			{
				IContext context = new StandardContext(kernel, typeof(IMock));
				IBinding binding = kernel.GetBinding<IMock>(context);

				Assert.That(binding, Is.Not.Null);
				Assert.That(binding.DebugInfo, Is.Not.Null);
			}
		}
		/*----------------------------------------------------------------------------------------*/
		[Test]
		public void DebugInfoFromStackFrameContainsFileInfo()
		{
			IModule module = new TestableModule(delegate(TestableModule m)
			{
				m.Bind<IMock>().To<SimpleObject>();
			});

			KernelOptions options = new KernelOptions();
			options.GenerateDebugInfo = true;

			using (IKernel kernel = new StandardKernel(options, module))
			{
				IContext context = new StandardContext(kernel, typeof(IMock));
				IBinding binding = kernel.GetBinding<IMock>(context);

				Assert.That(binding, Is.Not.Null);
				Assert.That(binding.DebugInfo, Is.Not.Null);
				Assert.That(binding.DebugInfo.FileName, Is.Not.Null);
			}
		}
		/*----------------------------------------------------------------------------------------*/
	}
}