package thrifts.service;

import java.util.TreeMap;

class LocalCache {
    public final static TreeMap<String,TreeMap<String,ServiceMetaInfo>> ServiceMap;

    static {
        ServiceMap = new TreeMap<String, TreeMap<String, ServiceMetaInfo>>(String.CASE_INSENSITIVE_ORDER);
    }
}
