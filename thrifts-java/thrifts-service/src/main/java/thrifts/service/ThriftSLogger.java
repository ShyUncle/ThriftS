package thrifts.service;

public class ThriftSLogger implements IThriftSLogger {

    public void Debug(String format, Object ... args) {
        System.out.printf(format, args);
        System.out.println();
    }

    public void Info(String format, Object ... args) {
        System.out.printf(format, args);
        System.out.println();
    }

    public void Error(String format, Object ... args) {
        System.out.printf(format, args);
        System.out.println();
    }

    public void Error(Exception exception) {
        exception.printStackTrace();
    }
}
