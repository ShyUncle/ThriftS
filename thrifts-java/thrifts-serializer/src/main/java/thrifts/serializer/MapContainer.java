package thrifts.serializer;

import thrifts.common.annotations.ThriftSMember;
import thrifts.common.annotations.ThriftSModel;

import java.util.HashMap;

@ThriftSModel
final class MapContainer<TKey, TValue> {
    @ThriftSMember(tag = 1)
    private HashMap<TKey, TValue> value;

    public MapContainer()
    {
    }

    public MapContainer(HashMap<TKey, TValue> value)
    {
        this.value = value;
    }

    public HashMap<TKey, TValue> getValue() {
        return this.value;
    }

    public void setValue(HashMap<TKey, TValue> value) {
        this.value = value;
    }
}
