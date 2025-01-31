using ccxt;
using ccxt.pro;

using Newtonsoft.Json;

using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
namespace Tests;
using dict = Dictionary<string, object>;

public partial class testMainClass : BaseTest
{
    public static SharedMethods testSharedMethods = new SharedMethods();
    // public Exchange exchange = new Exchange();
    // public dict testFiles = new dict();
    // consts to be accessed from transpiled tests
    public static string EXT = "cs";
    public static string LANG = "C#";
    public static bool IS_SYNCHRONOUS = false;
    public static string PROXY_TEST_FILE_NAME = "proxies";
    public static string ROOT_DIR = Tests.ccxtBaseDir + "/";
    public static dict ENV_VARS = null;
    public static string NEW_LINE = "\n";

    //public bool info = Tests.info;
    //public bool verbose = Tests.verbose;
    //public bool debug = Tests.debug;
    public static string httpsAgent = "";
    //public bool loadKeys = false;
    public static int TICK_SIZE = Exchange.TICK_SIZE;

    // public static object AuthenticationError = typeof(Exchange.AuthenticationError);
    public static Exchange initExchange(object exchangeId, object exchangeArgs = null, bool isWs = false)
    {
        if (isWs)
        {
            // var binance = new ccxt.binance();
            exchangeId = "ccxt.pro." + (string)exchangeId;// + "Ws";
        }
        var exchange = Exchange.DynamicallyCreateInstance((string)exchangeId, exchangeArgs);
        return exchange;
    }

    public static bool getCliArgValue(string option)
    {
        if (Tests.args.Contains(option))
            return true;
        return false;
    }


    public testMainClass()
    {
        initEnv();

    }

    dict getTestFilesSync(object properties, bool ws = false)
    {
        return null; // empty in c#
    }

    async Task<dict> getTestFiles(object properties, bool ws = false)
    {
        // var hasDict = properties as dict;
        // var hasKeys = hasDict.Keys;
        var testFiles = new dict();
        var hasKeys = properties as List<object>;
        hasKeys.Add("features");
        foreach (var key2 in hasKeys)
        {
            var key = key2 as string;
            var testFilePath = "";
            if (!ws)
            {
                testFilePath = ROOT_DIR + "cs/tests/Generated/Exchange/test." + key + ".cs";
            }
            else
            {
                testFilePath = ROOT_DIR + "cs/tests/Generated/Exchange/Ws/test." + key + ".cs";
            }
            if (ioFileExists(testFilePath))
            {
                var methodName = "test" + key.Substring(0, 1).ToUpper() + key.Substring(1);
                var testMethod = this.GetType().GetMethod(methodName);
                testFiles[key] = testMethod;
            }
        }
        return testFiles;
    }

    public object jsonStringify(object a)
    {
        return JsonConvert.SerializeObject(a);
    }

    public object jsonParse(object a)
    {
        // var jsonString = a.ToString();
        // if (jsonString.StartsWith("["))
        // {
        //     return JsonConvert.DeserializeObject<List<dict>>(jsonString);
        // }
        return JsonHelper.Deserialize((string)a);
    }

    void initEnv()
    {
        var vars = Environment.GetEnvironmentVariables();
        var parsedObject = new Dictionary<string, object>();
        for (var i = 0; i < vars.Count; i++)
        {
            var key = vars.Keys.Cast<string>().ElementAt(i);
            var value = vars[key];
            parsedObject[key] = value;
        }
        ENV_VARS = parsedObject;
    }

    async static Task close(object exchange)
    {
        // stub
    }

    public static void dump(params object[] values)
    {

        var parsedValues = new List<string> { };
        foreach (var value in values)
        {
            if (value is IList<object> || value is IDictionary<string, object>)
            {
                parsedValues.Add(JsonConvert.SerializeObject(value));
            }
            parsedValues.Add(value.ToString());
        }
        Console.WriteLine(string.Join(" ", parsedValues));
    }

    public static bool ioFileExists(object path2)
    {
        var path = path2 as string;
        var exists = System.IO.File.Exists(path);
        return exists;
    }

    public static object ioFileRead(object path2)
    {
        var path = path2 as string;
        var text = System.IO.File.ReadAllText(path);
        return JsonHelper.Deserialize(text);
    }

    public static object ioDirRead(object path2)
    {
        var path = path2 as string;
        var filesInDir = System.IO.Directory.GetFiles(path);
        // return filesInDir.ToList<string>();
        var fileNameOnly = new List<string>();
        foreach (string fileName in filesInDir)
        {
            fileNameOnly.Add(Path.GetFileName(fileName));
        }
        return fileNameOnly;
    }

    public object callMethodSync(object testFiles2, object methodName, object exchange, params object[] args)
    {
        return null; // empty in c#
    }

    public async Task<object> callMethod(object testFiles2, object methodName, object exchange, params object[] args)
    {
        var argsWithExchange = new List<object> { exchange };
        foreach (var arg in args)
        {
            if (arg == null) continue; // skip if no arguments passed into method
            // emulate ... spread operator in c#
            if (arg.GetType() == typeof(List<object>))
            {
                argsWithExchange.AddRange(arg as List<object>);
                continue;
            }
            argsWithExchange.Add(arg);
        }
        var testFiles = testFiles2 as dict;
        var method = testFiles[methodName as string] as MethodInfo;
        var res = method.Invoke(exchange, argsWithExchange.ToArray());
        await ((Task)res);
        return null;
    }

