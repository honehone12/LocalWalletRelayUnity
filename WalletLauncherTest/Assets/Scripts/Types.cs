using System;
using System.Collections.Generic;

[Serializable]
public class RpcResponse
{
    public string message;

    public RpcResponse(string message)
    {
        this.message = message;
    }
}

[Serializable]
public class RpcRequest
{
    public string type;
    public string function;
    public List<string> arguments;
    public List<string> type_arguments;

    public RpcRequest(
        string function, 
        List<string> arguments, 
        List<string> type_arguments)
    {
        type = "entry_function_payload";
        this.function = function;
        this.arguments = arguments;
        this.type_arguments = type_arguments;
    }
}
