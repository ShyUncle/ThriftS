include "ThriftSRequest.thrift"

namespace csharp ThriftS.IDL
namespace java thrifts.idl

struct ThriftSResult {
	1: required string ContentType,
	2: required ThriftSRequest.ThriftSCompression Compression = ThriftSRequest.ThriftSCompression.None,
	3: optional binary Data,
}

struct ThriftSResponse {
	1: required map<string,string> Headers,
	2: optional ThriftSResult Result
}