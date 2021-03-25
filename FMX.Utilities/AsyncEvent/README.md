# AsyncEvent Utility
A utility for allowing async event listener handlers.
Handlers are executed asyncronously but in sequential order.

---

### Example Usage

Setup events that will be available to listen for.
```csharp
public class WithMessageEventArgs : AsyncEventArgs {
    public string Message {get; set;}
    internal WithMessageEventArgs(string msg) : base() {
        Message = msg;
    }
}

public class TestClassWithEvent {
    public event AsyncEventHandler<TestClassWithEvent, WithMessageEventArgs> OnPlannedEvent;

    public TestClassWithEvent(){
        ...
    }

    ...

    public void TriggersEvent(){
        OnPlannedEvent?.Invoke(this, new WithMessageEventArgs("Some unique message"));
    }
}
```

Usage with async event handlers
```csharp
    private TestClassWithEvent _testClass;
    ...

        _testClass.OnPlannedEvent += _testClassEventHandler;

        public async Task _testClassEventHandler(TestClassWithEvent sender, WithMessageEventArgs e){
            Console.WriteLine(e.Message);
        }

    ...
```
