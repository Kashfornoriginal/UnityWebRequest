namespace Services.Factory
{
    public interface IUIFactory
    {
        public void CreateText(string textMessage);
        public void ClearText();
    }
}