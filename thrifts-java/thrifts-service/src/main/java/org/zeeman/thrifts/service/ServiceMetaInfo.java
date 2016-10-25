package org.zeeman.thrifts.service;

import java.lang.annotation.Annotation;
import java.lang.reflect.Method;

class ServiceMetaInfo {
    private String serviceName;
    private String methodName;
    private Class<?> serviceHandlerType;
    private Method method;
    private Annotation annotation;

    public ServiceMetaInfo()
    {

    }

    public String getServiceName()
    {
        return this.serviceName;
    }

    public void setServiceName(String serviceName)
    {
        this.serviceName = serviceName;
    }

    public String getMethodName() {
        return methodName;
    }

    public void setMethodName(String methodName) {
        this.methodName = methodName;
    }

    public Class<?> getServiceHandlerType() {
        return serviceHandlerType;
    }

    public void setServiceHandlerType(Class<?> serviceHandlerType) {
        this.serviceHandlerType = serviceHandlerType;
    }

    public Annotation getAnnotation() {
        return annotation;
    }

    public void setAnnotation(Annotation annotation) {
        this.annotation = annotation;
    }

    public Method getMethod() {
        return method;
    }

    public void setMethod(Method method) {
        this.method = method;
    }
}
