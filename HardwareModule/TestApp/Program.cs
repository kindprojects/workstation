using System;
using ModuleConnect;

namespace TestApp
{
	class Program
	{
		static int Main()
		{
			try
			{
				Console.WriteLine("[ tester app ]");
				HardwareModule m = new HardwareModule(@"D:\TFS\Prom\HardwareModule\TestModule\bin\Debug\TestModule.dll");
				HardwareInterfaces.ITestFunction1 i = m.GetClassInstance<HardwareInterfaces.ITestFunction1>("HardwareModule", "TestFunction");
				Console.WriteLine(i.Test(2, 3));

				//TestModule m = new TestModule(@"D:\TFS\Prom\HardwareModule\TestModule\bin\Debug\TestModule.dll");
				//ITestModule1 i = m.c1;
				//i.Test();
				//Console.WriteLine("test result: " + i.TestInt(2, 3).ToString());
			}
			catch (Exception e)
			{
				Console.WriteLine("[" + e.GetType().ToString() + "] " + e.Message);
			}
			Console.WriteLine("Press ENTER to exit");
			Console.ReadLine();
			return 0;
		}
	}
}
