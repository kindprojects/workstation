using System;
using System.Reflection;
using System.Collections.Generic;

namespace ModuleConnect
{
	public class MConnect
	{
		Assembly _asm;
		public MConnect(string dllPath)
		{
			try
			{
				this._asm = Assembly.LoadFile(dllPath);
			}
			catch (Exception ex)
			{
				throw new Exception("Не удалось загрузить модуль:\n" + dllPath
					+ "\nОписание:\n" + ex.Message, ex);
			}
		}

		private T _getClassInstance<T>(string nameSpace, string className, Dictionary<string,string> moduleParams) where T : class
		{
            string classPath = nameSpace + '.' + className;
			Type classType = this._asm.GetType(classPath);
			if (classType == null)
			{
				throw new Exception("Класс \"" + classPath + "\" не найден!");
			}
			else
			{
				try
				{
					object o = Activator.CreateInstance(classType, moduleParams);
					return o as T;
				}
				catch (Exception e)
				{
					throw new Exception("[" + e.GetType().ToString() + "] Не удалось создать объект \"" + classPath + "\"\n" + e.Message);
				}
			}
		}

        public IModule GetModuleInterface(Dictionary<string,string> moduleParams)
        {
            return _getClassInstance<IModule>("ModuleNamespace", "ModuleClass", moduleParams);
        }
	}
}