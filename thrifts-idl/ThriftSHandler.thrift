include "ThriftSRequest.thrift"
include "ThriftSResponse.thrift"
include "ThriftSException.thrift"

namespace csharp ThriftS.IDL
namespace java thrifts.idl

service ThriftSHandler {
    ThriftSResponse.ThriftSResponse Ping(1:ThriftSRequest.ThriftSRequest request) throws (1:ThriftSException.BadRequestException badRequestException,2:ThriftSException.InternalServerException internalServerException)
	ThriftSResponse.ThriftSResponse Hello(1:ThriftSRequest.ThriftSRequest request) throws (1:ThriftSException.BadRequestException badRequestException,2:ThriftSException.InternalServerException internalServerException)
    ThriftSResponse.ThriftSResponse Process(1:ThriftSRequest.ThriftSRequest request) throws (1:ThriftSException.BadRequestException badRequestException,2:ThriftSException.InternalServerException internalServerException,3:ThriftSException.InvocationException invocationException)
}