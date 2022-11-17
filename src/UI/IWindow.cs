using System.Collections.Generic;

public interface IWindow
{
    string GetId();

    IWindow GetActiveNestedWindow();

    void Build();

    void OnReturn();

    List<UIDynamicSlider> GetSliders();

    void Clear();

    void ClosePopups();
}
