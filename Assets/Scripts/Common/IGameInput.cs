namespace Common
{
    public interface IGameInput
    {
        string Name { get; }
        bool IsEnabled { get; set; }
        void Update(Spaceship spaceship);
    }
}