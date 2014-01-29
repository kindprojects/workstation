using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ServiceStack.Text;

namespace JsonPrinter
{
    public interface ISTest
    {
        string str { set; get; }
    }

    public class STest: ISTest
    {
        public string str { set; get; }

        public STest()
        {
            str = "1111";
        }
    }

    public class JPrinter
    {
        public List<JPrinterCommand> Commands { set; get; }

        public JPrinter()
        {
            Commands = new List<JPrinterCommand>();
        }
    }

    public abstract class JPrinterCommand{
        public string Command  { set; get; }
    }

    public class JCommandLabel:JPrinterCommand
    {
        public JLabelParams Params { set; get; }
        public JCommandLabel(int x, int y, string text)
        {
            base.Command = "LBL";
            Params = new JLabelParams();
            Params.x = x;
            Params.y = y;
            Params.text = text;
        }
    }
    public class JLabelParams
    {
        public int x  { set; get; }
        public int y { set; get; }
        public string text { set; get; }
    }
    
    public class Serializer
    {
        public static string GetJson(JPrinter printer)
        {
            return JsonSerializer.SerializeToString<JPrinter>(printer);
        }

        public static JPrinter GetPrinter(string json)
        {
            return JsonSerializer.DeserializeFromString<JPrinter>(json);
        }

        public static string GetJsonTest(ISTest xxx)
        {
            return JsonSerializer.SerializeToString<ISTest>(xxx);
        }
    }
}
