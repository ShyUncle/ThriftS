using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using ThriftS.Common.Attributes;

namespace ThriftS.Common
{
    /// <summary>
    /// 静态辅助方法
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// 版本
        /// </summary>
        public static string Version
        {
            get { return "0.6"; }
        }

        /// <summary>
        /// 获取服务名称
        /// </summary>
        /// <param name="contractType">类型</param>
        /// <returns>服务名</returns>
        public static string GetServiceName(Type contractType)
        {
            var serviceName = string.Empty;
            var attrs = Attribute.GetCustomAttributes(contractType);
            foreach (var attr in attrs)
            {
                var attribute = attr as ThriftSContractAttribute;
                if (attribute != null)
                {
                    serviceName = attribute.ServiceName;
                    break;
                }
            }

            return serviceName;
        }

        /// <summary>
        /// 获取方法名称
        /// </summary>
        /// <param name="method">Method</param>
        /// <returns>方法名</returns>
        public static string GetMethodName(MethodInfo method)
        {
            var methodName = method.Name;
            var attr = Attribute.GetCustomAttribute(method, typeof(ThriftSOperationAttribute), false);
            if (attr != null)
            {
                var attribute = attr as ThriftSOperationAttribute;
                if (string.IsNullOrEmpty(attribute.Name) == false)
                {
                    methodName = attribute.Name;
                }
            }

            return methodName;
        }

        /// <summary>
        /// 获取类型序列化方式
        /// </summary>
        /// <param name="targetType">类型</param>
        /// <returns>SerializerMode</returns>
        public static SerializerMode GetSerializerMode(Type targetType)
        {
            // 值类型和字符串
            if (targetType.IsValueType || targetType == typeof(string))
            {
                return SerializerMode.Thrift;
            }

            // 检查是否使用Attribute指定序列化类型
            if (targetType.IsDefined(typeof(ThriftSModelAttribute), false))
            {
                var modelAttr = (ThriftSModelAttribute)Attribute.GetCustomAttribute(targetType, typeof(ThriftSModelAttribute), false);
                if (modelAttr.SerializerMode != SerializerMode.Thrift)
                {
                    return modelAttr.SerializerMode;
                }
            }

            return SerializerMode.Thrift;
        }

        /// <summary>
        /// Gzip压缩
        /// </summary>
        /// <param name="buffer">buffer</param>
        /// <returns>结果</returns>
        public static byte[] GzipCompress(byte[] buffer)
        {
            if (buffer == null)
            {
                return null;
            }

            using (var ms = new MemoryStream())
            using (var zs = new GZipStream(ms, CompressionMode.Compress))
            {
                zs.Write(buffer, 0, buffer.Length);
                zs.Close();//必须关闭
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Gzip解压
        /// </summary>
        /// <param name="buffer">buffer</param>
        /// <returns>结果</returns>
        public static byte[] GzipUnCompress(byte[] buffer)
        {
            if (buffer == null)
            {
                return null;
            }

            using (var sourceStream = new MemoryStream(buffer))
            using (var gzipStream = new GZipStream(sourceStream, CompressionMode.Decompress))
            {
                const int bufferSize = 4096;
                int bytesRead = 0;

                byte[] bufChunk = new byte[bufferSize];
                using (MemoryStream targetStream = new MemoryStream()) 
                {
                    while ((bytesRead = gzipStream.Read(bufChunk, 0, bufferSize)) > 0)
                    {
                        targetStream.Write(bufChunk, 0, bytesRead);
                    }

                    return targetStream.ToArray();
                }
            }
        }
    }
}
