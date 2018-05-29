namespace Elarion.Saved.Events.Listeners {
    public interface IEventDispatcher<TParameter> {
        void AddListener(IEventListener<TParameter> listener);

        void RemoveListener(IEventListener<TParameter> listener);
    }
}