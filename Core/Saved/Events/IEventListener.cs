namespace Elarion.Saved.Events {
    public interface IEventListener<TParameter> {
        void OnEventRaised(TParameter value);
    }
}