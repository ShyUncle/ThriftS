package thrifts.common.annotations;

import thrifts.common.SerializerMode;

import java.lang.annotation.ElementType;
import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;
import java.lang.annotation.Target;

@Retention(RetentionPolicy.RUNTIME)
@Target(ElementType.TYPE)
public @interface ThriftSModel {
    SerializerMode SerializerMode() default SerializerMode.Auto;
}
