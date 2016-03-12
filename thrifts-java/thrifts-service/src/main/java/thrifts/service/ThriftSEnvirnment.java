package thrifts.service;

public final class ThriftSEnvirnment {
    private static IThriftXLogger logger = null;

    static
    {
        logger = new ThriftSLogger();
    }

    public static IThriftXLogger getLogger()
    {
        return logger;
    }

    public static void setLogger(IThriftXLogger value)
    {
        logger = value;
    }


}
