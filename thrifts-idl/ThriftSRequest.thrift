namespace csharp ThriftS.IDL
namespace java thrifts.idl

enum ThriftSCompression {
    None = 0,
    Gzip = 1
}

struct ThriftSParameter {
    1: required byte Index,
    2: required string Name,
	3: required string Type,
	4: required string ContentType,
	5: required ThriftSCompression Compression = ThriftSCompression.None,
	6: required bool HasValue = true,
	7: optional binary Value
}

struct ThriftSRequest {
	1: required map<string,string> Headers,
	2: optional string ServiceName,
	3: optional string MethodName,
	4: optional list<ThriftSParameter> Parameters
}