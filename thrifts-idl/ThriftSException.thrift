include "ThriftSRequest.thrift"

namespace csharp ThriftS.IDL
namespace java thrifts.idl

exception BadRequestException {
    1: required ThriftSRequest.ThriftSRequest Request,
    2: required string ErrorMessage,
}

exception InternalServerException {
    1: required ThriftSRequest.ThriftSRequest Request,
    2: required string ErrorMessage,
	3: optional string ErrorDescription
}

exception InvocationException {
    1: required ThriftSRequest.ThriftSRequest Request,
    2: required string ErrorMessage,
	3: optional string ErrorDescription
}

