
namespace EditorTools.Views
{
    public abstract class DataView<T> : IDataView
    {
        private T m_Data;

        public DataView(T data)
        {
            Update(data);
        }

        public void Update(T data) => m_Data = data;

        public void OnGUI()
        {
            OnDataGUI(m_Data);
        }

        protected abstract void OnDataGUI(T data);
    }

    public interface IDataView
    {
        void OnGUI();
    }
}