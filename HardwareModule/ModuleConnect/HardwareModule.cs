using System;
using System.Reflection;

namespace ModuleConnect
{
	public class HardwareModule
	{
		Assembly _asm;
		public HardwareModule(string dllPath)
		{
			this._asm = Assembly.LoadFile(dllPath);
		}

		public T GetClassInstance<T>(string nameSpace, string className) where T : class
		{
			string classPath = nameSpace+'.'+className;
			Type classType = this._asm.GetType(classPath);
			if (classType == null)
			{
				throw new Exception("Класс \"" + classPath + "\" не найден!");
			}
			else
			{
				try
				{
					object o = Activator.CreateInstance(classType);
					return o as T;
				}
				catch (Exception e)
				{
					throw new Exception("[" + e.GetType().ToString() + "] Не удалось создать объект \"" + classPath + "\"\n" + e.Message);
				}
			}
		}
	}
}
