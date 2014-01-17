
namespace HardwareInterfaces
{
    public interface IHardwareCommand {
        void Execute();        
    }


	public interface ITestFunction1: IHardwareCommand
	{
		int Test(int a, int b);
	}
	/*public class TestModule : Module
	{
		public TestClass c1;

		public TestModule(string modulePath)
			: base(modulePath)
		{

		}
		protected override void FillClasses(Assembly assembly)
		{
			c1 = new TestClass(assembly);
		}
	}

	public class TestClass : ModuleClass, ITestModule1
	{
		ModuleMethod _TestInt;
		ModuleMethod _Test;
		public TestClass(Assembly asm) : base(asm, "TestModule", "Class1", null)
		{
			
		}
		protected override void FillMethods(Type type)
		{
			this._TestInt = new ModuleMethod(type, this._instance, "TestInt");
			this._Test = new ModuleMethod(type, this._instance, "Test");
		}
		public int TestInt(int a, int b)
		{
			object[] param = { a, b };
			return (int)_TestInt.Call(param);
		}
		public void Test()
		{
			_Test.Call(null);
		}
	}*/
}