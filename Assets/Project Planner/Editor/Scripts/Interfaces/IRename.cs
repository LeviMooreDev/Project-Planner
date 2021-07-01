namespace ProjectPlanner
{
    public interface IRename
    {
        string GetName();
        void SetName(string value);
        void Validate();
    }
}
