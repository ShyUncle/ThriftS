package thrifts.service;

public final class ThriftSEnvirnment {
    private static IThriftSLogger logger = null;

    static
    {
        logger = new ThriftSLogger();
    }

    public static IThriftSLogger getLogger()
    {
        return logger;
    }

    public static void setLogger(IThriftSLogger value)
    {
        logger = value;
    }


}
