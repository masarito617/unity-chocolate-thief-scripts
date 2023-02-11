namespace Chocolate.Services
{
    public interface ITransitionService
    {
        public void LoadScene(string nextScene);
        public bool IsEqualPrevScene(string scene);
    }
}