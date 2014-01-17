using System;
using System.Runtime.InteropServices;

namespace WinDLL
{
	class win32
	{
		[DllImport("kernel32.dll")]
		public static extern IntPtr LoadLibrary(string dllToLoad);

		[DllImport("kernel32.dll")]
		public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

		[DllImport("kernel32.dll")]
		public static extern bool FreeLibrary(IntPtr hModule);
	}

	public abstract class RuntimeDll
	{
		public string dllPath { get; private set; }
		private IntPtr _hDll;

		public RuntimeDll(string dllPath)
		{
			this.dllPath = dllPath;
			this._hDll = win32.LoadLibrary(dllPath);
			if (this._hDll == IntPtr.Zero)
				throw new Exception("Не удалось загрузить библиотеку \"" + dllPath + "\"");
			this.Init();
		}

		~RuntimeDll()
		{
			if (this._hDll != IntPtr.Zero)
				win32.FreeLibrary(this._hDll);
		}

		public T LoadFunction<T>(string procName) where T : class
		{
			// коммент
			IntPtr address = win32.GetProcAddress(this._hDll, procName);
			if (address == IntPtr.Zero)
				throw new Exception("Функция \"" + procName + "\" не найдена в библиотеке \"" + this.dllPath + "\"");
			System.Delegate fn_ptr = Marshal.GetDelegateForFunctionPointer(address, typeof(T));
			return fn_ptr as T;
		}

		abstract protected void Init();
	}
}
