namespace Elarion.Saved.Events.Listeners {
    public interface IEventListener<TParameter> {
        void OnEventRaised(TParameter value);
    }
}