package thrifts.serializer;

import thrifts.common.annotations.ThriftSMember;
import thrifts.common.annotations.ThriftSModel;

import java.util.ArrayList;
import java.util.List;

@ThriftSModel
final class ListContainer<T> {

    @ThriftSMember(tag = 1)
    private ArrayList<T> value;

//    private Class<?> itemType;

    public ListContainer()
    {
    }

    public ListContainer(ArrayList<T> value)
    {
        this.value = value;
//        this.itemType = value.getClass();

    }

    public List<T> getValue() {
        return this.value;
    }

    public void setValue(ArrayList<T> value) {
        this.value = value;
    }
}
