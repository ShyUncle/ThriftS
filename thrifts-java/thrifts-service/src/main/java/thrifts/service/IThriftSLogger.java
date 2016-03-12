package thrifts.service;

public interface IThriftSLogger {
    void Debug(String format, Object ... args);

    void Info(String format, Object ... args);

    void Error(String format, Object ... args);

    void Error(Exception exception);
}
