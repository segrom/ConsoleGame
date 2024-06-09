namespace ClientServerDb.Common;

public class ReactValue<T>
{
    private T _value;
    public T Value
    {
        get => _value;
        set
        {
            if(_value != null && _value.Equals(value)) return;
            var old = _value;
            _value = value;
            OnChange?.Invoke(_value);
            OnChangeWithOld?.Invoke(_value, old);
        }
    }
    
    /// <summary>
    /// Called when the value has been changed.
    /// Transmits the new value.
    /// </summary>
    public event Action<T>? OnChange;
    
    /// <summary>
    /// Called when the value has been changed.
    /// Transmits the new value and the old value.
    /// </summary>
    public event Action<T, T>? OnChangeWithOld;

    public ReactValue(T value)
    {
        _value = value;
    }
}

public class ReactString: ReactValue<string> {
    public ReactString(string value) : base(value) { }
}

public class ReactBoolean: ReactValue<bool> {
    public ReactBoolean(bool value) : base(value) { }
}


public class ReactInt: ReactValue<int> {
    public ReactInt(int value) : base(value) { }
}

public class ReactFloat: ReactValue<float> {
    public ReactFloat(float value) : base(value) { }
}