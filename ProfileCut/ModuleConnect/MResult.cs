using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModuleConnect
{
    public class MResult
    {
        public int Code { set; get; }
        public string Description { set; get; }
        public string Module { set; get; }
        public bool Succeeded { get { return Code == 0; } }
        public MResult(int code=0, string description="")
        {
            this.Code = code;
            this.Description = description;
        }
        public void ThrowIfError()
        {
            if (!Succeeded)
                throw new Exception(String.Format("Модуль {0} вернул ошибку с кодом {1}. {2}", Module, Code, Description==""?"":"Описание:" + Description));
        }
    }
}