    public async Task<object> callExchangeMethodDynamically(object exchange, object methodName, params object[] args)
    {
        // args ??= new object[] { };
        // if (args.Length == 0)
        // {
        //     args = new object[] { null };
        // }
        var realArgs = (args.Length == 0) ? new List<object> { } : args[0] as List<object>;
        var method = exchange.GetType().GetMethod((string)methodName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        var parameters = method.GetParameters();
        var newArgs = new object[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            if (i < realArgs.Count)
            {
                newArgs[i] = realArgs[i];
            }
            else
            {
                newArgs[i] = null;
            }
        }
        var res = method.Invoke(exchange, newArgs);
        var awaittedResult = await ((Task<object>)res);
        return awaittedResult;
    }

    public object callExchangeMethodDynamicallySync(object exchange, object methodName, params object[] args)
    {
        throw new Exception("This functions shouldn't be used in C#");
    }

    public static void addProxy(object exchange, object proxy)
    {
        exchange.GetType().GetProperty("httpProxy").SetValue(exchange, proxy);
    }

    public static void exitScript(int exit = 0)
    {
        Environment.Exit(exit);
    }

    public static object getExchangeProp(object exchange, object prop, object defaultValue = null)
    {
        try
        {
            var propertyInfo = exchange.GetType().GetProperty(prop as string);
            if (propertyInfo != null)
            {
                var value = propertyInfo.GetValue(exchange);
                return value != null ? value : defaultValue;
            }
            else
            {
                return defaultValue;
            }
        }
        catch (Exception)
        {
            return defaultValue;
        }

    }

    public static void setExchangeProp(object exchange, object prop, object value)
    {
        try
        {
            exchange.GetType().GetProperty(prop as string).SetValue(exchange, value);
        }
        catch (Exception)
        {
            // do nothing
        }
    }

    public void add_proxy(Exchange exchange, object http_proxy)
    {
        exchange.proxy = http_proxy as string;
    }

    public string getTestName(object str2)
    {
        var str = (string)str2;
        return "test" + char.ToUpper(str[0]) + str.Substring(1);
    }

    public string exceptionMessage(object exc)
    {
        var e = exc as Exception;
        return e.Message;
    }

    public System.Exception getRootException(Exception exc)
    {
        if (exc is System.AggregateException)
        {
            var inner = exc.InnerException;
            if (inner != null)
            {
                return inner;
            }
        }
        return exc;
    }

    public Exchange setFetchResponse(object exchange2, object response)
    {
        var exchange = exchange2 as Exchange;

        exchange.fetchResponse = response;
        return exchange;

    }

    public bool isNullValue(object value)
    {
        return value == null;
    }

    public bool isSync()
    {
        return false;
    }

    public string getExt()
    {
        return EXT;
    }

    public string getLang()
    {
        return LANG;
    }

    public object getEnvVars()
    {
        return ENV_VARS;
    }

    public string getRootDir()
    {
        return ROOT_DIR;
    }

    public object convertAscii(object input)
    {
        // tmp fix the issue inside ascii-encoded json values
        // "[{\"symbol\":\"BTC-USDT\",\"type\":\"LIMIT\",\"side\":\"BUY\",\"quantity\":0.0002,\"price\":25000.0},{\"symbol\":\"BTC-USDT\",\"type\":\"LIMIT\",\"side\":\"BUY\",\"quantity\":0.0002,\"price\":27000.0}]"
        // "[{\"symbol\":\"BTC-USDT\",\"type\":\"LIMIT\",\"side\":\"BUY\",\"quantity\":0.0002,\"price\":25000},{\"symbol\":\"BTC-USDT\",\"type\":\"LIMIT\",\"side\":\"BUY\",\"quantity\":0.0002,\"price\":27000}]"
        // as you can see the numeric values might be 25 or 25.0
        // so we need to convert them to the same format
        // this is done when the message is a regular json-string or url-encoded string
        var decodedString = System.Web.HttpUtility.UrlDecode(input as string);
        // decodedString = decodedString.Replace(".0}", "}");
        // decodedString = decodedString.Replace(".0,", ",");
        // string pattern = @"(?<=\.\d*)0+(?!\d)|(?<=\d)\.0+$";
        // string pattern = @"(?<=\.\d*[1-9])0+|(?<=[0-9])\.0+$";
        // string pattern = @"(?<=\d)\.0+$|(\.\d*?[1-9])0+$";
        string pattern = @"(?<=\.[0-9]*[1-9])0+\b|(?<=\d)\.0+\b";


        string result1 = Regex.Replace(decodedString, pattern, "");
        return result1;

    }

    public partial class SharedMethods
    {
        // ast-transpiler uses "json()" method in transpiled C# content,
        // which should pre-exist in the language-specific helpers for project
        public object json(object a)
        {
            return Exchange.Json(a);
        }
    }
}