using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace TestModule
{
	public class TestDll : WinDLL.RuntimeDll
	{
		public TestDll(string dllPath)
			: base(dllPath)
		{

		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public delegate int TFunc1(string msg);
		public TFunc1 f1;

		protected override void Init()
		{
			this.f1 = this.LoadFunction<TFunc1>("test");
		}
	}

	public class Class1 : HardwareInterfaces.ITestFunction1
	{
		public void TestHardware()
		{
			TestDll d1 = new TestDll("TestModule1.dll");
			Encoding e = new UnicodeEncoding(false, true, true);
			d1.f1("Превед!");
		}

		public int Test(int a, int b)
		{
			return a * b;
		}

        public void Execute()
        {

        }
	}
}
