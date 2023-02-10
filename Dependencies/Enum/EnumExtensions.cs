using suffusive.uveitis.Dependencies.Model;

namespace suffusive.uveitis.Dependencies.Enum
{
    public static class EnumExtensions
    {
        public static string ToDescription(this ApplicationState state)
        {
            return state.ToString();
        }
    }
}