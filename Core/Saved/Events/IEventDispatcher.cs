namespace Elarion.Saved.Events {
    public interface IEventDispatcher<TParameter> {
        void AddListener(IEventListener<TParameter> listener);

        void RemoveListener(IEventListener<TParameter> listener);
    }
}