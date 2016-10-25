package org.zeeman.thrifts.serializer;

import org.zeeman.thrifts.common.annotations.ThriftSMember;
import org.zeeman.thrifts.common.annotations.ThriftSModel;

@ThriftSModel
final class ValueContainer<T> {
    @ThriftSMember(tag = 1)
    private T value;

//    private Class<?> valueType;

    public ValueContainer() {
    }

    public ValueContainer(T value) {
        this.value = value;
//        this.valueType = value.getClass();
    }

    public T getValue() {
        return this.value;
    }

    public void setValue(T value) {
        this.value = value;
    }
}
