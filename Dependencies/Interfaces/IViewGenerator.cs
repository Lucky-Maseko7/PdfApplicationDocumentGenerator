namespace suffusive.uveitis.Dependencies.Interfaces
{
    public interface IViewGenerator
    {
        string GenerateFromPath(string url, object viewModel);
    }
}