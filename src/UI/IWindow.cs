interface IWindow
{
    string GetId();

    IWindow GetActiveNestedWindow();

    void Build();

    void Clear();

    void ClosePopups();
}
