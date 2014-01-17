using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Reflection; 
using System.Runtime.InteropServices;


namespace ProfileCut
{

    static class win32
    {

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate Boolean Test(string arg);

    }

    class plugins
    {
        public void Load()
        {
            IntPtr dll = win32.LoadLibrary("dll.dll");
            IntPtr addr = win32.GetProcAddress(dll, "test");


            win32.Test test =  (win32.Test)Marshal.GetDelegateForFunctionPointer(addr, typeof(win32.Test));
            test("xxxxxxxxxxxxxxxxxxxxxxxx");
            


            win32.FreeLibrary(dll);
        }

    }

    //struct ATTRLIST
    //{
    //    char* name;
    //    ATTRLIST* next;
    //};

    //struct VALUELIST
    //{
    //    char* attr_name;
    //    char* value;
    //    VALUELIST* next;
    //};

    //struct OBJECTLIST
    //{
    //    VALUELIST* attr_values;
    //    OBJECTLIST* next;
    //};

    //struct MODULE_PARAMS
    //{
    //    ATTRLIST* attributes;
    //    OBJECTLIST* objects;
    //};

    //class ClassTemp
    //{
    //    public void Load(string name)
    //    {
    //    try {
    //        Assembly asm = Assembly.LoadFile(name);
    //        asm.GetTypes();
    //    }
    //    catch (Exception ex) 
    //    {
    //    }

            //}

//        MODULE_PARAMS* createParams(){
//    MODULE_PARAMS params2 = new MODULE_PARAMS();
//    params2.
//    params->objects = NULL;
//    params->attributes = NULL;
//    return params;
//}



//void deleteParams(MODULE_PARAMS *params){
//    delete params;
//}

//        //public void main(){
//        //TESTPROC proc;
//        //HINSTANCE hLib;
//        //hLib = LoadLibrary(DLL_PATH);
//        //if (hLib != NULL){
//        //    proc = (TESTPROC)GetProcAddress(hLib, DLL_PROCNAME);
//        //    if (proc != NULL){
//        //    }else{
//        //    }
//        //    FreeLibrary(hLib);
//        //}else{
//        //    mb("Íå óäàëîñü çàãðóçèòü áèáëèîòåêó "DLL_PATH);
//        //}
    //}
}
